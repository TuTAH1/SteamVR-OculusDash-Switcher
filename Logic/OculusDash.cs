using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp.Dom;
using DataCollector;
using Microsoft.Win32;
using SteamVR_OculusDash_Switcher.Properties.Localization;
using Titanium;

namespace SteamVR_OculusDash_Switcher.Logic
{
    public static class OculusDash
    {
        private static readonly string _oculusFolderPath;
		/// <summary>OculusDash.exe path</summary>
		private static readonly string _oculusDashExePath;
		///<summary>Oculus killere exe's location inside of this program's folder</summary>
		const string _innerOculusKillerPath = @"OculusDashKiller\OculusDash.exe";
		private static bool OculusKillerChecked = false;

		static OculusDash()
		{
			_oculusFolderPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Oculus")?.GetValue("InstallLocation")?.ToString()?? (Directory.Exists( @"C:\Program Files\Oculus\")?  @"C:\Program Files\Oculus\" : null); //: Check where Oculus is installed, if not found, set default path

			if (_oculusFolderPath == null)
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
			}

			if (_oculusFolderPath == null)
			{
				using var fbd = new FolderBrowserDialog();
				fbd.Description = LocalizationStrings.OculusDash_OculusNotFound__Select_Oculus_folder_path;
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					string[] files = Directory.GetFiles(fbd.SelectedPath);
				}
			}

			_oculusDashExePath = _oculusFolderPath?.Add("\\" + @"Support\oculus-dash\dash\bin\OculusDash.exe");
		}

		public static bool IsOculusKillerExist => File.Exists(_innerOculusKillerPath);
		public static bool IsOculusExist => _oculusFolderPath != null;
		

		//:Checks if OculusKiller exe exist and downloads the lastest release if not
		private static async Task<GitHub.Status> checkOculusKiller(bool checkUpdates)
		{
			const string oculusDashRepository = @"ItsKaitlyn03/OculusKiller";
			return(await GitHub.checkSoftwareUpdatesByLink(GitHub.UpdateMode.Update, oculusDashRepository,_innerOculusKillerPath,"OculusDashKiller").ConfigureAwait(false)).Status;
		}

		/// <summary>
		/// Checks if OculusKiller exe exist and otherwise downloads the lastest release (also tries to update, if <param name="checkUpdates"/> is true)
		/// </summary>
		public static async Task<GitHub.Status> CheckKiller(bool checkUpdates = false)
		{
			var status = await checkOculusKiller(checkUpdates).ConfigureAwait(false);
			OculusKillerChecked = true; //: Sets true if no exceptions
			return status;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// <para>false, if not killed </para>
		/// <para>true, if killed and restoring is posible (orig file exist)</para>
		/// <para>null, if killed and orig is not exist</para>
		/// </returns>
		public static bool? IsOculusDashKilled()
		{
			try
			{
				var oculusDashExeFileInfo = new FileInfo(_oculusDashExePath);

				if (!oculusDashExeFileInfo.Exists) //: OculusDash.exe exist
				{
					var backup = new FileInfo(_oculusDashExePath + "_");
					if (!backup.Exists) return null; //: OculusDash.exe_ exist
					else return true;
				}
				else if (oculusDashExeFileInfo.Length <= (IsOculusKillerExist ? new FileInfo(_innerOculusKillerPath).Length : 1000)) //: OculusDash.exe is replaced by killer
					return true;
				else
					return false; //: OculusDash.exe is ok
			}
			catch (Exception e)
			{
				throw;
				//e.ShowMessageBox();
			}
		}

		public static void Break()
		{
			if (!OculusKillerChecked) CheckKiller().Wait();
			if(IsOculusDashKilled()!= false) return;
			File.Move(_oculusDashExePath, _oculusDashExePath + "_", true); //: Backup
			File.Copy(_innerOculusKillerPath, _oculusDashExePath,true); //: Replace with Oculus Killer
		}

		public static void Restore()
		{
			if(File.Exists(_oculusDashExePath+"_")) File.Move(_oculusDashExePath+"_",_oculusDashExePath, true);
		}
    }
}
