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
			this.components = new System.ComponentModel.Container();
			this.lbLanguage = new System.Windows.Forms.Label();
			this.picLanguage = new System.Windows.Forms.PictureBox();
			this.comboLanguage = new System.Windows.Forms.ComboBox();
			this.btnApply = new System.Windows.Forms.Button();
			this.lbSteamVRDisableMethod = new System.Windows.Forms.Label();
			this.comboSteamVRDisableMethod = new System.Windows.Forms.ComboBox();
			this.ttCbSteamVR = new System.Windows.Forms.ToolTip(this.components);
			this.slideIconRealism = new System.Windows.Forms.TrackBar();
			this.lbIconsRealism = new System.Windows.Forms.Label();
			this.gbInterface = new System.Windows.Forms.GroupBox();
			this.picRealismLevel = new System.Windows.Forms.PictureBox();
			this.lbTrayIconColorValue = new System.Windows.Forms.Label();
			this.lbTrayIconColor = new System.Windows.Forms.Label();
			this.cbKillSteamVR = new System.Windows.Forms.CheckBox();
			this.lbIconsRealismDiscription = new System.Windows.Forms.Label();
			this.gbFunctions = new System.Windows.Forms.GroupBox();
			this.btnCheckOculusKillerUpdates = new System.Windows.Forms.Button();
			this.cbKillOculus = new System.Windows.Forms.CheckBox();
			this.lbSaved = new System.Windows.Forms.Label();
			this.animationManager = new SteamVR_OculusDash_Switcher.Logic.AnimationManager(this.components);
			((System.ComponentModel.ISupportInitialize)(this.picLanguage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.slideIconRealism)).BeginInit();
			this.gbInterface.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picRealismLevel)).BeginInit();
			this.gbFunctions.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbLanguage
			// 
			this.lbLanguage.AutoSize = true;
			this.lbLanguage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbLanguage.Location = new System.Drawing.Point(40, 31);
			this.lbLanguage.Name = "lbLanguage";
			this.lbLanguage.Size = new System.Drawing.Size(89, 25);
			this.lbLanguage.TabIndex = 999;
			this.lbLanguage.Text = "Language";
			// 
			// picLanguage
			// 
			this.picLanguage.ErrorImage = null;
			this.picLanguage.Location = new System.Drawing.Point(9, 31);
			this.picLanguage.Name = "picLanguage";
			this.picLanguage.Size = new System.Drawing.Size(25, 25);
			this.picLanguage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picLanguage.TabIndex = 1;
			this.picLanguage.TabStop = false;
			// 
			// comboLanguage
			// 
			this.comboLanguage.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.comboLanguage.FormattingEnabled = true;
			this.comboLanguage.Location = new System.Drawing.Point(0, 62);
			this.comboLanguage.Name = "comboLanguage";
			this.comboLanguage.Size = new System.Drawing.Size(182, 33);
			this.comboLanguage.TabIndex = 10;
			// 
			// btnApply
			// 
			this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnApply.Location = new System.Drawing.Point(555, 354);
			this.btnApply.Margin = new System.Windows.Forms.Padding(10);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(112, 34);
			this.btnApply.TabIndex = 3;
			this.btnApply.Text = "Apply";
			this.btnApply.UseVisualStyleBackColor = true;
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// lbSteamVRDisableMethod
			// 
			this.lbSteamVRDisableMethod.AutoSize = true;
			this.lbSteamVRDisableMethod.Location = new System.Drawing.Point(9, 31);
			this.lbSteamVRDisableMethod.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
			this.lbSteamVRDisableMethod.Name = "lbSteamVRDisableMethod";
			this.lbSteamVRDisableMethod.Size = new System.Drawing.Size(212, 25);
			this.lbSteamVRDisableMethod.TabIndex = 999;
			this.lbSteamVRDisableMethod.Text = "SteamVR disable method";
			// 
			// comboSteamVRDisableMethod
			// 
			this.comboSteamVRDisableMethod.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.comboSteamVRDisableMethod.FormattingEnabled = true;
			this.comboSteamVRDisableMethod.Location = new System.Drawing.Point(6, 59);
			this.comboSteamVRDisableMethod.Name = "comboSteamVRDisableMethod";
			this.comboSteamVRDisableMethod.Size = new System.Drawing.Size(182, 33);
			this.comboSteamVRDisableMethod.TabIndex = 1;
			this.comboSteamVRDisableMethod.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbSteamVRDisableMethod_DrawItem);
			this.comboSteamVRDisableMethod.SelectedIndexChanged += new System.EventHandler(this.EnableApplyButton);
			this.comboSteamVRDisableMethod.DropDownClosed += new System.EventHandler(this.cbSteamVRDisableMethod_DropDownClosed);
			// 
			// slideIconRealism
			// 
			this.slideIconRealism.Location = new System.Drawing.Point(9, 135);
			this.slideIconRealism.Maximum = 5;
			this.slideIconRealism.Name = "slideIconRealism";
			this.slideIconRealism.Size = new System.Drawing.Size(156, 69);
			this.slideIconRealism.TabIndex = 11;
			this.slideIconRealism.Value = 1;
			this.slideIconRealism.Scroll += new System.EventHandler(this.slIconRealism_Scroll);
			// 
			// lbIconsRealism
			// 
			this.lbIconsRealism.AutoSize = true;
			this.lbIconsRealism.Location = new System.Drawing.Point(9, 107);
			this.lbIconsRealism.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
			this.lbIconsRealism.Name = "lbIconsRealism";
			this.lbIconsRealism.Size = new System.Drawing.Size(155, 25);
			this.lbIconsRealism.TabIndex = 999;
			this.lbIconsRealism.Text = "Icons realism level";
			this.lbIconsRealism.Resize += new System.EventHandler(this.lbIconsRealism_Resize);
			// 
			// gbInterface
			// 
			this.gbInterface.AutoSize = true;
			this.gbInterface.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.gbInterface.Controls.Add(this.picRealismLevel);
			this.gbInterface.Controls.Add(this.lbTrayIconColorValue);
			this.gbInterface.Controls.Add(this.lbTrayIconColor);
			this.gbInterface.Controls.Add(this.cbKillSteamVR);
			this.gbInterface.Controls.Add(this.lbIconsRealismDiscription);
			this.gbInterface.Controls.Add(this.lbLanguage);
			this.gbInterface.Controls.Add(this.lbIconsRealism);
			this.gbInterface.Controls.Add(this.slideIconRealism);
			this.gbInterface.Controls.Add(this.picLanguage);
			this.gbInterface.Controls.Add(this.comboLanguage);
			this.gbInterface.Location = new System.Drawing.Point(318, 12);
			this.gbInterface.Name = "gbInterface";
			this.gbInterface.Size = new System.Drawing.Size(267, 315);
			this.gbInterface.TabIndex = 999;
			this.gbInterface.TabStop = false;
			this.gbInterface.Text = "Interface";
			// 
			// picRealismLevel
			// 
			this.picRealismLevel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.picRealismLevel.Location = new System.Drawing.Point(170, 107);
			this.picRealismLevel.Name = "picRealismLevel";
			this.picRealismLevel.Size = new System.Drawing.Size(56, 69);
			this.picRealismLevel.TabIndex = 1001;
			this.picRealismLevel.TabStop = false;
			// 
			// lbTrayIconColorValue
			// 
			this.lbTrayIconColorValue.AutoSize = true;
			this.lbTrayIconColorValue.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lbTrayIconColorValue.ForeColor = System.Drawing.SystemColors.MenuHighlight;
			this.lbTrayIconColorValue.Location = new System.Drawing.Point(135, 263);
			this.lbTrayIconColorValue.Margin = new System.Windows.Forms.Padding(0);
			this.lbTrayIconColorValue.Name = "lbTrayIconColorValue";
			this.lbTrayIconColorValue.Size = new System.Drawing.Size(105, 25);
			this.lbTrayIconColorValue.TabIndex = 1004;
			this.lbTrayIconColorValue.Text = "White/Black";
			this.lbTrayIconColorValue.Click += new System.EventHandler(this.lbTrayIconColorValue_Click);
			// 
			// lbTrayIconColor
			// 
			this.lbTrayIconColor.AutoSize = true;
			this.lbTrayIconColor.Location = new System.Drawing.Point(0, 263);
			this.lbTrayIconColor.Margin = new System.Windows.Forms.Padding(3, 12, 0, 0);
			this.lbTrayIconColor.Name = "lbTrayIconColor";
			this.lbTrayIconColor.Size = new System.Drawing.Size(130, 25);
			this.lbTrayIconColor.TabIndex = 1003;
			this.lbTrayIconColor.Text = "Tray icon color:";
			// 
			// cbKillSteamVR
			// 
			this.cbKillSteamVR.AutoSize = true;
			this.cbKillSteamVR.Location = new System.Drawing.Point(6, 219);
			this.cbKillSteamVR.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
			this.cbKillSteamVR.Name = "cbKillSteamVR";
			this.cbKillSteamVR.Size = new System.Drawing.Size(255, 29);
			this.cbKillSteamVR.TabIndex = 1000;
			this.cbKillSteamVR.Text = "show \"Kill SteamVR\" option";
			this.cbKillSteamVR.UseVisualStyleBackColor = true;
			this.cbKillSteamVR.CheckedChanged += new System.EventHandler(this.cbKillSteamVR_CheckedChanged);
			// 
			// lbIconsRealismDiscription
			// 
			this.lbIconsRealismDiscription.AutoSize = true;
			this.lbIconsRealismDiscription.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.lbIconsRealismDiscription.Location = new System.Drawing.Point(6, 179);
			this.lbIconsRealismDiscription.Name = "lbIconsRealismDiscription";
			this.lbIconsRealismDiscription.Size = new System.Drawing.Size(163, 19);
			this.lbIconsRealismDiscription.TabIndex = 999;
			this.lbIconsRealismDiscription.Text = "lbIconsRealismDiscription";
			// 
			// gbFunctions
			// 
			this.gbFunctions.AutoSize = true;
			this.gbFunctions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.gbFunctions.Controls.Add(this.btnCheckOculusKillerUpdates);
			this.gbFunctions.Controls.Add(this.cbKillOculus);
			this.gbFunctions.Controls.Add(this.lbSteamVRDisableMethod);
			this.gbFunctions.Controls.Add(this.comboSteamVRDisableMethod);
			this.gbFunctions.Location = new System.Drawing.Point(12, 12);
			this.gbFunctions.Name = "gbFunctions";
			this.gbFunctions.Size = new System.Drawing.Size(255, 206);
			this.gbFunctions.TabIndex = 999;
			this.gbFunctions.TabStop = false;
			this.gbFunctions.Text = "Main functions";
			// 
			// btnCheckOculusKillerUpdates
			// 
			this.btnCheckOculusKillerUpdates.AutoSize = true;
			this.btnCheckOculusKillerUpdates.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnCheckOculusKillerUpdates.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCheckOculusKillerUpdates.Location = new System.Drawing.Point(6, 142);
			this.btnCheckOculusKillerUpdates.Name = "btnCheckOculusKillerUpdates";
			this.btnCheckOculusKillerUpdates.Size = new System.Drawing.Size(243, 34);
			this.btnCheckOculusKillerUpdates.TabIndex = 3;
			this.btnCheckOculusKillerUpdates.Text = "Check Oculus Killer updates";
			this.btnCheckOculusKillerUpdates.UseVisualStyleBackColor = true;
			this.btnCheckOculusKillerUpdates.Click += new System.EventHandler(this.btnCheckOculusKillerUpdates_Click);
			// 
			// cbKillOculus
			// 
			this.cbKillOculus.AutoSize = true;
			this.cbKillOculus.Location = new System.Drawing.Point(6, 107);
			this.cbKillOculus.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
			this.cbKillOculus.Name = "cbKillOculus";
			this.cbKillOculus.Size = new System.Drawing.Size(164, 29);
			this.cbKillOculus.TabIndex = 2;
			this.cbKillOculus.Text = "Kill Oculus Dash";
			this.cbKillOculus.UseVisualStyleBackColor = true;
			this.cbKillOculus.CheckedChanged += new System.EventHandler(this.EnableApplyButton);
			// 
			// lbSaved
			// 
			this.lbSaved.AutoSize = true;
			this.lbSaved.Location = new System.Drawing.Point(479, 359);
			this.lbSaved.Margin = new System.Windows.Forms.Padding(3, 0, 13, 0);
			this.lbSaved.Name = "lbSaved";
			this.lbSaved.Size = new System.Drawing.Size(65, 25);
			this.lbSaved.TabIndex = 1000;
			this.lbSaved.Text = "Saved!";
			// 
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(675, 397);
			this.Controls.Add(this.lbSaved);
			this.Controls.Add(this.gbFunctions);
			this.Controls.Add(this.gbInterface);
			this.Controls.Add(this.btnApply);
			this.Name = "SettingsForm";
			this.Text = "Settings";
			((System.ComponentModel.ISupportInitialize)(this.picLanguage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.slideIconRealism)).EndInit();
			this.gbInterface.ResumeLayout(false);
			this.gbInterface.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picRealismLevel)).EndInit();
			this.gbFunctions.ResumeLayout(false);
			this.gbFunctions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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
	}
}