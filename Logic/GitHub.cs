using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Octokit;
using Octokit.Internal;
using FileMode = System.IO.FileMode;

namespace Titanium;

public static class GitHub //TODO: Update it with new version of my Github library
{
	public enum Status
	{
		Downloaded,
		Updated,
		NoAction
	}
		
	public static string ProxyAddress = "auto";

	public class UpdateResult
	{
		public Status Status;
		public string? ReleaseDiscription { get; private set; }
		public string? ReleaseName { get; private set; }
		public Version? Version { get; private set; }

		public UpdateResult(Status Status = Status.NoAction, Version? Version = null, string? ReleaseName = null, string? ReleaseDiscription = null)
		{
			this.Status = Status;
			this.ReleaseDiscription = ReleaseDiscription;
			this.ReleaseName = ReleaseName;
			this.Version = Version;
		}

		public UpdateResult Change(Status? Status = null, Version? Version = null, string? ReleaseName = null, string? ReleaseDiscription = null)
		{
			if (Status!=null) this.Status = (Status)Status;
			if (ReleaseDiscription!=null) this.ReleaseDiscription = ReleaseDiscription;
			if (ReleaseName!=null) this.ReleaseName = ReleaseName;
			if (Version!=null) this.Version = Version;
			return this;
		}
	}

		
	public enum UpdateMode
	{
		/// <summary> Download programm if it's not exist, but don't update it if it's exist </summary>
		Download,
		/// <summary> Only checks a new version, but not update it. </summary>
		Check,
		/// <summary> Update programm only if it's not installed or older version is installed </summary>
		Update,
		/// <summary> Update programm even if the newer version is installed </summary>
		Replace
	}

	/// <summary>
	/// Updates the exe in specified paths from GitHub releases page
	/// </summary>
	/// <param name="repositoryLink">GitHub repository link from where updates will be downloaded. In any format from "https://github.com/TuTAH1/xml-js-Parser/releases/tag/1.2.0" to just "TuTAH1/xml-js-Parser" (both variants will give the same result)</param>
	/// <param name="ProgramExePath">Path of physical exe file that should be updated</param>
	/// <param name="Unpack">Should archives be unpacked while placing in </param>
	/// <param name="GitHubFilenameRegex">regex of the filename of the release</param>
	/// <param name="TempFolder">Leave GitHub release files in ./Temp. Don't Forget to DELETE TEMP folder after performing needed operations</param>
	/// <param name="ArchiveIgnoreFileList">List of files that shouldn't extracted from downloaded archive. If null, all files will be extracted</param>
	/// <param name="ReverseArchiveFileList">Turns Blacklist into whitelist if true</param>
	/// <param name="AskUpdatestion">Function that will be executed if update found. If this function will return false, update will be canceled</param>
	/// <returns></returns>
	public static async Task<UpdateResult> checkSoftwareUpdatesByLink(UpdateMode UpdateMode, string repositoryLink, string ProgramExePath, string DownloadPath = "Temp", bool ClearDownloadFolder = false, bool Unpack = true, Regex? GitHubFilenameRegex = null, bool ReverseGithubFilenameRegex = false, bool TempFolder = false, Regex[] ArchiveIgnoreFileList = null, bool ReverseArchiveFileList = false, bool KillRelatedProcesses = false, Func<UpdateResult,bool> AskUpdate = default)
	{
		string[] ss = repositoryLink.RemoveFrom(TypesFuncs.Side.Start, "https://", "github.com/").Split("/");
		if (ss.Length < 2) throw new ArgumentException("Can't get username and repName from " + repositoryLink);
		return await checkSoftwareUpdates(UpdateMode, ss[0], ss[1], ProgramExePath, DownloadPath,ClearDownloadFolder,   Unpack: Unpack, GitHubFilenameRegex: GitHubFilenameRegex,ReverseGithubFilenameRegex: ReverseGithubFilenameRegex, TempFolder: TempFolder, ArchiveIgnoreFileList: ArchiveIgnoreFileList, ReverseArchiveFileList: ReverseArchiveFileList, KillRelatedProcesses: KillRelatedProcesses, AskUpdate: AskUpdate).ConfigureAwait(false);
	}

	/// <summary>
	/// Updates the exe in specified paths from GitHub releases page
	/// </summary>
	/// ///
	/// <param name="UpdateMode"></param>
	/// <param name="author">Repository author id (example: TuTAH1)</param>
	/// <param name="repName">Repository name (example: SteamVR-OculusDash-Switcher)</param>
	/// <param name="ProgramExePath">Path of physical exe file that should be updated</param>
	/// <param name="DownloadPath"></param>
	/// <param name="ClearDownloadFolder"></param>
	/// <param name="Unpack">Should archives be unpacked while placing in </param>
	/// <param name="GitHubFilenameRegex">regex of the filename of the release</param>
	/// <param name="ReverseGithubFilenameRegex"></param>
	/// <param name="TempFolder">Leave GitHub release files in ./Temp. Don't Forget to DELETE TEMP folder after performing needed operations</param>
	/// <param name="ArchiveIgnoreFileList">List of files that shouldn't extracted from downloaded archive. If null, all files will be extracted</param>
	/// <param name="ReverseArchiveFileList">Turns Blacklist into whitelist if true</param>
	/// <param name="KillRelatedProcesses"></param>
	/// <param name="AskUpdate">Function that will be executed if update found. If this function will return false, update will be canceled</param>
	/// <returns></returns>
	public static async Task<UpdateResult> checkSoftwareUpdates(UpdateMode UpdateMode, string author, string repName, string ProgramExePath, string DownloadPath = "Temp", bool ClearDownloadFolder = false, bool Unpack = true, Regex GitHubFilenameRegex = null, bool ReverseGithubFilenameRegex = false, bool TempFolder = false, Regex[] ArchiveIgnoreFileList = null, bool ReverseArchiveFileList = false, bool? KillRelatedProcesses = false, Func<UpdateResult, bool> AskUpdate = default)

