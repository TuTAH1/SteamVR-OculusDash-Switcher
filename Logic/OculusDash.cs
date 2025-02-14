using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using SteamVR_OculusDash_Switcher.Properties.Localization;
using Titanium;
using ProgressBarStyle = Ookii.Dialogs.WinForms.ProgressBarStyle;
using TaskDialogButton = Ookii.Dialogs.WinForms.TaskDialogButton;

namespace SteamVR_OculusDash_Switcher.Logic
{
	public static class OculusDash
	{
		private static readonly string _oculusFolderPath;
		/// <summary>OculusDash.exe path</summary>
		private static readonly string _oculusDashExePath;
		///<summary>Oculus killere exe's location inside of this program's folder</summary>
		const string _innerOculusKillerPath = @"OculusDashKiller\OculusDash.exe";
		private static bool _oculusKillerChecked = false; //: If OculusKiller is checked for updates
		public static DashState State; //: If OculusDash is replaced by OculusKiller. Null: not exist
		public static bool IsOculusKillerExists => File.Exists(_innerOculusKillerPath);
		public static bool IsOculusExists => _oculusFolderPath != null;
		public static bool IsDashExists => State != null; //: If OculusDash.exe exist at all, no matter if killed or not

		static OculusDash()
		{
			_oculusFolderPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Oculus")?.GetValue("InstallLocation")?.ToString()?? (Directory.Exists( @"C:\Program Files\Oculus\")?  @"C:\Program Files\Oculus\" : null); //: Check where Oculus is installed, if not found, set default path

			if (_oculusFolderPath == null) //: Look for Oculus Folder in the root of all drives 
			{
				var drives = DriveInfo.GetDrives();
				foreach (var drive in drives)
				{
					string driveLetter = drive.RootDirectory.FullName;
					string oculusFolderPath = Path.Combine(driveLetter, @"Oculus\");
					if (!Directory.Exists(oculusFolderPath)) continue;

					_oculusFolderPath = oculusFolderPath;
					break;
				}

				if (_oculusFolderPath == null) //: Ask user to manually select Oculus Folder
				{
					using var fbd = new FolderBrowserDialog();
					fbd.Description = LocalizationStrings.OculusDash_OculusNotFound__Select_Oculus_folder_path;
					DialogResult result = fbd.ShowDialog();

					if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
					{
						string[] files = Directory.GetFiles(fbd.SelectedPath);
					}
				}
			}

			

			_oculusDashExePath = _oculusFolderPath?.Add("\\" + @"Support\oculus-dash\dash\bin\OculusDash.exe");
			State = IsOculusDashKilled();
		}

		
		

		//:Checks if OculusKiller exe exist and downloads the lastest release if not
		private static async Task<GitHub.Status> checkOculusKillerAsync(bool checkUpdates)
		{
			const string oculusDashRepository = @"ItsKaitlyn03/OculusKiller";
				return (await GitHub.checkSoftwareUpdatesByLinkAsync(checkUpdates? GitHub.UpdateMode.Update : GitHub.UpdateMode.Download, oculusDashRepository, _innerOculusKillerPath, "OculusDashKiller").ConfigureAwait(false)).Status;
		}

		/// <summary>
		/// Checks if OculusKiller exe exist and otherwise downloads the lastest release (also tries to update, if <param name="checkUpdates"/> is true)
		/// </summary>
		public static async Task<GitHub.Status> CheckKillerAsync(bool checkUpdates = false)
		{
			var status = await checkOculusKillerAsync(checkUpdates).ConfigureAwait(false);
			_oculusKillerChecked = true; //: Sets true if no exceptions
			return status;
		}

		public enum DashState
		{
			Killed, //: OculusDash.exe is replaced by OculusKiller and can be recovered
			NotKilled,
			NotExist, //: or unrecoverably broken
			DashBackupOnly //: OculusDash.exe is not exists, but backup is exists
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// <para>false, if not killed </para>
		/// <para>true, if killed and restoring is posible (orig file exist)</para>
		/// <para>null, if killed and orig is not exist</para>
		/// </returns>
		private static DashState checkOculusDashKilled()
		{
			var oculusDashExeFileInfo = new FileInfo(_oculusDashExePath);

			if (!oculusDashExeFileInfo.Exists) //? OculusDash.exe is NOT exist
			{
				var backup = new FileInfo(_oculusDashExePath + "_"); //. ".../OculusDash.exe_"
				if (!backup.Exists) //? Backup is NOT exist
					return DashState.NotExist;
				else //. Backup exist, but OculusDash.exe is not exist
				{
					return isDashReplacedByKiller(backup) ? DashState.NotExist : DashState.DashBackupOnly;
				}

			} //? OculusDash.exe exist
			return isDashReplacedByKiller(oculusDashExeFileInfo) ? DashState.Killed : DashState.NotKilled; 
		
		}

		private static bool isDashReplacedByKiller(FileInfo OculusDashExeFileInfo) => OculusDashExeFileInfo.Exists? OculusDashExeFileInfo.Length <= (IsOculusKillerExists ? new FileInfo(_innerOculusKillerPath).Length : 1000) : throw new FileNotFoundException("OculusDash not found when checking is it replaced by OculusDashKiller", OculusDashExeFileInfo.Name);

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// <para>false, if not killed </para>
		/// <para>true, if killed and restoring is posible (orig file exist)</para>
		/// <para>null, if killed and orig is not exist</para>
		/// </returns>
		public static DashState IsOculusDashKilled()
		{
			State = checkOculusDashKilled();
			return State;
		}

		public static async Task BreakAsync()
		{
			if (!_oculusKillerChecked)
			{
				var progressDialog = new Ookii.Dialogs.WinForms.ProgressDialog() //: не закрывается, заменить на  ProgressDialog.NET
				{
					WindowTitle = "Checking OculusKiller, downloading...",
					ProgressBarStyle = ProgressBarStyle.MarqueeProgressBar
				};
				progressDialog.ShowDialog();
				CheckKillerAsync().Wait();
				progressDialog.Dispose();
			}
			if(IsOculusDashKilled()!= DashState.NotKilled) return;
			File.Move(_oculusDashExePath, _oculusDashExePath + "_", true); //: Backup
			File.Copy(_innerOculusKillerPath, _oculusDashExePath,true); //: Replace with Oculus Killer
			State = DashState.Killed;
		}

		public static void Restore()
		{
			if(File.Exists(_oculusDashExePath+"_")) File.Move(_oculusDashExePath+"_",_oculusDashExePath, true);
			State = DashState.NotKilled;
		}
	}
}
