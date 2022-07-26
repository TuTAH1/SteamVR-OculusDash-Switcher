﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using SteamVR_OculusDash_Switcher.Logic;
using SteamVR_OculusDash_Switcher.Properties;
using SteamVR_OculusDash_Switcher.Properties.Localization;
using Titanium;
using TitaniumComparator.LogicClasses;
using static SteamVR_OculusDash_Switcher.Program;

namespace SteamVR_OculusDash_Switcher
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();
			Text = LocalizationStrings.SettingsForm_Title;

			#region Main functions

			lbSteamVRDisableMethod.Text = LocalizationStrings.SettingsForm_lbSteamVRDisableMethod;
			cbSteamVRDisableMethod.DropDownStyle = ComboBoxStyle.DropDownList;
			cbSteamVRDisableMethod.DrawMode = DrawMode.OwnerDrawFixed;
			var SteamVRDisableMethods = SteamVRMethod.GetAll();
			cbSteamVRDisableMethod.Items.AddRange(SteamVRDisableMethods);
			cbSteamVRDisableMethod.SelectedItem = _SteamVr;
			__DefaultSettings.SteamVRDisablingMethod = _SteamVr.Method;
			__DefaultSettings.Save();

			cbKillOculus.Enabled = _isOculusExist;
			cbKillOculus.Text = LocalizationStrings.SettingsForm_cbKillOculus;
			cbKillOculus.Checked = __DefaultSettings.KillOculusDash;

			btnCheckOculusKillerUpdates.Text = LocalizationStrings.SettingsForm_btnCheckOculusKillerUpdates;

			#endregion

			#region Interface

			picLanguage.Image = GetImage("Settings", "language");
			lbLanguage.Text = LocalizationStrings.SettingsForm_lbLanguage;
			lbLanguage.Left = GetPositionRightTo(picLanguage);

			var Languages = new[]
			{
				new Language(new CultureInfo("en")),
				new Language(new CultureInfo("ru"))
			};
			cbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
			cbLanguage.Items.AddRange(Languages);
			cbLanguage.SelectedIndex = Languages.GetCurrentLanguageIndex() ?? 0;


			lbIconsRealism.Text = LocalizationStrings.SettingsForm_lbIconsRealism;
			slideIconRealism.Value = _IconsRealismLevel;
			slideIconRealism.Maximum = _MaxIconsRealismLevel;
			Change_IconRealism_DiscriptionAndImage();

			cbKillSteamVR.Text = LocalizationStrings.SettingsForm_cb__show_xxx_option.Replace("R3pl@ceMe",LocalizationStrings.MenuOptions__Kill_SteamVR);
			cbKillSteamVR.Checked = __DefaultSettings.KillSteamVR_Enabled;

			var usedSpaceIconRealism = Math.Max(lbIconsRealism.Padding.Left + lbIconsRealism.Width + lbIconsRealism.Padding.Right, slideIconRealism.Padding.Left + slideIconRealism.Width + slideIconRealism.Padding.Right) + panelRealismLevel.Padding.Left;

			panelRealismLevel.Left = gbInterface.Left + usedSpaceIconRealism;
			panelRealismLevel.Width = gbInterface.Width - (usedSpaceIconRealism + panelRealismLevel.Padding.Right);

			lbTrayIconColor.Text = LocalizationStrings.SettingsForm_lbTrayIconColor;
			lbTrayIconColorValue.Left = lbTrayIconColor.Left + lbTrayIconColor.Width + lbTrayIconColorValue.Padding.Right;
			SetTrayColorValue();

			#endregion

			btnApply.Text = LocalizationStrings.Button_Apply;
			//btnApply.Enabled = false;
		}

		public static int GetPositionRightTo(Control AnchorControl, int Margin = 3) => AnchorControl.Left + AnchorControl.Width + Margin;


		#region Обработчики событий
		
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

			private void btnCheckOculusKillerUpdates_Click(object sender, EventArgs e)
			{
				try
				{
					var status = OculusDash.CheckKiller(true);
					MessageBox.Show(status switch
					{
						OculusDash.Status.Downloaded => LocalizationStrings.OculusKiller_StatusDiscription_Downloaded,
						OculusDash.Status.Updated => LocalizationStrings.OculusKiller_StatusDiscription_Updated,
						OculusDash.Status.NoAction => LocalizationStrings.OculusKiller_StatusDiscription_NoAction
					}, "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (Exception exception)
				{
					exception.ShowMessageBox();
				}
			
			}

			private SoundPlayer Egg = null;
			private void slIconRealism_Scroll(object sender, EventArgs e)
			{
				Change_IconRealism_DiscriptionAndImage();

				if (_IconsRealismLevel == _MaxIconsRealismLevel)
				{
					picRealismLevel.BackgroundImage = GetImage("Settings", "RealismLevelImage", "Max"); //: add meme image left to that slider
					Egg = new SoundPlayer(@"images\Settings\Max\Horizon\Heart.png");
					Egg.Play();//: play BMttH: can you break my heart
				}
				else
				{
					Egg?.Stop();
				}
				__DefaultSettings.Save();
			}

			protected override void OnClosing(CancelEventArgs e)
			{
				if(Egg!=null && MessageBox.Show(LocalizationStrings.SettingsForm_EasterEgg_OnClosing_Message__Stop_playing_amazing_music, LocalizationStrings.SettingsForm_EasterEgg_OnClosing_Title__Easter_egg_speaking_with_you, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) 
					Egg.Stop();

				base.OnClosing(e);
			}
			private void lbTrayIconColorValue_Click(object sender, EventArgs e)
			{
				__DefaultSettings.BlackMode = !__DefaultSettings.BlackMode;
				SetTrayColorValue();
				__DefaultSettings.Save();
			}
			private void EnableApplyButton(object sender, EventArgs e)
			{
				btnApply.Enabled = true;
			}

			private void cbKillSteamVR_CheckedChanged(object sender, EventArgs e)
			{
				__DefaultSettings.KillSteamVR_Enabled = cbKillSteamVR.Checked;
			}

			private void btnApply_Click(object sender, EventArgs e)
			{
				try
				{
					Thread.CurrentThread.CurrentUICulture = ((Language)cbLanguage.SelectedItem).Culture;

					if (cbSteamVRDisableMethod.SelectedValue!= null && __DefaultSettings.SteamVRDisablingMethod != (BreakMethod)cbSteamVRDisableMethod.SelectedValue)
					{
						if (_SteamVr.IsBroken)
						{
							if (MessageBox.Show("SteamVR needs to be restored before changing method. Do it now?", "Do you realy read this captions?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
								_SteamVr.Restore();
							else 
								cbSteamVRDisableMethod.SelectedValue = _SteamVr.Method;
						}
						__DefaultSettings.SteamVRDisablingMethod = _SteamVr.Method = (BreakMethod)cbSteamVRDisableMethod.SelectedValue;
					}

					__DefaultSettings.KillOculusDash = cbKillOculus.Checked;

					__DefaultSettings.Save();
				}
				catch (Exception exception)
				{
					exception.ShowMessageBox();
				}
			}
		#endregion

		private void SetTrayColorValue()
		{
			lbTrayIconColorValue.Text = __DefaultSettings.BlackMode ? LocalizationStrings.SettingsForm_lbTrayIconColorValue_Black : LocalizationStrings.SettingsForm_lbTrayIconColorValue_White;
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
