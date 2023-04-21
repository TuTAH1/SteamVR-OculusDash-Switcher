using System;
using System.Windows.Forms;
using SteamVR_OculusDash_Switcher.Logic;

namespace SteamVR_OculusDash_Switcher
{
	partial class SettingsForm : Form

	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private AnimationManager animationManager;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			lbLanguage = new Label();
			picLanguage = new PictureBox();
			comboLanguage = new ComboBox();
			btnApply = new Button();
			lbSteamVRDisableMethod = new Label();
			comboSteamVRDisableMethod = new ComboBox();
			ttCbSteamVR = new ToolTip(components);
			slideIconRealism = new TrackBar();
			lbIconsRealism = new Label();
			gbInterface = new GroupBox();
			picRealismLevel = new PictureBox();
			lbTrayIconColorValue = new Label();
			lbTrayIconColor = new Label();
			cbKillSteamVR = new CheckBox();
			lbIconsRealismDiscription = new Label();
			gbFunctions = new GroupBox();
			btnCheckOculusKillerUpdates = new Button();
			cbKillOculus = new CheckBox();
			lbSaved = new Label();
			animationManager = new AnimationManager(components);
			gbUpdate = new GroupBox();
			lbCurrentVersionText = new Label();
			lbCurrentVersionNumber = new Label();
			lbLastestVersionText = new Label();
			lbLastestVersionNumber = new Label();
			btnUpdate = new Button();
			((System.ComponentModel.ISupportInitialize)picLanguage).BeginInit();
			((System.ComponentModel.ISupportInitialize)slideIconRealism).BeginInit();
			gbInterface.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)picRealismLevel).BeginInit();
			gbFunctions.SuspendLayout();
			gbUpdate.SuspendLayout();
			SuspendLayout();
			// 
			// lbLanguage
			// 
			lbLanguage.AutoSize = true;
			lbLanguage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			lbLanguage.Location = new System.Drawing.Point(40, 31);
			lbLanguage.Name = "lbLanguage";
			lbLanguage.Size = new System.Drawing.Size(89, 25);
			lbLanguage.TabIndex = 999;
			lbLanguage.Text = "Language";
			// 
			// picLanguage
			// 
			picLanguage.ErrorImage = null;
			picLanguage.Location = new System.Drawing.Point(9, 31);
			picLanguage.Name = "picLanguage";
			picLanguage.Size = new System.Drawing.Size(25, 25);
			picLanguage.SizeMode = PictureBoxSizeMode.StretchImage;
			picLanguage.TabIndex = 1;
			picLanguage.TabStop = false;
			// 
			// comboLanguage
			// 
			comboLanguage.FlatStyle = FlatStyle.System;
			comboLanguage.FormattingEnabled = true;
			comboLanguage.Location = new System.Drawing.Point(0, 62);
			comboLanguage.Name = "comboLanguage";
			comboLanguage.Size = new System.Drawing.Size(182, 33);
			comboLanguage.TabIndex = 10;
			// 
			// btnApply
			// 
			btnApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnApply.FlatStyle = FlatStyle.System;
			btnApply.Location = new System.Drawing.Point(555, 354);
			btnApply.Margin = new Padding(10);
			btnApply.Name = "btnApply";
			btnApply.Size = new System.Drawing.Size(112, 34);
			btnApply.TabIndex = 3;
			btnApply.Text = "Apply";
			btnApply.UseVisualStyleBackColor = true;
			btnApply.Click += btnApply_Click;
			// 
			// lbSteamVRDisableMethod
			// 
			lbSteamVRDisableMethod.AutoSize = true;
			lbSteamVRDisableMethod.Location = new System.Drawing.Point(9, 31);
			lbSteamVRDisableMethod.Margin = new Padding(3, 12, 3, 0);
			lbSteamVRDisableMethod.Name = "lbSteamVRDisableMethod";
			lbSteamVRDisableMethod.Size = new System.Drawing.Size(212, 25);
			lbSteamVRDisableMethod.TabIndex = 999;
			lbSteamVRDisableMethod.Text = "SteamVR disable method";
			// 
			// comboSteamVRDisableMethod
			// 
			comboSteamVRDisableMethod.FlatStyle = FlatStyle.System;
			comboSteamVRDisableMethod.FormattingEnabled = true;
			comboSteamVRDisableMethod.Location = new System.Drawing.Point(6, 59);
			comboSteamVRDisableMethod.Name = "comboSteamVRDisableMethod";
			comboSteamVRDisableMethod.Size = new System.Drawing.Size(182, 33);
			comboSteamVRDisableMethod.TabIndex = 1;
			comboSteamVRDisableMethod.DrawItem += cbSteamVRDisableMethod_DrawItem;
			comboSteamVRDisableMethod.SelectedIndexChanged += EnableApplyButton;
			comboSteamVRDisableMethod.DropDownClosed += cbSteamVRDisableMethod_DropDownClosed;
			// 
			// slideIconRealism
			// 
			slideIconRealism.Location = new System.Drawing.Point(9, 135);
			slideIconRealism.Maximum = 5;
			slideIconRealism.Name = "slideIconRealism";
			slideIconRealism.Size = new System.Drawing.Size(156, 69);
			slideIconRealism.TabIndex = 11;
			slideIconRealism.Value = 1;
			slideIconRealism.Scroll += slIconRealism_Scroll;
			// 
			// lbIconsRealism
			// 
			lbIconsRealism.AutoSize = true;
			lbIconsRealism.Location = new System.Drawing.Point(9, 107);
			lbIconsRealism.Margin = new Padding(3, 12, 3, 0);
			lbIconsRealism.Name = "lbIconsRealism";
			lbIconsRealism.Size = new System.Drawing.Size(155, 25);
			lbIconsRealism.TabIndex = 999;
			lbIconsRealism.Text = "Icons realism level";
			lbIconsRealism.Resize += lbIconsRealism_Resize;
			// 
			// gbInterface
			// 
			gbInterface.AutoSize = true;
			gbInterface.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			gbInterface.Controls.Add(picRealismLevel);
			gbInterface.Controls.Add(lbTrayIconColorValue);
			gbInterface.Controls.Add(lbTrayIconColor);
			gbInterface.Controls.Add(cbKillSteamVR);
			gbInterface.Controls.Add(lbIconsRealismDiscription);
			gbInterface.Controls.Add(lbLanguage);
			gbInterface.Controls.Add(lbIconsRealism);
			gbInterface.Controls.Add(slideIconRealism);
			gbInterface.Controls.Add(picLanguage);
			gbInterface.Controls.Add(comboLanguage);
			gbInterface.Location = new System.Drawing.Point(318, 12);
			gbInterface.Name = "gbInterface";
			gbInterface.Size = new System.Drawing.Size(295, 315);
			gbInterface.TabIndex = 999;
			gbInterface.TabStop = false;
			gbInterface.Text = "Interface";
			// 
			// picRealismLevel
			// 
			picRealismLevel.BackgroundImageLayout = ImageLayout.Zoom;
			picRealismLevel.Location = new System.Drawing.Point(170, 107);
			picRealismLevel.Name = "picRealismLevel";
			picRealismLevel.Size = new System.Drawing.Size(56, 69);
			picRealismLevel.TabIndex = 1001;
			picRealismLevel.TabStop = false;
			// 
			// lbTrayIconColorValue
			// 
			lbTrayIconColorValue.AutoSize = true;
			lbTrayIconColorValue.BorderStyle = BorderStyle.Fixed3D;
			lbTrayIconColorValue.Cursor = Cursors.Hand;
			lbTrayIconColorValue.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			lbTrayIconColorValue.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			lbTrayIconColorValue.Location = new System.Drawing.Point(135, 263);
			lbTrayIconColorValue.Margin = new Padding(0);
			lbTrayIconColorValue.Name = "lbTrayIconColorValue";
			lbTrayIconColorValue.Size = new System.Drawing.Size(157, 23);
			lbTrayIconColorValue.TabIndex = 1004;
			lbTrayIconColorValue.Text = "◄ White/Black ►";
			lbTrayIconColorValue.Click += lbTrayIconColorValue_Click;
			// 
			// lbTrayIconColor
			// 
			lbTrayIconColor.AutoSize = true;
			lbTrayIconColor.Location = new System.Drawing.Point(0, 263);
			lbTrayIconColor.Margin = new Padding(3, 12, 0, 0);
			lbTrayIconColor.Name = "lbTrayIconColor";
			lbTrayIconColor.Size = new System.Drawing.Size(130, 25);
			lbTrayIconColor.TabIndex = 1003;
			lbTrayIconColor.Text = "Tray icon color:";
			// 
			// cbKillSteamVR
			// 
			cbKillSteamVR.AutoSize = true;
			cbKillSteamVR.Location = new System.Drawing.Point(6, 219);
			cbKillSteamVR.Margin = new Padding(3, 12, 3, 3);
			cbKillSteamVR.Name = "cbKillSteamVR";
			cbKillSteamVR.Size = new System.Drawing.Size(255, 29);
			cbKillSteamVR.TabIndex = 1000;
			cbKillSteamVR.Text = "show \"Kill SteamVR\" option";
			cbKillSteamVR.UseVisualStyleBackColor = true;
			cbKillSteamVR.CheckedChanged += cbKillSteamVR_CheckedChanged;
			// 
			// lbIconsRealismDiscription
			// 
			lbIconsRealismDiscription.AutoSize = true;
			lbIconsRealismDiscription.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			lbIconsRealismDiscription.Location = new System.Drawing.Point(6, 179);
			lbIconsRealismDiscription.Name = "lbIconsRealismDiscription";
			lbIconsRealismDiscription.Size = new System.Drawing.Size(163, 19);
			lbIconsRealismDiscription.TabIndex = 999;
			lbIconsRealismDiscription.Text = "lbIconsRealismDiscription";
			// 
			// gbFunctions
			// 
			gbFunctions.AutoSize = true;
			gbFunctions.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			gbFunctions.Controls.Add(btnCheckOculusKillerUpdates);
			gbFunctions.Controls.Add(cbKillOculus);
			gbFunctions.Controls.Add(lbSteamVRDisableMethod);
			gbFunctions.Controls.Add(comboSteamVRDisableMethod);
			gbFunctions.Location = new System.Drawing.Point(12, 12);
			gbFunctions.Name = "gbFunctions";
			gbFunctions.Size = new System.Drawing.Size(255, 206);
			gbFunctions.TabIndex = 999;
			gbFunctions.TabStop = false;
			gbFunctions.Text = "Main functions";
			// 
			// btnCheckOculusKillerUpdates
			// 
			btnCheckOculusKillerUpdates.AutoSize = true;
			btnCheckOculusKillerUpdates.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			btnCheckOculusKillerUpdates.FlatStyle = FlatStyle.System;
			btnCheckOculusKillerUpdates.Location = new System.Drawing.Point(6, 142);
			btnCheckOculusKillerUpdates.Name = "btnCheckOculusKillerUpdates";
			btnCheckOculusKillerUpdates.Size = new System.Drawing.Size(243, 34);
			btnCheckOculusKillerUpdates.TabIndex = 3;
			btnCheckOculusKillerUpdates.Text = "Check Oculus Killer updates";
			btnCheckOculusKillerUpdates.UseVisualStyleBackColor = true;
			btnCheckOculusKillerUpdates.Click += btnCheckOculusKillerUpdates_Click;
			// 
			// cbKillOculus
			// 
			cbKillOculus.AutoSize = true;
			cbKillOculus.Location = new System.Drawing.Point(6, 107);
			cbKillOculus.Margin = new Padding(3, 12, 3, 3);
			cbKillOculus.Name = "cbKillOculus";
			cbKillOculus.Size = new System.Drawing.Size(164, 29);
			cbKillOculus.TabIndex = 2;
			cbKillOculus.Text = "Kill Oculus Dash";
			cbKillOculus.UseVisualStyleBackColor = true;
			cbKillOculus.CheckedChanged += EnableApplyButton;
			// 
			// lbSaved
			// 
			lbSaved.AutoSize = true;
			lbSaved.Location = new System.Drawing.Point(479, 359);
			lbSaved.Margin = new Padding(3, 0, 13, 0);
			lbSaved.Name = "lbSaved";
			lbSaved.Size = new System.Drawing.Size(65, 25);
			lbSaved.TabIndex = 1000;
			lbSaved.Text = "Saved!";
			// 
			// gbUpdate
			// 
			gbUpdate.AutoSize = true;
			gbUpdate.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			gbUpdate.Controls.Add(btnUpdate);
			gbUpdate.Controls.Add(lbLastestVersionNumber);
			gbUpdate.Controls.Add(lbLastestVersionText);
			gbUpdate.Controls.Add(lbCurrentVersionNumber);
			gbUpdate.Controls.Add(lbCurrentVersionText);
			gbUpdate.Location = new System.Drawing.Point(12, 234);
			gbUpdate.Name = "gbUpdate";
			gbUpdate.Size = new System.Drawing.Size(243, 120);
			gbUpdate.TabIndex = 1001;
			gbUpdate.TabStop = false;
			gbUpdate.Text = "Updates";
			// 
			// lbCurrentVersionText
			// 
			lbCurrentVersionText.AutoSize = true;
			lbCurrentVersionText.Location = new System.Drawing.Point(9, 38);
			lbCurrentVersionText.Name = "lbCurrentVersionText";
			lbCurrentVersionText.Size = new System.Drawing.Size(137, 25);
			lbCurrentVersionText.TabIndex = 0;
			lbCurrentVersionText.Text = "Current Version:";
			// 
			// lbCurrentVersionNumber
			// 
			lbCurrentVersionNumber.AutoSize = true;
			lbCurrentVersionNumber.Location = new System.Drawing.Point(152, 39);
			lbCurrentVersionNumber.Name = "lbCurrentVersionNumber";
			lbCurrentVersionNumber.Size = new System.Drawing.Size(59, 25);
			lbCurrentVersionNumber.TabIndex = 1;
			lbCurrentVersionNumber.Text = "v0.0.0";
			// 
			// lbLastestVersionText
			// 
			lbLastestVersionText.AutoSize = true;
			lbLastestVersionText.Location = new System.Drawing.Point(9, 68);
			lbLastestVersionText.Name = "lbLastestVersionText";
			lbLastestVersionText.Size = new System.Drawing.Size(133, 25);
			lbLastestVersionText.TabIndex = 2;
			lbLastestVersionText.Text = "Lastest Version:";
			// 
			// lbLastestVersionNumber
			// 
			lbLastestVersionNumber.AutoSize = true;
			lbLastestVersionNumber.Location = new System.Drawing.Point(152, 68);
			lbLastestVersionNumber.Name = "lbLastestVersionNumber";
			lbLastestVersionNumber.Size = new System.Drawing.Size(85, 25);
			lbLastestVersionNumber.TabIndex = 3;
			lbLastestVersionNumber.Text = "unknown";
			// 
			// btnUpdate
			// 
			btnUpdate.Dock = DockStyle.Bottom;
			btnUpdate.Enabled = false;
			btnUpdate.Location = new System.Drawing.Point(3, 83);
			btnUpdate.Name = "btnUpdate";
			btnUpdate.Size = new System.Drawing.Size(237, 34);
			btnUpdate.TabIndex = 4;
			btnUpdate.Text = "Update";
			btnUpdate.UseVisualStyleBackColor = true;
			// 
			// SettingsForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(675, 397);
			Controls.Add(gbUpdate);
			Controls.Add(lbSaved);
			Controls.Add(gbFunctions);
			Controls.Add(gbInterface);
			Controls.Add(btnApply);
			Name = "SettingsForm";
			Text = "Settings";
			((System.ComponentModel.ISupportInitialize)picLanguage).EndInit();
			((System.ComponentModel.ISupportInitialize)slideIconRealism).EndInit();
			gbInterface.ResumeLayout(false);
			gbInterface.PerformLayout();
			((System.ComponentModel.ISupportInitialize)picRealismLevel).EndInit();
			gbFunctions.ResumeLayout(false);
			gbFunctions.PerformLayout();
			gbUpdate.ResumeLayout(false);
			gbUpdate.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label lbLanguage;
		private System.Windows.Forms.PictureBox picLanguage;
		private System.Windows.Forms.ComboBox comboLanguage;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.Label lbSteamVRDisableMethod;
		private ComboBox comboSteamVRDisableMethod;
		private ToolTip ttCbSteamVR;
		private TrackBar slideIconRealism;
		private Label lbIconsRealism;
		private GroupBox gbInterface;
		private GroupBox gbFunctions;
		private CheckBox cbKillOculus;
		private Button btnCheckOculusKillerUpdates;
		private Label lbIconsRealismDiscription;
		private CheckBox cbKillSteamVR;
		private PictureBox picRealismLevel;
		private Label lbTrayIconColor;
		private Label lbTrayIconColorValue;
		private Label lbSaved;
		private GroupBox gbUpdate;
		private Label lbLastestVersionNumber;
		private Label lbLastestVersionText;
		private Label lbCurrentVersionNumber;
		private Label lbCurrentVersionText;
		private Button btnUpdate;
	}
}