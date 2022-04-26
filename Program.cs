using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlexibileMessageBox.FlexibileMessageBox;
using InfoBox;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamVR_Toggle_for_Viveport.Properties;
using Titanium;
using Icon = System.Drawing.Icon;

namespace SteamVR_Toggle_for_Viveport
{
	static class Program
	{
		private static string[] _SteamVRProcesses =
		{
			"vrdashboard", "vrserver", "vrservice",
			"vrmonitor", "vrcompositor",
			"steamvr_tutorial", "steamtours",
			"vrwebhelper"
		};

		private static  string[] _SteamVR64exes =
		{
			"vrdashboard.exe", "vrserver.exe",  "vrservice.exe", "vrmonitor.exe", "vrcompositor.exe"
		};

		private static string _SteamInstallPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 250820").GetValue("InstallLocation")?.ToString().Replace("\\","/")?? @"C:/Program Files (x86)/Steam/steamapps/common/SteamVR/";
		private static string _SteamVRexePath => _SteamInstallPath.Replace("\\","/").Add("/") + "bin/win64/";

		public static bool _isSteamVRBroken
		{
			get
			{
				try
				{
					return new FileInfo(_SteamVRexePath + _SteamVR64exes[0]).Length < 10;
				}
				catch (Exception e)
				{
					return true;
				}
			}
		}

		public static NotifyIcon notifyIcon1 = null;
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			var openvrpaths = $@"c:\Users\{Environment.UserName}\AppData\Local\openvr\openvrpaths.vrpath";

			if (File.Exists(openvrpaths))
			{
				var content = File.ReadAllText(openvrpaths);
				var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

				if (json.ContainsKey("runtime"))
				{
					var item = (JArray)json["runtime"];
					// ReSharper disable once PossibleInvalidOperationException
					if ((bool)item?.Contains("SteamVR"))
						_SteamInstallPath = item.First.ToString();
				}
			}

			if (!Directory.Exists(_SteamVRexePath))
			{
				var dialog = InformationBox.Show("SteamVR not found! The programm will not work if you continue", title: "Error", icon: InformationBoxIcon.Error, buttons: InformationBoxButtons.User1User2, customButtons: new[] { "Exit", "Cancel" });
				switch (dialog)
				{
					case InformationBoxResult.User1: return;
					case InformationBoxResult.User2: break;
				}
			}

			ContextMenuStrip contextMenu = new ContextMenuStrip();
			

			notifyIcon1 = new NotifyIcon();
			notifyIcon1.ContextMenuStrip = contextMenu;
			contextMenu.GenerateMenuOptions();
			notifyIcon1.Visible = true;
			notifyIcon1.MouseClick += (Sender, Args) =>
			{
				switch (Args.Button)
				{
					case MouseButtons.Right: contextMenu.GenerateMenuOptions(); break;

					case MouseButtons.Left: if (Settings.Default.OneClickMode) ToggleSteamVR_Click(null,null); break;

					case MouseButtons.Middle: if(Settings.Default.OneClickMode) KillSteamVR(); break;
				}
				
			};
			notifyIcon1.Icon = Icon.ExtractAssociatedIcon($"icons/{(_isSteamVRBroken ? "Fix" : "Break")} SteamVR.ico");
			Application.Run();
			notifyIcon1.Visible = false;
		}

