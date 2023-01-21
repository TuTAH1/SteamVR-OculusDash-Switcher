using System.Diagnostics;
using System.Windows.Forms;

namespace SteamVR_OculusDash_Switcher.Forms
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			Stopwatch test = Stopwatch.StartNew();
			var lab = new Label();
			this.Controls.Add(lab);
			//label1.CopyPropertiesTo(lab);
			lab.Text = "Test";
			lab.Top = 20;
			//lab.Top += lab.Height + lab.Padding.Top;
			test.Stop();
			MessageBox.Show(test.ElapsedMilliseconds.ToString());
		}
	}
}
