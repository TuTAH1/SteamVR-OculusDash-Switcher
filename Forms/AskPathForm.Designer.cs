namespace SteamVR_OculusDash_Switcher.Forms
{
	partial class AskPathForm
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
			this.lbText = new System.Windows.Forms.Label();
			this.btnApply = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.lbPath = new System.Windows.Forms.Label();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lbText
			// 
			this.lbText.AutoSize = true;
			this.lbText.Location = new System.Drawing.Point(12, 9);
			this.lbText.Name = "lbText";
			this.lbText.Size = new System.Drawing.Size(59, 25);
			this.lbText.TabIndex = 0;
			this.lbText.Text = "label1";
			// 
			// btnApply
			// 
			this.btnApply.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnApply.Location = new System.Drawing.Point(0, 193);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(844, 39);
			this.btnApply.TabIndex = 0;
			this.btnApply.Text = "Apply";
			this.btnApply.UseVisualStyleBackColor = true;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(68, 51);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(356, 31);
			this.textBox1.TabIndex = 0;
			// 
			// lbPath
			// 
			this.lbPath.AutoSize = true;
			this.lbPath.Location = new System.Drawing.Point(12, 54);
			this.lbPath.Name = "lbPath";
			this.lbPath.Size = new System.Drawing.Size(50, 25);
			this.lbPath.TabIndex = 1;
			this.lbPath.Text = "Path:";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(430, 49);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(112, 34);
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.Text = "Browse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			// 
			// AskPathForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(844, 232);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.lbText);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.lbPath);
			this.Name = "AskPathForm";
			this.Text = "AskPath";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbText;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label lbPath;
		private System.Windows.Forms.Button btnBrowse;
	}
}