		private static void GenerateMenuOptions(this ContextMenuStrip contextMenu)
		{
			List<ToolStripMenuItem> items = new List<ToolStripMenuItem>(4);

			{
				ToolStripMenuItem item = new ToolStripMenuItem();
				item.Text = "Launch with Windows";
				item.CheckState = IsAutorun() ? CheckState.Checked : CheckState.Unchecked;
				item.CheckOnClick = true;
				item.Click += delegate
				{
					SetAutorunValue(item.Checked);
				};
				items.Add(item);
			}

			{
				ToolStripMenuItem item = new ToolStripMenuItem();
				item.Text = "Enable one-click mode";
				item.Checked = Settings.Default.OneClickMode;
				item.CheckOnClick = true;
				item.Click += delegate
				{
					Settings.Default.OneClickMode = item.Checked;
					Settings.Default.Save();
				};
				items.Add(item);
			}

			if(Settings.Default.OneClickMode)
			{
				ToolStripMenuItem item = new ToolStripMenuItem();
				item.Text = "Control tips";
				item.Click += delegate
				{
					FlexibleMessageBox.FONT = new Font("Consolas", 8);
					FlexibleMessageBox.MAX_HEIGHT_FACTOR = 1.0;
					FlexibleMessageBox.Show("Right Click ( |■) – Open context menu\nLeft Click (■| ) – Break/Restore SteamVR\nMiddle Click ( ⫯ ) – Kill SteamVR", "Control tips", MessageBoxButtons.OK, MessageBoxIcon.Information);
				};
				items.Add(item);
			}

			if (IsSteamVRActive())
			{
				ToolStripMenuItem item = new ToolStripMenuItem();
				item.Text = "Kill SteamVR";
				item.Click += delegate
				{
					KillSteamVR();
				};
				items.Add(item);
			}

			{
				ToolStripMenuItem item = new ToolStripMenuItem();
				item.Text = (_isSteamVRBroken ? "Restore" : "Break") + " SteamVR";
				item.Click += ToggleSteamVR_Click;
				items.Add(item);
			}

			{
				ToolStripMenuItem item = new ToolStripMenuItem();
				item.Text = "Exit";
				item.Click += delegate
				{
					Application.Exit();
				};
				items.Add(item);
			}

			contextMenu.Items.Clear();
			contextMenu.Items.AddRange(items.ToArray());
			notifyIcon1.Icon = Icon.ExtractAssociatedIcon($"icons/{(_isSteamVRBroken ? "Fix" : "Break")} SteamVR.ico");
		}

		private static void ToggleSteamVR_Click(object sender, EventArgs args)
		{
			if (_isSteamVRBroken) RestoreSteamVR();
			else BreakSteamVR();
		}



		private static  void KillSteamVR()
		{
			foreach (var name in _SteamVRProcesses)
			{
				var processes = Process.GetProcessesByName(name);
				if (processes.Length == 0)
					continue;

				foreach (var process in processes)
					process.Kill();
			}
		}

		private static  bool IsSteamVRActive()
		{
			foreach (var name in _SteamVRProcesses)
			{
				var processes = Process.GetProcessesByName(name);
				if (processes.Length > 0)
					return true;
			}

			return false;
		}

		private static async void BreakSteamVR()
		{
			KillSteamVR();
			for (int i = 0; i < _SteamVR64exes.Length; i++)
			{
				string path = _SteamVRexePath + _SteamVR64exes[i];
				if (!IsWeightless(path))
				{
					try
					{
						File.Move(path,path+".bak",true); //:Rename
						if (!File.Exists(path + ".bak")) throw new OperationCanceledException();
						File.Create(path, 1).Close();
					}
					catch (OperationCanceledException)
					{
						var dialog = System.Windows.Forms.MessageBox.Show("WTF File.Move haven't been executed while no exceptions thrown?", "WTF error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
						switch (dialog)
						{
							case DialogResult.Abort: return;
							case DialogResult.Retry: i--; break;
						}
					}
				}
			}

			notifyIcon1.Icon = Icon.ExtractAssociatedIcon("icons/Fix SteamVR.ico");
		}

		private static void RestoreSteamVR()
		{
			KillSteamVR();

			for (int i = 0; i < _SteamVR64exes.Length; i++)
			{
				string path = _SteamVRexePath + _SteamVR64exes[i];
				if (IsWeightless(path))
				{
					try
					{
						//File.Delete(path);
						File.Move(path + ".bak", path, true); //:Rename and replace
					}
					catch (FileNotFoundException)
					{
						var dialog = System.Windows.Forms.MessageBox.Show("Error restoring SteamVR: no backup found. Restore it yourself by checking SteamVR integrity files in Steam", "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
						switch (dialog)
						{
							case DialogResult.Abort: return;
							case DialogResult.Retry: i--; break;
						}
						continue;
					}
					catch (Exception e)
					{
						var dialog = System.Windows.Forms.MessageBox.Show($"Error restoring SteamVR: {e.Message}", "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
						switch (dialog)
						{
							case DialogResult.Abort: return;
							case DialogResult.Retry: i--; break;
						}
						continue;
					}
				}
			}

			notifyIcon1.Icon = Icon.ExtractAssociatedIcon("icons/Break SteamVR.ico");
		}

		private static bool IsWeightless(string path) => new FileInfo(path).Length < 10;

		public static bool SetAutorunValue(bool autorun)
		{
			string ExePath = System.Windows.Forms.Application.ExecutablePath;
			var reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
			try
			{
				if (autorun) reg.SetValue(Application.ProductName, ExePath);
				else reg.DeleteValue(Application.ProductName);
 
				reg.Close();
			}
			catch { return false; }

			return true;
		}

		public static bool IsAutorun() => Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\")?.GetValue(Application.ProductName) is not null;
	}
}
