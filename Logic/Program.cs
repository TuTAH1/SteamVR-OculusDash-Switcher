﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using InfoBox;
using Microsoft.Win32;
using Titanium;
using Icon = System.Drawing.Icon;
using SteamVR_OculusDash_Switcher.Forms;
using SteamVR_OculusDash_Switcher.Logic;
using SteamVR_OculusDash_Switcher.Properties;
using SteamVR_OculusDash_Switcher.Properties.Localization;
using static SteamVR_OculusDash_Switcher.Logic.OculusDash.DashState;

namespace SteamVR_OculusDash_Switcher
{
	static partial class Program
	{
		public static NotifyIcon notifyIcon1 = null;
		public static SteamVR _SteamVr;
			
			/*_SteamVrMethod.Method == SteamVR.BreakMethod.None? _SteamVrMethod.IsMethodApplied() : _SteamVrMethod.IsMethodApplied() switch
		{
			true => false,
			false => true,
			null => throw new ArgumentNullException("")
		};*/
		public static int _IconsRealismLevel = Settings.Default.RealismLevel;
		public static int _MaxIconsRealismLevel = 3;

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory); //: Костыль for autostart

			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(true);
			if (_IconsRealismLevel <0 || _IconsRealismLevel > _MaxIconsRealismLevel) _IconsRealismLevel = 1;


			try //: Try to find SteamVR
			{
				_SteamVr = new SteamVR(Settings.Default.SteamVRDisablingMethod);
				Settings.Default.SteamVRDisablingMethod = _SteamVr.Method;
				Settings.Default.Save();
			}
			catch (Exception e)
			{
				new Exception(LocalizationStrings.MessageBox_Main_Error_SteamVrNotFound_Text, e).ShowMessageBox(LocalizationStrings.MessageBox_Main_Error_SteamVrNotFound_Title);
				/*var dialog = InformationBox.Show(, title: , icon: InformationBoxIcon.Error, buttons: InformationBoxButtons.User1User2, customButtons: new[] { LocalizationStrings.Button__Exit, LocalizationStrings.Button__Continue });
				switch (dialog)
				{
					case InformationBoxResult.User1: return;
					case InformationBoxResult.User2: break;
				}*/
			}

