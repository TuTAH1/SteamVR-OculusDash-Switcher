using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using DataCollector;
using Microsoft.Win32;
using Titanium;

namespace SteamVR_OculusDash_Switcher.Logic
{
    public static class OculusDash
    {
        private static readonly string _oculusFolderPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Oculus")?.GetValue("InstallLocation")?.ToString()?? (Directory.Exists( @"C:\Program Files\Oculus\")?  @"C:\Program Files\Oculus\" : null);
		/// <summary>OculusDash.exe path</summary>
		private static readonly string _oculusDashExePath = _oculusFolderPath?.Add("\\" + @"Support\oculus-dash\dash\bin\OculusDash.exe");
		///<summary>Oculus killere exe's location inside of this program's folder</summary>
		const string _innerOculusKillerPath = @"OculusDashKiller\OculusDash.exe";
		private static bool OculusKillerChecked = false;
		public static bool IsOculusKillerExist => File.Exists(_innerOculusKillerPath);
		public static bool IsOculusExist => _oculusFolderPath != null;

		public enum Status
		{
			Downloaded,
			Updated,
			NoAction
		}

		//TODO: Make it async
		//:Checks if OculusKiller exe exist and downloads the lastest release if not
		private static Status checkOculusKiller(bool checkUpdates)
		{
			const string oculusDashDownloadLink = @"https://github.com/ItsKaitlyn03/OculusKiller/releases/latest/download/OculusDash.exe";
			
			if (!IsOculusKillerExist)
			{
				DownloadLastestOculusKiller();
				return Status.Downloaded;
			}
			else if (checkUpdates)
			{
				var doc = Internet.getResponseAsync(@"https://github.com/ItsKaitlyn03/OculusKiller/releases").Result;
				var lastestVersion = doc.QuerySelector(".Link--primary")?.Text();
				if (lastestVersion is null) 
					throw new ArgumentNullException(nameof(lastestVersion), "Can't get lastest version");

				var currentVersion = FileVersionInfo.GetVersionInfo(_innerOculusKillerPath);
				if (currentVersion is null) 
					throw new InvalidOperationException("Product version field is empty");

				//:If current file's version is lower than in github, download lastest from github
				if (Version.Parse(lastestVersion) > Version.Parse(currentVersion.ProductVersion))
				{
					DownloadLastestOculusKiller();
					return Status.Updated;
				}
			}
			return Status.NoAction;

			void DownloadLastestOculusKiller()
			{
				using (var client = new HttpClient())
				{
					using var s = client.GetStreamAsync(oculusDashDownloadLink);
					using var fs = new FileStream(_innerOculusKillerPath, FileMode.OpenOrCreate);
					s.Result.CopyTo(fs);
				}

				new WebClient().DownloadFile(oculusDashDownloadLink, "OculusDash.exe");
			}
		}

		/// <summary>
		/// Checks if OculusKiller exe exist and otherwise downloads the lastest release (also tries to update, if <param name="checkUpdates"/> is true)
		/// </summary>
		public static Status CheckKiller(bool checkUpdates = false)
		{
			var status = checkOculusKiller(checkUpdates);
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
				return (new FileInfo(_oculusDashExePath).Length <= (IsOculusKillerExist? new FileInfo(_innerOculusKillerPath).Length : 1000))? //TODO: Add IsOculusExist check
						File.Exists(_oculusDashExePath)? true : null
						: false;
			}
			catch (Exception e)
			{
				throw;
				//e.ShowMessageBox();
			}
		}

		public static void Break()
		{
			if (!OculusKillerChecked) CheckKiller();
			if(IsOculusDashKilled()!= false) return;

			File.Move(_oculusDashExePath, _oculusDashExePath + "_", true); //: Backup
			File.Move(_innerOculusKillerPath, _oculusDashExePath,true); //: Replace with Oculus Killer
		}

		public static void Restore()
		{
			if(File.Exists(_oculusDashExePath+"_")) File.Move(_oculusDashExePath+"_",_oculusDashExePath, true);
		}
    }
}
