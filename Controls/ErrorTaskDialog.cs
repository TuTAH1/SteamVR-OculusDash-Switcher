using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteamVR_OculusDash_Switcher.Properties.Localization;
using TaskDialogButton = Ookii.Dialogs.WinForms.TaskDialogButton;

namespace TitaniumComparator.LogicClasses
{
    public static class ErrorTaskDialog
    {
	    public static void ShowMessageBox(this Exception? Error)
	    {
		    if(Error == null) return;

		    Ookii.Dialogs.WinForms.TaskDialog taskDialog = new();
		    taskDialog.Content = Error.Message;
		    //if(Error.Data.Count!=0) taskDialog.CollapsedControlText = Error.Data.ToDictionary().ToStringT();
		    if(Error.HelpLink!=null) taskDialog.Footer = $"<a href=\"{Error.HelpLink}\">{LocalizationStrings.ErrorTaskDialog__OpenMicrosoftDocs}</a>";
		    TaskDialogButton closeBtn = new(LocalizationStrings.Button__Close), 
			    copyBtn = new(LocalizationStrings.ErrorTaskDialog__Copy_to_Clipboard), 
			    innerExceptionBtn = new(LocalizationStrings.ErrorTaskDialog__Open_Inner_Exception);
		    innerExceptionBtn.Enabled = Error.InnerException != null;
		    taskDialog.Buttons.Add(closeBtn);
		    taskDialog.Buttons.Add(copyBtn);
		    taskDialog.Buttons.Add(innerExceptionBtn);

		    var button = taskDialog.ShowDialog();
		    if(button == closeBtn) taskDialog.Dispose();
		    if(button == copyBtn) {Thread thread = new(() => Clipboard.SetText(Error.Message));
			    thread.SetApartmentState(ApartmentState.STA);
			    thread.Start();
			    thread.Join();
			    Error.ShowMessageBox();
		    }
		    if(button == innerExceptionBtn) 
			    Error.InnerException.ShowMessageBox();
	    }
    }
}