	{
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		KillRelatedProcesses ??= !TempFolder;

		//string registyProxyAddress = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings";
		//string? proxyConfigScriptLink = Registry.GetValue(registyProxyAddress,"AutoConfigURL", null)?.ToString();
		//bool proxyEnabled = Registry.GetValue(registyProxyAddress, "ProxyEnable",null)?.ToString() == 1.ToString() || !proxyConfigScriptLink.IsNullOrEmpty();

		HttpClientHandler clientHandler = new HttpClientHandler();
		clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
		var webProxy = new WebProxy();

		clientHandler.Proxy = ProxyAddress switch
		{
			"auto" => (HttpClient.DefaultProxy)
			,"no" => null
			,_=> new WebProxy(new Uri(ProxyAddress))
		};

		
		var connection = new Connection(
			new ProductHeaderValue("Titanium-GithubSoftwareUpdater"),
			new HttpClientAdapter(() => clientHandler));

		var github = new GitHubClient(connection);
		var release = await github.Repository.Release.GetLatest(author, repName).ConfigureAwait(false);
		Version? relVersion = null;
		try
		{
			relVersion = Version.Parse(new Regex("[^.0-9]").Replace(release.TagName, ""));
		} catch (Exception) { }
		UpdateResult result = new(Status.NoAction, relVersion, release.Name, release.Body);
			
		bool fileExist = File.Exists(ProgramExePath);

		if (UpdateMode!= UpdateMode.Check && !fileExist || UpdateMode == UpdateMode.Replace)
		{
			await DownloadLastest().ConfigureAwait(false);
			return result.Change(Status.Downloaded);
		}
		else if (UpdateMode is UpdateMode.Update or UpdateMode.Check)
		{
			var currentVersion = FileVersionInfo.GetVersionInfo(ProgramExePath);
			if (currentVersion is null) 
				throw new InvalidOperationException("Product version field is empty");

			//:If current file's version is higher than in github, don't do anything
			if (UpdateMode == UpdateMode.Check || relVersion <= Version.Parse(currentVersion.ProductVersion!)) return result;

			if (AskUpdate == default || !AskUpdate(result)) 
				return result;
			await DownloadLastest().ConfigureAwait(false);
			return result.Change(Status.Updated);
		}
		return result;

		async Task<bool> DownloadLastest()
		{

			var gitHubFiles = release.Assets;

			if (!gitHubFiles.Any()) throw new ArgumentNullException(nameof(gitHubFiles), "No any files found in the release");

			gitHubFiles = (
				from file in gitHubFiles 
				where (GitHubFilenameRegex?.IsMatch(file.Name) ^ ReverseGithubFilenameRegex ??  true) //: Select all files aliased with GitHubFilename regex 
				select file).ToList();

			if (!gitHubFiles.Any()) throw new ArgumentNullException(nameof(gitHubFiles),GitHubFilenameRegex==null? "No files found in the release" : $"No files matching \"{GitHubFilenameRegex}\" found in the release");

			foreach (var file in gitHubFiles)
			{
				string filepath = $"{DownloadPath}\\{file.Name}";

				using var client = new HttpClient();
				var s = await client.GetStreamAsync(file.BrowserDownloadUrl).ConfigureAwait(false);
				if(ClearDownloadFolder)
					try {IO.RemoveAll(DownloadPath, false); } catch (Exception) {}
				Directory.CreateDirectory(filepath.Slice(0,"\\"));
				var fs = new FileStream(filepath, FileMode.OpenOrCreate); //TODO: Заменить Temp на DownloadPath
				s.CopyTo(fs); //TODO: may be done async
				fs.Close();
				s.Close();
					
				Unpack = Unpack && new FileInfo(filepath).Extension == ".zip";
				if (Unpack)
				{
					if ((bool)KillRelatedProcesses)
					{
						var archive = new ZipFile(fs.Name);
						foreach (ZipEntry entry in archive)
						{
							var entryPath = (TempFolder ? "Temp\\" : "") + entry.Name;
							var entryName = entryPath.Slice("\\", LastStart: true);

							if ((bool)KillRelatedProcesses && entryName.EndsWith(".exe"))
								TypesFuncs.KillProcesses(Path: AppContext.BaseDirectory + entryName, Name: entryName);
						}

						archive.Close();
					}

					ZipStrings.CodePage = 866;
					new FastZip { EntryFactory = new ZipEntryFactory { IsUnicodeText = true } }.ExtractZip(filepath, (TempFolder? "Temp\\" : ""), null);
					File.Delete(filepath);
				}
				else
				{
					File.Move(filepath, filepath.RemoveFrom(TypesFuncs.Side.Start, "Temp\\"), true);
				}
			}
				

			//new WebClient().DownloadFile(releasesPage, "OculusDash.exe");
			return true;
		}

			
	}

	class GitHubFile
	{
		public string Name;
		public string Link;
		public Classes.FileSize? Size;
		public DateTime? Date;

		public GitHubFile(string Name, string Link, string Size, string Date)
		{
			this.Name = Name;
			this.Link = Link;
			this.Size = Classes.FileSize.Get(Size);
			try 
			{ this.Date = Convert.ToDateTime(Date); }
			catch (Exception) 
			{ this.Date = null; }
				
		}
	}
}