using System;
using System.Windows.Forms;

namespace SteamVR_OculusDash_Switcher
{
	partial class SettingsForm : Form

	{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

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
			this.cbLanguage = new System.Windows.Forms.ComboBox();
			this.btnApply = new System.Windows.Forms.Button();
			this.lbSteamVRDisableMethod = new System.Windows.Forms.Label();
			this.cbSteamVRDisableMethod = new System.Windows.Forms.ComboBox();
			this.ttCbSteamVR = new System.Windows.Forms.ToolTip(this.components);
			this.slideIconRealism = new System.Windows.Forms.TrackBar();
			this.lbIconsRealism = new System.Windows.Forms.Label();
			this.gbInterface = new System.Windows.Forms.GroupBox();
			this.lbTrayIconColorValue = new System.Windows.Forms.Label();
			this.lbTrayIconColor = new System.Windows.Forms.Label();
			this.panelRealismLevel = new System.Windows.Forms.Panel();
			this.picRealismLevel = new System.Windows.Forms.PictureBox();
			this.cbKillSteamVR = new System.Windows.Forms.CheckBox();
			this.lbIconsRealismDiscription = new System.Windows.Forms.Label();
			this.gbFunctions = new System.Windows.Forms.GroupBox();
			this.btnCheckOculusKillerUpdates = new System.Windows.Forms.Button();
			this.cbKillOculus = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.picLanguage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.slideIconRealism)).BeginInit();
			this.gbInterface.SuspendLayout();
			this.panelRealismLevel.SuspendLayout();
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
			// cbLanguage
			// 
			this.cbLanguage.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cbLanguage.FormattingEnabled = true;
			this.cbLanguage.Location = new System.Drawing.Point(0, 62);
			this.cbLanguage.Name = "cbLanguage";
			this.cbLanguage.Size = new System.Drawing.Size(182, 33);
			this.cbLanguage.TabIndex = 10;
			// 
			// btnApply
			// 
			this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnApply.Location = new System.Drawing.Point(771, 404);
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
			this.lbSteamVRDisableMethod.Location = new System.Drawing.Point(6, 31);
			this.lbSteamVRDisableMethod.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
			this.lbSteamVRDisableMethod.Name = "lbSteamVRDisableMethod";
			this.lbSteamVRDisableMethod.Size = new System.Drawing.Size(212, 25);
			this.lbSteamVRDisableMethod.TabIndex = 999;
			this.lbSteamVRDisableMethod.Text = "SteamVR disable method";
			// 
			// cbSteamVRDisableMethod
			// 
			this.cbSteamVRDisableMethod.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cbSteamVRDisableMethod.FormattingEnabled = true;
			this.cbSteamVRDisableMethod.Location = new System.Drawing.Point(6, 59);
			this.cbSteamVRDisableMethod.Name = "cbSteamVRDisableMethod";
			this.cbSteamVRDisableMethod.Size = new System.Drawing.Size(182, 33);
			this.cbSteamVRDisableMethod.TabIndex = 1;
			this.cbSteamVRDisableMethod.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbSteamVRDisableMethod_DrawItem);
			this.cbSteamVRDisableMethod.SelectedIndexChanged += new System.EventHandler(this.EnableApplyButton);
			this.cbSteamVRDisableMethod.DropDownClosed += new System.EventHandler(this.cbSteamVRDisableMethod_DropDownClosed);
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
			// 
			// gbInterface
			// 
			this.gbInterface.Controls.Add(this.lbTrayIconColorValue);
			this.gbInterface.Controls.Add(this.lbTrayIconColor);
			this.gbInterface.Controls.Add(this.panelRealismLevel);
			this.gbInterface.Controls.Add(this.cbKillSteamVR);
			this.gbInterface.Controls.Add(this.lbIconsRealismDiscription);
			this.gbInterface.Controls.Add(this.lbLanguage);
			this.gbInterface.Controls.Add(this.lbIconsRealism);
			this.gbInterface.Controls.Add(this.slideIconRealism);
			this.gbInterface.Controls.Add(this.picLanguage);
			this.gbInterface.Controls.Add(this.cbLanguage);
			this.gbInterface.Location = new System.Drawing.Point(318, 12);
			this.gbInterface.Name = "gbInterface";
			this.gbInterface.Size = new System.Drawing.Size(339, 376);
			this.gbInterface.TabIndex = 999;
			this.gbInterface.TabStop = false;
			this.gbInterface.Text = "Interface";
			// 
			// lbTrayIconColorValue
			// 
			this.lbTrayIconColorValue.AutoSize = true;
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
			// panelRealismLevel
			// 
			this.panelRealismLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panelRealismLevel.Controls.Add(this.picRealismLevel);
			this.panelRealismLevel.Location = new System.Drawing.Point(175, 107);
			this.panelRealismLevel.Name = "panelRealismLevel";
			this.panelRealismLevel.Size = new System.Drawing.Size(158, 69);
			this.panelRealismLevel.TabIndex = 1002;
			// 
			// picRealismLevel
			// 
			this.picRealismLevel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.picRealismLevel.Dock = System.Windows.Forms.DockStyle.Left;
			this.picRealismLevel.Location = new System.Drawing.Point(0, 0);
			this.picRealismLevel.Name = "picRealismLevel";
			this.picRealismLevel.Size = new System.Drawing.Size(56, 69);
			this.picRealismLevel.TabIndex = 1001;
			this.picRealismLevel.TabStop = false;
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
			this.gbFunctions.Controls.Add(this.btnCheckOculusKillerUpdates);
			this.gbFunctions.Controls.Add(this.cbKillOculus);
			this.gbFunctions.Controls.Add(this.lbSteamVRDisableMethod);
			this.gbFunctions.Controls.Add(this.cbSteamVRDisableMethod);
			this.gbFunctions.Location = new System.Drawing.Point(12, 12);
			this.gbFunctions.Name = "gbFunctions";
			this.gbFunctions.Size = new System.Drawing.Size(300, 376);
			this.gbFunctions.TabIndex = 999;
			this.gbFunctions.TabStop = false;
			this.gbFunctions.Text = "Main functions";
			// 
			// btnCheckOculusKillerUpdates
			// 
			this.btnCheckOculusKillerUpdates.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCheckOculusKillerUpdates.Location = new System.Drawing.Point(6, 142);
			this.btnCheckOculusKillerUpdates.Name = "btnCheckOculusKillerUpdates";
			this.btnCheckOculusKillerUpdates.Size = new System.Drawing.Size(246, 34);
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
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(895, 450);
			this.Controls.Add(this.gbFunctions);
			this.Controls.Add(this.gbInterface);
			this.Controls.Add(this.btnApply);
			this.Name = "SettingsForm";
			this.Text = "Settings";
			((System.ComponentModel.ISupportInitialize)(this.picLanguage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.slideIconRealism)).EndInit();
			this.gbInterface.ResumeLayout(false);
			this.gbInterface.PerformLayout();
			this.panelRealismLevel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picRealismLevel)).EndInit();
			this.gbFunctions.ResumeLayout(false);
			this.gbFunctions.PerformLayout();
			this.ResumeLayout(false);

	}

	#endregion

	private System.Windows.Forms.Label lbLanguage;
	private System.Windows.Forms.PictureBox picLanguage;
	private System.Windows.Forms.ComboBox cbLanguage;
	private System.Windows.Forms.Button btnApply;
	private System.Windows.Forms.Label lbSteamVRDisableMethod;
		private ComboBox cbSteamVRDisableMethod;
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
		private Panel panelRealismLevel;
		private Label lbTrayIconColor;
		private Label lbTrayIconColorValue;
	}
}