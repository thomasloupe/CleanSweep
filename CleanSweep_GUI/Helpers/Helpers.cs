using System;
using System.Drawing;
using System.Windows.Forms;

public static class Helpers
{
    public static void AddWaitText(RichTextBox outputWindow)
    {
        if (outputWindow.InvokeRequired)
        {
            outputWindow.Invoke(new Action(() => AddWaitText(outputWindow)));
        }
        else
        {
            outputWindow.AppendText(".", Color.Green);
        }
    }
}

public static class RichTextBoxExtensions
{
    public static void AppendText(this RichTextBox box, string text, Color color)
    {
        box.HideSelection = false;
        box.SelectionStart = box.TextLength;
        box.SelectionLength = 0;

        box.SelectionColor = color;
        box.AppendText(text);
        box.SelectionColor = box.ForeColor;
    }
}
