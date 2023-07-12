using System;
using System.Threading;
using System.Windows.Forms;
using TaskDialogButton = Ookii.Dialogs.WinForms.TaskDialogButton;
using Localization = Titanium.ErrorTaskDialog_Localisation;

namespace Titanium
{

	public static class ErrorTaskDialog
	{
		public static void ShowMessageBox(this Exception Error, string Title = null)
		{
			if (Error == null) return;


			Ookii.Dialogs.WinForms.TaskDialog taskDialog = new();
			taskDialog.WindowTitle = Title ?? Localization.Title_Error;
			taskDialog.Content = Error.Message;
			//if(Error.Data.Count!=0) taskDialog.CollapsedControlText = Error.Data.ToDictionary().ToStringT();
			if (Error.HelpLink != null) taskDialog.Footer = $"<a href=\"{Error.HelpLink}\">{"d"}</a>";
			//extract "close" localisation from resource

			TaskDialogButton closeBtn = new(Localization.Close),
				copyBtn = new(Localization.Copy_to_Clipboard),
				innerExceptionBtn = new(Localization.Open_Inner_Exception),
				callStack = new(Localization.Open_Callstack);
			innerExceptionBtn.Enabled = Error.InnerException != null;
			taskDialog.Buttons.Add(closeBtn);
			taskDialog.Buttons.Add(copyBtn);
			taskDialog.Buttons.Add(callStack);
			taskDialog.Buttons.Add(innerExceptionBtn);

			var button = taskDialog.ShowDialog();
			if (button == closeBtn) taskDialog.Dispose();
			if (button == copyBtn)
			{
				Thread thread = new(() => Clipboard.SetText(Error.Message));
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
				thread.Join();
				Error.ShowMessageBox();
			}

			if (button == callStack)
			{
				MessageBox.Show(Error.StackTrace);
			}
			if (button == innerExceptionBtn)
				Error.InnerException.ShowMessageBox();
		}
	}
}
