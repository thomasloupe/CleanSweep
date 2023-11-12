using System;
using System.Windows.Forms;
using System.Drawing;

public class UIHelpers
{
    private RichTextBox _richTextBox;

    public UIHelpers(RichTextBox richTextBox)
    {
        _richTextBox = richTextBox;
    }

    public void AddWaitText()
    {
        _richTextBox.Invoke(new Action(() =>
        {
            _richTextBox.AppendText(".", Color.Green);
        }));
    }
}

public static class RichTextBoxExtensions
{
    public static void AppendText(this RichTextBox box, string text, Color color)
    {
        box.SelectionStart = box.TextLength;
        box.SelectionLength = 0;

        box.SelectionColor = color;
        box.AppendText(text);
        box.SelectionColor = box.ForeColor;
    }
}