			if (OculusDash.IsOculusExists)
			{
				try
				{
					OculusDash.IsOculusDashKilled();
					if (OculusDash.State is NotExist)
					{
						MessageBox.Show(LocalizationStrings.Program_Main_Error_OculusDashUnrecoverable_Text, LocalizationStrings.Program_Main_Error_OculusDashUnrecoverable_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				catch (Exception e)
				{
					e.ShowMessageBox();
				}
			}
			else
			{
				Settings.Default.KillOculusDash = false;
			}

			if (!OculusDash.IsDashExists && _SteamVr.IsBroken && OculusDash.State is Killed)
			{
				switch (InformationBox.Show("Both SteamVR and OculusDash properly broken. What to restore?", "Choosing a side", buttons: InformationBoxButtons.User1User2, icon: InformationBoxIcon.Error, customButtons: new[] { "SteamVR", "Oculus" }))
				{
					case InformationBoxResult.User1:
						_SteamVr.Restore();
						break;

					case InformationBoxResult.User2:
						OculusDash.Restore();
						break;
				}
			}


			ContextMenuStrip contextMenu = new();
			
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
			notifyIcon1.Icon = GetIcon();
			//new Form1().Show(); //!Debug
			Application.Run();
			notifyIcon1.Visible = false;
		}

		private static void GenerateMenuOptions(this ContextMenuStrip contextMenu)
		{
			List<ToolStripMenuItem> items = new List<ToolStripMenuItem>(4);

			//: MORE SETTINGS
			{
				ToolStripMenuItem item = new();
				item.Text = LocalizationStrings.MenuOptions__More_Settings;
				item.Image =  GetImage("Menu Options", "Settings");
				item.Click += delegate
				{
					new SettingsForm().Show();
				};
				items.Add(item);
			}

			//: LAUNCH WITH WINDOWS
			{
				ToolStripMenuItem item = new();
				item.Text = LocalizationStrings.MenuOptions__Launch_with_Windows;
				item.Image =  GetImage("Menu Options", "Windows");
				item.CheckState = IsAutorun() ? CheckState.Checked : CheckState.Unchecked;
				item.CheckOnClick = true;
				item.Click += delegate
				{
					SetAutorunValue(item.Checked);
				};
				items.Add(item);
			}

			//: One Click Mode
			{ 
				ToolStripMenuItem item = new();
				item.Text = LocalizationStrings.MenuOptions__Enable_one_click_mode;
				item.Checked = Settings.Default.OneClickMode;
				item.CheckOnClick = true;
				item.Click += delegate
				{
					Settings.Default.OneClickMode = item.Checked;
					Settings.Default.Save();
				};
				items.Add(item);
			}

			//: Control Tips
			if(Settings.Default.OneClickMode)
			{ 
				ToolStripMenuItem item = new();
				item.Text = LocalizationStrings.MenuOptions__Control_tips;
				item.Image = GetImage("Menu Options", "Question");
				item.Click += delegate //TODO: Replace it with hand-made Form with images
				{
					new ControlsTipsForm().Show();
				};
				items.Add(item);
			}

			//: Kill SteamVR
			if (SteamVR.IsActive() && Settings.Default.KillSteamVR_Enabled)
			{
				ToolStripMenuItem item = new();
				item.Text = LocalizationStrings.MenuOptions__Kill_SteamVR;
				item.Image = GetImage("Menu Options", "Knife");
				item.Click += delegate
				{
					KillSteamVR();
				};
				items.Add(item);
			}

			//: Break/Restore SteamVR
			{
				ToolStripMenuItem item = new();
				item.Image = GetImage("Menu Options", "Hammer"); 
				item.Text = (Settings.Default.KillOculusDash? 
					(OculusDash.IsOculusExists? LocalizationStrings.MenuOptions__Switch_to_SteamVR : LocalizationStrings.MenuOptions__Switch_to_Oculus) : //TODO: Oculus and SteamVR icon
					(_SteamVr.IsBroken ? LocalizationStrings.MenuOptions__Restore_SteamVR : LocalizationStrings.MenuOptions__Break_SteamVR));
				item.Click += ToggleSteamVR_Click;
				items.Add(item);
			}

			//: Exit
			{
				ToolStripMenuItem item = new();
				item.Text = LocalizationStrings.Button__Exit;
				item.Click += delegate
				{
					Application.Exit();
				};
				items.Add(item);
			}

			contextMenu.Items.Clear();
			contextMenu.Items.AddRange(items.ToArray());
			notifyIcon1.Icon = GetIcon();
		}

		private static async void ToggleSteamVR_Click(object sender, EventArgs args)
		{
			try
			{
				if (Settings.Default.KillOculusDash) //? SteamVR ↔ Oculus mode
				{
					switch (OculusDash.State)
					{
						case Killed:
						case DashBackupOnly:
							_SteamVr.Break();
							OculusDash.Restore();
							break;
						case NotKilled:
							_SteamVr.Restore();
							await OculusDash.BreakAsync();
							break;
						case NotExist:
							Settings.Default.KillOculusDash = false;
							MessageBox.Show(LocalizationStrings.Program_ToggleSteamVR_KillOculusDash_NotExist_Error);
							break;
					}
				}
				else //? SteamVR enable/disable mode
				{
					if (_SteamVr.IsBroken)
						_SteamVr.Restore();
					else
						_SteamVr.Break();
				}
			}
			catch (Exception e)
			{
				e.ShowMessageBox();
			}

			notifyIcon1.Icon = GetIcon();
		}

		private static void KillSteamVR()
		{
			try
				{ SteamVR.KillProcess(); }
			catch (Exception e) 
				{ e.ShowMessageBox(); }
		}

		public static void SetAutorunValue(bool Autorun)
		{
			string exePath = Application.ExecutablePath;
			var reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

			try
			{
				// ReSharper disable twice PossibleNullReferenceException
				if (Autorun) reg.SetValue(Application.ProductName, $"\"{exePath}\"");
				else reg.DeleteValue(Application.ProductName);

				reg.Close();
			}
			catch (Exception e)
			{
				e.ShowMessageBox(LocalizationStrings.MenuOptions_SetAutorunValue_ErrorText + $" (→ {Autorun})");
			}
		}

		public static bool IsAutorun() => Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\")?.GetValue(Application.ProductName) is not null;

		public static Icon GetIcon()
		{
			string iconTheme = Settings.Default.BlackMode ? "Black" : "White";
			string iconType;
			if (Settings.Default.KillOculusDash && OculusDash.State is not NotExist) { //: if KillOculusDash enabled, it's SteamVR ↔ Oculus mode
				iconType = OculusDash.State == Killed ? "SteamVR" : "Oculus";
			} else {
				iconType = _SteamVr.IsBroken ? "Fix SteamVR" : "Break SteamVR";
			}
			return Icon.ExtractAssociatedIcon($@"icons\{iconTheme}\{iconType}.ico");
		}


		/// <summary>
		/// Get image from path $@"images\{Path}\{_IconsRealismLevel}\{imageName}.png", lowering _IconsRealismLevel until it exist
		/// </summary>
		/// <param name="Path"></param>
		/// <param name="imageName"></param>
		/// <returns></returns>
		public static Image GetImage(string Path, string imageName)
		{
			for (int i = _IconsRealismLevel; i >= 0; i--)
			{
				string path = $@"images\{Path}\{i}\{imageName}.png";
				if (File.Exists(path)) return Image.FromFile(path);
			}

			return null;

		}

		public static Image GetImage(string Path, string imageName, string RealismLevel)
		{
			
				string path = $@"images\{Path}\{RealismLevel}\{imageName}.png";
				return File.Exists(path) ? Image.FromFile(path) : null;
		}

		/// <summary>
		/// Get image from path $@"images\{Path}\{imageName}.png"; ignoring _IconsRealismLevel
		/// </summary>
		/// <param name="Path"></param>
		/// <param name="imageName"></param>
		/// <returns></returns>
		public static Bitmap GetBitmap(string Path, string imageName)
		{
			for (int i = _IconsRealismLevel; i > 0; i--)
			{
				string path = $@"images\{Path}\{imageName}.png";
				if (File.Exists(path)) return (Bitmap)Image.FromFile(path);
			}

			return null;

		}
	}
}