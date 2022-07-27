using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using InfoBox;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Titanium;
using Icon = System.Drawing.Icon;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Resources;
using System.Threading;
using AngleSharp.Dom;
using DataCollector;
using SteamVR_OculusDash_Switcher.Forms;
using SteamVR_OculusDash_Switcher.Logic;
using SteamVR_OculusDash_Switcher.Properties;
using SteamVR_OculusDash_Switcher.Properties.Localization;
using TitaniumComparator.LogicClasses;

namespace SteamVR_OculusDash_Switcher
{
	static partial class Program
	{
		public static NotifyIcon notifyIcon1 = null;
		public static SteamVR _SteamVr;
		public static bool _isOculusExist;
		public static Mode CurrentMode = Mode.Oculus;
		public static Settings __DefaultSettings = Settings.Default;
		public enum Mode
		{
			SteamVR,
			Oculus
		}
			
			/*_SteamVrMethod.Method == SteamVR.BreakMethod.None? _SteamVrMethod.IsMethodApplied() : _SteamVrMethod.IsMethodApplied() switch
		{
			true => false,
			false => true,
			null => throw new ArgumentNullException("")
		};*/
		public static int _IconsRealismLevel = __DefaultSettings.RealismLevel;
		public static int _MaxIconsRealismLevel = 3;

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(true);
			if (_IconsRealismLevel <0 || _IconsRealismLevel > _MaxIconsRealismLevel) _IconsRealismLevel = 1;

			bool oculusBroken = false;
			try
			{
				_SteamVr = new SteamVR(__DefaultSettings.SteamVRDisablingMethod);
				__DefaultSettings.SteamVRDisablingMethod = _SteamVr.Method;
			}
			catch (Exception e)
			{
				e.ShowMessageBox();
				/*var dialog = InformationBox.Show(LocalizationStrings.MessageBox_text__SteamVR_not_found, title: LocalizationStrings.MessageBox_Title__Error, icon: InformationBoxIcon.Error, buttons: InformationBoxButtons.User1User2, customButtons: new[] { LocalizationStrings.Button__Exit, LocalizationStrings.Button__Continue });
				switch (dialog)
				{
					case InformationBoxResult.User1: return;
					case InformationBoxResult.User2: break;
				}*/
			}

			_isOculusExist = OculusDash.IsOculusExist;
			if (_isOculusExist)
			{
				try
				{
					bool? OculusDashKilled = OculusDash.IsOculusDashKilled();
					if (OculusDashKilled is null)
					{
						MessageBox.Show("Something wrong happened and OculusDash become unrecoverable broken 😭", "Something go wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
						_isOculusExist = false;
					}
					else
					{
						oculusBroken = OculusDashKilled is true;
					}
				}
				catch (Exception e)
				{
					e.ShowMessageBox();
				}
			}
			else
			{
				__DefaultSettings.KillOculusDash = false;
			}

			if (!_isOculusExist && _SteamVr.IsBroken && oculusBroken)
			{
				switch (InformationBox.Show("Both SteamVR and OculusDash properly broken. What to restore?", "Choosing a side", buttons: InformationBoxButtons.User1User2, icon: InformationBoxIcon.Error, customButtons: new[] { "SteamVR", "Oculus" }))
				{
					case InformationBoxResult.User1:
						_SteamVr.Restore();
						break;

					case InformationBoxResult.User2:
						OculusDash.Restore();
						oculusBroken = false;
						break;
				}
			}

			CurrentMode = oculusBroken != _SteamVr.IsBroken ? oculusBroken ? Mode.SteamVR : Mode.Oculus : Mode.Oculus;

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

					case MouseButtons.Left: if (__DefaultSettings.OneClickMode) ToggleSteamVR_Click(null,null); break;

					case MouseButtons.Middle: if(__DefaultSettings.OneClickMode) KillSteamVR(); break;
				}
				
			};
			notifyIcon1.Icon = GetIcon();
			new SettingsForm().Show(); //!Debug
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
				item.Checked = __DefaultSettings.OneClickMode;
				item.CheckOnClick = true;
				item.Click += delegate
				{
					__DefaultSettings.OneClickMode = item.Checked;
					__DefaultSettings.Save();
				};
				items.Add(item);
			}

			//: Control Tips
			if(__DefaultSettings.OneClickMode)
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
			if (SteamVR.IsActive() && __DefaultSettings.KillSteamVR_Enabled)
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
				item.Text = (_SteamVr.IsBroken ? LocalizationStrings.MenuOptions__Restore_SteamVR :LocalizationStrings.MenuOptions__Break_SteamVR);
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

		private static void ToggleSteamVR_Click(object sender, EventArgs args)
		{
			if (_SteamVr.IsBroken) RestoreSteamVR();
			else BreakSteamVR();
		}

		private static void KillSteamVR()
		{
			SteamVR.KillProcess();
		}

		private static void BreakSteamVR()
		{
			_SteamVr.Break();
			if (__DefaultSettings.KillOculusDash)
			{
				OculusDash.Restore();
			}
			notifyIcon1.Icon = Icon.ExtractAssociatedIcon("icons/Fix SteamVR.ico");
		}

		private static void RestoreSteamVR()
		{
			_SteamVr.Restore();
			if (__DefaultSettings.KillOculusDash)
			{
				OculusDash.Break();
			}
			notifyIcon1.Icon = Icon.ExtractAssociatedIcon("icons/Break SteamVR.ico");
		}

		public static void SetAutorunValue(bool Autorun)
		{
			string exePath = Application.ExecutablePath;
			var reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

			try
			{
				// ReSharper disable twice PossibleNullReferenceException
				if (Autorun) reg.SetValue(Application.ProductName, exePath);
				else reg.DeleteValue(Application.ProductName);
	
				reg.Close();
			}
			catch {}
		}

		public static bool IsAutorun() => Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\")?.GetValue(Application.ProductName) is not null;

		public static Icon GetIcon() =>
			Icon.ExtractAssociatedIcon($@"icons\{(__DefaultSettings.BlackMode ? "Black" : "White")}\{(CurrentMode is Mode.SteamVR? "SteamVR" : "Oculus")}.ico");
		

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