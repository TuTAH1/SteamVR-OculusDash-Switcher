using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamVR_OculusDash_Switcher.Logic
{
	public class Language
	{
		public CultureInfo Culture;

		public override string ToString()
		{
			return Culture.NativeName;
		}

		public Language(CultureInfo Culture)
		{
			this.Culture = Culture;
		}
	}

	public static class LanguageFunctions
	{
		public static Language GetCurrentLanguage(this Language[] Languages)
		{
			foreach (var lang in Languages)
			{
				if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == lang.Culture.TwoLetterISOLanguageName) return lang;
			}

			return null;
		}

		public static int? GetCurrentLanguageIndex(this Language[] Languages)
		{
			for (int i = 0; i < Languages.Length; i++)
			{
				if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == Languages[i].Culture.TwoLetterISOLanguageName) return i;
			}

			return null;
		}
	}
}
