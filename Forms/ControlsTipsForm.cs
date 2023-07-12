using System.Drawing;
using System.Windows.Forms;
using SteamVR_OculusDash_Switcher.Properties.Localization;
using Titanium;
using static SteamVR_OculusDash_Switcher.Program;
using static Titanium.Forms;

namespace SteamVR_OculusDash_Switcher.Forms
{
    public partial class ControlsTipsForm : Form
	{
		public ControlsTipsForm()
		{
			InitializeComponent();
			Size tipsPadding = new Size(12, 12);
			var Tips = new PictureTextCollection((Point)tipsPadding, 40, 3, 10,mainPanel);
			Tips.AddRange(
				new []{GetBitmap("Control Tips","RMB"),
					GetBitmap("Control Tips","LMB"),
					GetBitmap("Control Tips","MMB")},

				new []{LocalizationStrings.ControlsTipsForm_tip__RMB,
					LocalizationStrings.ControlsTipsForm_tip__LMB,
					LocalizationStrings.ControlsTipsForm_tip__MMB}
				);
			Size = Tips.Size + tipsPadding*2 + new Size(0, this.GetHeaderHeight());
			this.Text = LocalizationStrings.ControlsTipsForm__TitleText;
		}
	}
}
