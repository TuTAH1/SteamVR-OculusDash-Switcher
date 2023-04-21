using System;
using System.IO;
using System.Net; //для работы с интернетом

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using static System.Console;

namespace DataCollector
{
	public class Internet
	{
		public static string getResponseString(string urlAddress)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
			request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:85.0) Gecko/20100101 Firefox/85.0");
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			if (response.StatusCode == HttpStatusCode.OK)
			{
				Stream receiveStream = response.GetResponseStream();
				StreamReader readStream = null;

				if (response.CharacterSet == null)
				{
					readStream = new StreamReader(receiveStream);
				}
				else
				{
					readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
				}

				string data = readStream.ReadToEnd();

				receiveStream.Close();
				response.Close();
				readStream.Close();

				return data;
			}

			throw new Exception("bad url");
		}

		public static async Task<IDocument> getResponseAsync(string urlAddress)
		{
			var requester = new DefaultHttpRequester("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:99.0) Gecko/20100101 Firefox/99.0");
			var config = new Configuration().WithDefaultLoader().With(requester);
			var document = await BrowsingContext.New(config).OpenAsync(urlAddress);
			return document;
		}

		/*
		public static async Task<IDocument> getResponseAS(string urlAddress)
		{
			var requester = new DefaultHttpRequester("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");
			var config = Configuration.Default.WithDefaultLoader().With(requester);
			var document = await BrowsingContext.New(config).OpenAsync(urlAddress);
			return document;
		}*/

		/*static void getHTMLfile(string urlAdress, string filename)
		{
			File.WriteAllText(
				filename,
				getResponse(urlAdress),
				System.Text.Encoding.UTF8
			);
		}*/
	}


	#region Garbage
	//public static string getResponse(string urlAddress)
	//{
	//	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
	//	request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:85.0) Gecko/20100101 Firefox/85.0");
	//	HttpWebResponse response = (HttpWebResponse)request.GetResponse();

	//	if (response.StatusCode == HttpStatusCode.OK)
	//	{
	//		Stream receiveStream = response.GetResponseStream();
	//		StreamReader readStream = null;

	//		if (response.CharacterSet == null)
	//		{
	//			readStream = new StreamReader(receiveStream);
	//		}
	//		else
	//		{
	//			readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
	//		}

	//		string data = readStream.ReadToEnd();

	//		receiveStream.Close();
	//		response.Close();
	//		readStream.Close();
	//		return data;
	//	}

	//	throw new Exception("bad url");
	//}

	//public static void getHWlinks(string urlAddress, string HWname)
	//{
	//	using (IWebDriver driver = new FirefoxDriver())
	//	{
	//		IReadOnlyCollection<IWebElement> HWs;
	//		driver.Navigate().GoToUrl(urlAddress);
	//		driver.Manage().Timeouts().ImplicitWait = new TimeSpan(30);
	//		driver.Manage().Timeouts().AsynchronousJavaScript = new TimeSpan(30);
	//		ForegroundColor = ConsoleColor.Gray;
	//		Clear();
	//		WindowWidth = 110;
	//		ForegroundColor = ConsoleColor.Yellow; WriteLine($"Идёт получение ссылок {HWname} \n");
	//		short added = 0;
	//		CursorTop = 3;
	//		CursorLeft = 0;
	//		ForegroundColor = ConsoleColor.Green; Write("Всего добавлено:");
	//		try
	//		{
	//			while (true) driver.FindElement(By.ClassName("pull-right")).Click();
	//		}
	//		catch { }
	//		driver.Manage().Timeouts().ImplicitWait = new TimeSpan(1);
	//		driver.Manage().Timeouts().AsynchronousJavaScript = new TimeSpan(1);
	//		while (true)
	//		{
	//			CursorTop = 2;
	//			CursorLeft = 0;
	//			ForegroundColor = ConsoleColor.Cyan; Write("[                                                                                                    ]");
	//			//driver.FindElement(By.CssSelector(
	//			//		"a[onclick=\"window.location.href=updateQueryStringParameter(window.location.href,'sort','Newest');\"]"))
	//			//	.Click();
	//			CursorTop = 1;
	//			CursorLeft = 0;
	//			ForegroundColor = ConsoleColor.DarkCyan; Write(driver.FindElement(By.CssSelector("ul[class=\"pagination pagination-lg\"]>li[class=\"disabled\"]>a")).Text);

	//			HWs = driver.FindElements(By.ClassName("tl-tag"));

	//			CursorLeft = 1;
	//			CursorTop = 2;
	//			foreach (var HW in HWs)
	//			{
	//				if (Titanium.String.CountDigitsDouble(HW
	//					.FindElement(By.XPath("//*[contains(text(), 'User benchmarks')]")).Text) > 2)
	//				{
	//					File.AppendAllText(HWname + ".txt", HW.GetAttribute("href"), Encoding.UTF8);
	//					int c = CursorLeft;
	//					CursorLeft = 11;
	//					CursorTop = 3;
	//					Write(++added);
	//					CursorTop = 2;
	//					CursorLeft = c;
	//				}

	//				ForegroundColor = ConsoleColor.Cyan; Write("-");
	//			}
	//			Write("]\n");

	//			var nextButton = driver.FindElement(By.Id("searchForm:j_idt80"));
	//			if (nextButton.Enabled) nextButton.Click();
	//			else break;
	//		}

	//	}
	//}

	//public static string getHWHTML(string urlAddress, string HWname)
	//{
	//	using (IWebDriver driver = new FirefoxDriver())
	//	{
	//		IReadOnlyCollection<IWebElement> HWs;
	//		driver.Navigate().GoToUrl(urlAddress);
	//		try
	//		{
	//			while (true) driver.FindElement(By.ClassName("pull-right")).Click();
	//		}
	//		catch { }

	//		for (int i = 0; ; i++)
	//		{
	//			//File.AppendAllText(HWname+i+".Html",
	//			//driver.PageSource);
	//			IBrowsingContext browsing = new BrowsingContext();
	//			browsing.OpenAsync(driver.PageSource);

	//			var nextButton = driver.FindElement(By.Id("searchForm:j_idt80"));
	//			if (nextButton.Enabled) nextButton.Click();
	//		}

	//	}
	//}
	#endregion
}
