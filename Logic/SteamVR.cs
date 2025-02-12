using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamVR_OculusDash_Switcher.Properties;
using SteamVR_OculusDash_Switcher.Properties.Localization;
using Titanium;
using TaskDialog = Ookii.Dialogs.WinForms.TaskDialog;

namespace SteamVR_OculusDash_Switcher.Logic
{
    public class SteamVR
	{
		public BreakMethod Method;
		private static string _steamVrFolderPath;
		private static string _steamVRexeFolderPath => _steamVrFolderPath.Add("\\") + @"bin\win64\";
		public bool IsBroken;
		public static bool IsSteamVRExeFolderExist =>  Directory.Exists(_steamVRexeFolderPath);
		private static readonly string[] _steamVRProcesses =
		{
			"vrdashboard", "vrserver", "vrservice",
			"vrmonitor", "vrcompositor",
			"steamvr_tutorial", "steamtours",
			"vrwebhelper"
		};

		private static readonly string[] _steamVR64exes =
		{
			"vrdashboard.exe", "vrserver.exe",  "vrservice.exe", "vrmonitor.exe", "vrcompositor.exe"
		};

		public SteamVR(BreakMethod CurrentMethod)
		{
			Method = CurrentMethod;
			if (!LocateSteamVR()) return;

			var realCurrentMethod = WhatMethodApplied();
			if (realCurrentMethod != BreakMethod.None) Method = realCurrentMethod;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>false if Method "rename folder" is applied</returns>
		public bool LocateSteamVR()
		{
			bool methodFound = false;
			var openvrpaths = $@"c:\Users\{Environment.UserName}\AppData\Local\openvr\openvrpaths.vrpath";
			_steamVrFolderPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 250820")?.GetValue("InstallLocation")?.ToString() ?? @"C:\Program Files (x86)\Steam\steamapps\common\SteamVR\";

			if (!Directory.Exists(_steamVRexeFolderPath))
			{
				if (File.Exists(openvrpaths))
				{
					var content = File.ReadAllText(openvrpaths);
					var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

					if (json.ContainsKey("runtime"))
					{
						var item = (JArray)json["runtime"];
						if (item != null && item.First.ToString().Contains("SteamVR"))
						{
							_steamVrFolderPath = item.First.ToString();
						}
					}
				}

				if (!Directory.Exists(_steamVrFolderPath))
				{
					if (Directory.Exists(_steamVrFolderPath.ReplaceFromLast("SteamVR", "SteamVR_", false)))
					{
						Method = BreakMethod.RenameFolder;
						IsBroken = true;
						methodFound = true;
					} 
					else throw new DirectoryNotFoundException("SteamVR Folder not found");
				}
				
			}
			return !methodFound;
		}

		public override bool Equals(object obj) =>
			obj switch
			{
				SteamVR svr => svr.Method == Method,
				BreakMethod bm => bm == Method,
				SteamVRMethod svrm => svrm.Value == Method,
				_ => obj != null && (int)obj == (int)Method
			};

		protected bool Equals(SteamVR other) => Method == other.Method;

		public override int GetHashCode() => (int)Method;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Method"></param>
		/// <returns>true if method applied, false if not, null if error</returns>
		public bool? IsMethodApplied(BreakMethod? Method)
		{
			if (Method is not (BreakMethod.RenameFolder or BreakMethod.None) && ! IsSteamVRExeFolderExist)
			{
				return null;
			}

			switch (Method)
			{
				case BreakMethod.None:
				{
					return IsMethodApplied(BreakMethod.RenameFolder) is false && IsMethodApplied(BreakMethod.RenameExe) is false && IsMethodApplied(BreakMethod.DummyExe) is false;
				}

				case BreakMethod.RenameFolder:
				{
					return Directory.Exists(_steamVRexeFolderPath.ReplaceFromLast("SteamVR", "SteamVR_"));
				}

				case BreakMethod.RenameExe:
				{
					
					foreach (var exe in _steamVR64exes)
					{
						if (File.Exists(_steamVRexeFolderPath.Add("\\") + exe)) return false;
					}

					foreach (var exe in _steamVR64exes)
					{
						if (!File.Exists(_steamVRexeFolderPath.Add("\\") + exe + "_")) throw new FileLoadException(LocalizationStrings.SteamVRDisable_IsMethodApplied_RenameExe_Exception__both_original_and_renamed_file_not_found,_steamVRexeFolderPath + exe);
					}

					return true;
				}

				case BreakMethod.DummyExe:
				{
					return new FileInfo(_steamVRexeFolderPath.Add("\\") + _steamVR64exes[0]).Length < 10;
				}

				default:
					throw new ArgumentOutOfRangeException(nameof(Method));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Method"></param>
		/// <returns>true if method applied, false if not, null if error</returns>
		public bool? IsMethodApplied() => IsMethodApplied(Method);

		private static BreakMethod? whatMethodApplied()
		{
			if (IsSteamVRExeFolderExist)
			{
				var allExesExist = _steamVR64exes.All(exe => File.Exists(_steamVRexeFolderPath.Add("\\") + exe));
				if (!allExesExist)
				{
					var allExesRenamed =  _steamVR64exes.All(exe => File.Exists(_steamVRexeFolderPath.Add("\\") + exe + "_"));
					if (allExesRenamed) return BreakMethod.RenameExe;
					return null;
				}
				else if (new FileInfo(_steamVRexeFolderPath.Add("\\") + _steamVR64exes[0]).Length < 10) return BreakMethod.DummyExe;
				else return BreakMethod.None;
			}
			else
			{
				return Directory.Exists(_steamVRexeFolderPath.ReplaceFromLast("SteamVR", "SteamVR_"))? BreakMethod.RenameFolder : null;
			}
		}

		/// <summary>
		/// Checks what STeamVR disabling method is applied
		/// </summary>
		/// <returns>
		/// <para>BreakMethod (including None) or null if SteamVR is broken with other methods (like "кривые руки" method :D)</para>
		/// </returns>
		public BreakMethod WhatMethodApplied(bool ChangeCurrentMethod = false, bool Retry = true)
		{
			var method = whatMethodApplied();
			if (method != null)
			{
				if (ChangeCurrentMethod) this.Method = (BreakMethod)method;
			}
			else
			{
				if (!Retry) throw new InvalidOperationException(Directory.Exists(_steamVRexeFolderPath) ? "Something go wrong in WhatMethodApplied while SteamVR is found. How it's even possible?" : "SteamVR not found. But why only now, in WhatMethodApplied method?");
				LocateSteamVR();
				return WhatMethodApplied(ChangeCurrentMethod, false);
			}

			IsBroken = method != BreakMethod.None;
			return (BreakMethod)method;
		}

		public void Break()
		{
			if(Method == BreakMethod.None) return;

			KillProcess();

			try
			{
				switch (Method)
				{
					case BreakMethod.RenameFolder:
						Directory.Move(_steamVrFolderPath, _steamVrFolderPath + "_"); //:Rename
						Settings.Default.Current_SteamVRDisablingMethod = BreakMethod.RenameFolder;
						break;

					case BreakMethod.RenameExe:
					{
						foreach (var exe in _steamVR64exes)
						{
							string path = Path.Combine(_steamVRexeFolderPath, exe);
							File.Move(path,path+"_"); //: Rename
							Settings.Default.Current_SteamVRDisablingMethod = BreakMethod.RenameFolder;
						}
					} break;

					case BreakMethod.DummyExe:
					{
						for (int i = 0; i < _steamVR64exes.Length; i++)
						{
							string path = _steamVRexeFolderPath + _steamVR64exes[i];
							if (IsWeightless(path)) continue;
							try
							{
								File.Move(path,path+".bak",true); //:Rename
								if (!File.Exists(path + ".bak")) throw new OperationCanceledException();
								File.Create(path, 1).Close();
								Settings.Default.Current_SteamVRDisablingMethod = BreakMethod.RenameFolder;
							}
							catch (OperationCanceledException)
							{
								var dialog = MessageBox.Show("WTF File.Move haven't been executed while no exceptions thrown?", "WTF error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
								switch (dialog)
								{
									case DialogResult.Abort: return;
									case DialogResult.Retry: i--; break;
								}
							}
						}
					} break;
					default:
						throw new ArgumentOutOfRangeException(nameof(Method));
				}
			IsBroken = true;
			}
			catch (Exception e)
			{
				//TODO: Localize
				throw new InvalidOperationException($"Can't restore using method {new SteamVRMethod(Method)}", e);
			}
		}
		public void Restore()
		{
			var CurrentMethod = Method;

			Settings.Default.Current_SteamVRDisablingMethod = BreakMethod.None;
			if(CurrentMethod == BreakMethod.None) 
				return;

			KillProcess();

			try
			{
				switch (CurrentMethod)
				{
					case BreakMethod.RenameFolder:
						//TODO: check is it takes a long time
						if (Directory.Exists(_steamVrFolderPath + "_") && Directory.EnumerateFileSystemEntries(_steamVrFolderPath + "_").Any()) //: Check if folder exists and not empty
						 if (Directory.Exists(_steamVrFolderPath))
							 if (!Directory.EnumerateFileSystemEntries(_steamVrFolderPath).Any())
								Directory.Delete(_steamVrFolderPath + "_", true);
							 else // discribe all ifs: if backup exist and not empty and SteamVR folder exist and not empty
							 {
								 var answer = new TaskDialog().Show(LocalizationStrings.SteamVR_Restore_RenameFolder_Error_BothFoldersExist_Content, LocalizationStrings.MessageBox_Title__Error, null, LocalizationStrings.SteamVR_Restore_RenameFolder_Error_BothFoldersExist_Option_Replace, LocalizationStrings.SteamVR_Restore_RenameFolder_Error_BothFoldersExist_DeleteBackup, LocalizationStrings.Button_cancel, LocalizationStrings.Button_skip).Text;

								 if (answer == LocalizationStrings.SteamVR_Restore_RenameFolder_Error_BothFoldersExist_Option_Replace)
								 {
									Directory.Delete(_steamVrFolderPath, true);
									Directory.Move(_steamVrFolderPath + "_", _steamVrFolderPath);
								 }
									 else if (answer == LocalizationStrings.SteamVR_Restore_RenameFolder_Error_BothFoldersExist_DeleteBackup)
								 { 
									 Directory.Delete(_steamVrFolderPath + "_", true);
								 }
									 else if (answer == LocalizationStrings.Button_cancel)
								 {
									 return;
								 }
									 else if (answer == LocalizationStrings.Button_skip)
								 {
									 
								 }
							 }
						 else //: if backup exist and not empty and SteamVR folder not exist
						 {
								 Directory.Move(_steamVrFolderPath + "_", _steamVrFolderPath);
						 }
						else //: if backup not exist or empty
						{
							if (Directory.Exists(_steamVrFolderPath)) return;
							throw new FileNotFoundException(LocalizationStrings.SteamVR_Restore_RenameFolder_Error_NoBackup);
						}
								
					break;

					case BreakMethod.RenameExe:
					{
						foreach (var exe in _steamVR64exes)
						{
							string path = Path.Combine(_steamVRexeFolderPath, exe);
							File.Move(path+"_",path); //: Rename
						}
					} break;

					case BreakMethod.DummyExe:
					{
						for (int i = 0; i < _steamVR64exes.Length; i++)
						{
							string path = _steamVRexeFolderPath + _steamVR64exes[i];
							if (!IsWeightless(path)) continue;
							try
							{
								//File.Delete(path);
								File.Move(path + ".bak", path, true); //:Rename and replace
							}
							catch (FileNotFoundException)
							{
								//TODO: Localize
								var dialog = MessageBox.Show("Error restoring SteamVR: no backup found. Restore it yourself by checking SteamVR integrity files in Steam", LocalizationStrings.MessageBox_Title__Error, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
								switch (dialog)
								{
									case DialogResult.Abort: return;
									case DialogResult.Retry: i--; break;
								}
								continue;
							}
							catch (Exception e)
							{
								//TODO: Localize
								var dialog = MessageBox.Show($"Error restoring SteamVR: {e.Message}", LocalizationStrings.MessageBox_Title__Error, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
								switch (dialog)
								{
									case DialogResult.Abort: return;
									case DialogResult.Retry: i--; break;
								}
								continue;
							}
						}
					} break;

					case BreakMethod.None:
					{

					} break;

					default:
						throw new ArgumentOutOfRangeException(nameof(Method));
				}
			IsBroken = false;
			}
			catch (Exception e)
			{
				//TODO: Localize
				throw new InvalidOperationException($"Can't restore using method {new SteamVRMethod(Method)}", e);
			}
		}

		public static void KillProcess()
		{
			foreach (var name in _steamVRProcesses)
			{
				var processes = Process.GetProcessesByName(name);
				if (processes.Length == 0)
					continue;

				foreach (var process in processes)
					process.Kill();
			}
		}

		public static bool IsActive()
		{
			foreach (var name in _steamVRProcesses)
			{
				var processes = Process.GetProcessesByName(name);
				if (processes.Length > 0)
					return true;
			}

			return false;
		}
		private static bool IsWeightless(string Path) => new FileInfo(Path).Length < 10;
	}
}
