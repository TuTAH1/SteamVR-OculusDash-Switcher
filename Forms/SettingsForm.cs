using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteamVR_OculusDash_Switcher.Logic;
using SteamVR_OculusDash_Switcher.Properties;
using SteamVR_OculusDash_Switcher.Properties.Localization;
using Titanium;
using static SteamVR_OculusDash_Switcher.Program;
using static Titanium.Forms;
using WMPLib;

namespace SteamVR_OculusDash_Switcher
{
    public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();
			InitializeControls();
		}

		public void InitializeControls()
		{
			Text = LocalizationStrings.SettingsForm_Title;
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);

			gbUpdate.Visible = false; //: Autoupdate not ready yet

			#region Functions group

			lbSteamVRDisableMethod.Text = LocalizationStrings.SettingsForm_lbSteamVRDisableMethod;
			lbSteamVRDisableMethod.Left = lbSteamVRDisableMethod.Margin.Left;
			comboSteamVRDisableMethod.DropDownStyle = ComboBoxStyle.DropDownList;
			comboSteamVRDisableMethod.DrawMode = DrawMode.OwnerDrawFixed;
			var SteamVRDisableMethods = SteamVRMethod.GetAll();
			comboSteamVRDisableMethod.ResetText();
			//comboSteamVRDisableMethod.SelectedIndex = -1;
			//comboSteamVRDisableMethod.SelectedItem = null;
			comboSteamVRDisableMethod.Items.Clear();
			comboSteamVRDisableMethod.Items.AddRange(SteamVRDisableMethods);
			comboSteamVRDisableMethod.SelectedItem = _SteamVr;
			Settings.Default.SteamVRDisablingMethod = _SteamVr.Method;
			Settings.Default.Save();

			cbKillOculus.Enabled = OculusDash.State is not OculusDash.DashState.NotExist;
			cbKillOculus.Text = LocalizationStrings.SettingsForm_cbKillOculus;
			cbKillOculus.Checked = Settings.Default.KillOculusDash;

			btnCheckOculusKillerUpdates.Text = LocalizationStrings.SettingsForm_btnCheckOculusKillerUpdates;

			//gbFunctions.Width = new[] { lbSteamVRDisableMethod.Width, comboSteamVRDisableMethod.Width, cbKillOculus.Width, btnCheckOculusKillerUpdates.Width }.Max();

			#endregion

			#region Interface group

			
			gbInterface.Left = Math.Max(gbFunctions.Right + gbFunctions.Margin.Right, gbUpdate.Right + gbUpdate.Margin.Right) + gbInterface.Margin.Left; //: gbInterface Left position

			picLanguage.Image = GetImage("Settings", "language");
			lbLanguage.Text = LocalizationStrings.SettingsForm_lbLanguage;
			lbLanguage.Left = GetPositionRightTo(picLanguage);

			var Languages = new[]
			{
				new Language(new CultureInfo("en")),
				new Language(new CultureInfo("ru"))
			};
			comboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
			comboLanguage.SelectedItem = null;
			comboLanguage.Items.Clear();
			comboLanguage.Items.AddRange(Languages);
			comboLanguage.SelectedIndex = Languages.GetCurrentLanguageIndex() ?? 0;


			lbIconsRealism.Text = LocalizationStrings.SettingsForm_lbIconsRealism;
			slideIconRealism.Value = _IconsRealismLevel;
			slideIconRealism.Maximum = _MaxIconsRealismLevel;
			Change_IconRealism_DiscriptionAndImage();

			cbKillSteamVR.Text = LocalizationStrings.SettingsForm_cb__show_xxx_option.Replace("R3pl@ceMe",LocalizationStrings.MenuOptions__Kill_SteamVR);
			cbKillSteamVR.Checked = Settings.Default.KillSteamVR_Enabled;

			lbIconsRealism_Resize(this, null);

			lbTrayIconColor.Text = LocalizationStrings.SettingsForm_lbTrayIconColor;
			lbTrayIconColorValue.Left = lbTrayIconColor.Left + lbTrayIconColor.Width + lbTrayIconColorValue.Margin.Right;
			SetTrayColorValue();

			#endregion

			btnApply.Text = LocalizationStrings.Button_Apply;
			btnApply.Left = gbInterface.Right - btnApply.Width;
			btnApply.Top = this.GetHeaderHeight() + Math.Max(gbFunctions.Bottom, gbInterface.Bottom) + gbInterface.Margin.Bottom + btnApply.Margin.Top;

			lbSaved.Text = LocalizationStrings.SettingsForm_btnApply_label__Saved;
			lbSaved.Left = btnApply.Left - MeasureText(lbSaved).Width; //- lbSaved.Margin.Right;
			lbSaved.ForeColor = this.BackColor;
			gbFunctions.Text = LocalizationStrings.SettingsForm__Main_functions;
			gbInterface.Text = LocalizationStrings.SettingsForm__Interface;

			//btnApply.Enabled = false;

			//: Form autosize
			/*this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;*/

			Width = gbInterface.Right + gbInterface.Margin.Right + this.GetWindowPadding(Orientation.Horizontal)*3;
			Height = this.GetHeaderHeight() + this.GetWindowPadding(Orientation.Vertical) + btnApply.Bottom + btnApply.Margin.Bottom;

			/*MessageBox.Show($"HeaderWidth = {this.GetHeaderHeight()}\n" +
			                $"gbInterface.Bottom = {gbInterface.Bottom}\n" +
			                $"gbInterface.Margin = {gbInterface.Margin.Bottom}\n" +
			                $"btnApply.Height = {btnApply.Height}\n" +
							$"btn computated height = {btnApply.Bottom-btnApply.Top}\n" +
			                $"btnApply Bounds height = {btnApply.Bounds.Size.Height}\n" +
			                $"btnApply Horisontal Margin = {btnApply.Margin.Top + btnApply.Margin.Bottom}\n" +
			                $"Form Height = {this.Height}");*/
		
		}

		public static int GetPositionRightTo(Control AnchorControl, int Margin = 3) => AnchorControl.Left + AnchorControl.Width + Margin;


		#region Обработчики событий
		
			//\---------------
			//! SETTINGS APPLY
			//\---------------
			private void btnApply_Click(object sender, EventArgs e)
			{
				try
				{
					bool restartControlBecouseStupidWinformsCantDoSuchBasicThingAsClearCombobox = false; //TODO: Someone plaese say me how to clear combobox and it's work without -1 index exception if it's possible

					//! LANGUAGE CHANGE
					if (!Equals(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, ((Language)comboLanguage.SelectedItem).Culture.TwoLetterISOLanguageName))
					{
						Thread.CurrentThread.CurrentUICulture = ((Language)comboLanguage.SelectedItem).Culture;
						restartControlBecouseStupidWinformsCantDoSuchBasicThingAsClearCombobox = true;
					}

					if (comboSteamVRDisableMethod.SelectedItem!= null && Settings.Default.SteamVRDisablingMethod != (SteamVRMethod)comboSteamVRDisableMethod.SelectedItem) //: if method changed
					{
						
						/*if (_SteamVr.IsBroken)
						{
							if (MessageBox.Show("SteamVR needs to be restored before changing method. Do it now?", "Do you realy read this captions?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
								_SteamVr.Restore();
							else 
								comboSteamVRDisableMethod.SelectedItem = _SteamVr.Method;
						}*/
						Settings.Default.SteamVRDisablingMethod = _SteamVr.Method = (SteamVRMethod)comboSteamVRDisableMethod.SelectedItem; //: set new method
					}

					Settings.Default.KillOculusDash = cbKillOculus.Checked;

					Settings.Default.Save();
					savedLabelActivation(lbSaved, Color.Green, 1000);

					if (!restartControlBecouseStupidWinformsCantDoSuchBasicThingAsClearCombobox) return;
					new SettingsForm().Show(); this.Close();
				}
				catch (Exception exception)
				{
					exception.ShowMessageBox();
				}
			}

			private async Task savedLabelActivation(Label sender, Color e, int timeMs)
			{
				animationManager.Clear();
				sender.ForeColor = e;

				animationManager.Add(new AnimationSequence(
					new PropertyAnimation("ForeColor", sender, this.BackColor, TimeSpan.FromMilliseconds(timeMs), new ColorAnimator())));
				
			}

			private void cbSteamVRDisableMethod_DrawItem(object sender, DrawItemEventArgs e)
			{
				var comboBox = (ComboBox)sender;
				var item = comboBox.Items[e.Index];
				e.DrawBackground();
				using (SolidBrush br = new SolidBrush(e.ForeColor))
				{
					e.Graphics.DrawString(item.ToString(), e.Font, br, e.Bounds);
				}
 
				if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
				{
					ttCbSteamVR.Show(((SteamVRMethod)item).Description, comboBox, e.Bounds.Right, e.Bounds.Bottom);
				}
				else
				{
					ttCbSteamVR.Hide(comboBox);
				}
				e.DrawFocusRectangle();
			}

			private void cbSteamVRDisableMethod_DropDownClosed(object sender, EventArgs e) => this.ttCbSteamVR.Hide((ComboBox)sender);

			private async void btnCheckOculusKillerUpdates_Click(object sender, EventArgs e)
			{
				btnCheckOculusKillerUpdates.Text = LocalizationStrings.SettingsForm_btnCheckOculusKillerUpdates__Checking_updates;
				btnCheckOculusKillerUpdates.Enabled = false;
				try
				{
					var status = await OculusDash.CheckKillerAsync(true);
					MessageBox.Show(status switch
					{
						GitHub.Status.Downloaded => LocalizationStrings.OculusKiller_StatusDiscription_Downloaded,
						GitHub.Status.Updated => LocalizationStrings.OculusKiller_StatusDiscription_Updated,
						GitHub.Status.NoAction => LocalizationStrings.OculusKiller_StatusDiscription_NoAction
					}, "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
					btnCheckOculusKillerUpdates.Text = LocalizationStrings.SettingsForm_btnCheckOculusKillerUpdates;
					btnCheckOculusKillerUpdates.Enabled = true;
				}
				catch (Exception exception)
				{
					exception.ShowMessageBox();
					btnCheckOculusKillerUpdates.Text = LocalizationStrings.SettingsForm_btnCheckOculusKillerUpdates;
					btnCheckOculusKillerUpdates.Enabled = true;
				}

			}

			private WindowsMediaPlayer Egg = null;
			private void slIconRealism_Scroll(object sender, EventArgs e)
			{
				Change_IconRealism_DiscriptionAndImage();

				if (_IconsRealismLevel == _MaxIconsRealismLevel)
				{
					try
					{
						picRealismLevel.BackgroundImage = GetImage("Settings", "RealismLevelImage", "Max"); //: add meme image left to that slider

						if (!File.Exists(@"images\Settings\Max\Horizon\Heart.mp3"))
							File.Copy(@"images\Settings\Max\Horizon\Heart.png", @"images\Settings\Max\Horizon\Heart.mp3");
						Egg = new WindowsMediaPlayer
						{
							URL = @"images\Settings\Max\Horizon\Heart.mp3"
						};
						Egg.controls.play();//: play BMttH: can you break my heart
					}
					catch (Exception exception)
					{
						Egg?.controls.stop();
						if(File.Exists(@"images\Settings\Max\Horizon\Heart.mp3")) 
							File.Delete(@"images\Settings\Max\Horizon\Heart.mp3");
						Egg?.close();
						Egg = null;
						exception.ShowMessageBox(LocalizationStrings.SettingsForm_RealismLevel_Max_Error);
					}
				}
				else
				{
					if(File.Exists(@"images\Settings\Max\Horizon\Heart.mp3")) 
						File.Delete(@"images\Settings\Max\Horizon\Heart.mp3");

					Egg?.controls.stop();
					Egg?.close();
					Egg = null;
				}
				Settings.Default.Save();
			}

			protected override void OnClosing(CancelEventArgs e)
			{
				if (Egg != null && MessageBox.Show(LocalizationStrings.SettingsForm_EasterEgg_OnClosing_Message__Stop_playing_amazing_music, LocalizationStrings.SettingsForm_EasterEgg_OnClosing_Title__Easter_egg_speaking_with_you, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					Egg.controls.stop();
					if(File.Exists(@"images\Settings\Max\Horizon\Heart.mp3")) 
						File.Delete(@"images\Settings\Max\Horizon\Heart.mp3");
				}

				base.OnClosing(e);
			}
			private void lbTrayIconColorValue_Click(object sender, EventArgs e)
			{
				Settings.Default.BlackMode = !Settings.Default.BlackMode;
				SetTrayColorValue();
				Settings.Default.Save();
			}
			private void EnableApplyButton(object sender, EventArgs e)
			{
				btnApply.Enabled = true;
			}

			private void cbKillSteamVR_CheckedChanged(object sender, EventArgs e)
			{
				Settings.Default.KillSteamVR_Enabled = cbKillSteamVR.Checked;
			}

			private void lbIconsRealism_Resize(object sender, EventArgs e)
			{
				var usedSpaceIconRealism = Math.Max(lbIconsRealism.Margin.Left + lbIconsRealism.Width + lbIconsRealism.Margin.Right, slideIconRealism.Margin.Left + slideIconRealism.Width + slideIconRealism.Margin.Right) + picRealismLevel.Margin.Left;

				picRealismLevel.Left = usedSpaceIconRealism;
				picRealismLevel.Width = picRealismLevel.Image?.Width?? picRealismLevel.Width;
			}
			#endregion

		private void SetTrayColorValue()
		{
			lbTrayIconColorValue.Text = Settings.Default.BlackMode ? LocalizationStrings.SettingsForm_lbTrayIconColorValue_Black + " ►" : "◄ " + LocalizationStrings.SettingsForm_lbTrayIconColorValue_White;
		}

		private void Change_IconRealism_DiscriptionAndImage()
		{
			_IconsRealismLevel = slideIconRealism.Value;

			lbIconsRealismDiscription.Text = _IconsRealismLevel switch
			{
				0 => LocalizationStrings.IconRealismLevel_0_Discription,
				1 => LocalizationStrings.IconRealismLevel_1_Discription,
				2 => LocalizationStrings.IconRealismLevel_2_Discription,
				3 => LocalizationStrings.IconRealismLevel_3_Discription,
				_ => LocalizationStrings.IconRealismLevel_Unknown_Discription
			};

			var image = GetImage("Settings", "RealismLevelImage").Resize(picRealismLevel.Height);
			picRealismLevel.Width = image.Width;
			picRealismLevel.BackgroundImage = image;

		}
	}




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
