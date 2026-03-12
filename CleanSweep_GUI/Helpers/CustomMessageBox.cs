using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CleanSweep.Classes
{
    public static class CustomMessageBox
    {
        /// <summary>
        /// Displays a custom message box with multiple clickable links.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="links">An array of tuples containing link text and URLs.</param>
        public static void Show(string message, string title, params (string linkText, string linkUrl)[] links)
        {
            // Create a new form for the custom message box
            Form form = new Form
            {
                Text = title,
                ClientSize = new Size(400, 150 + (links.Length * 20)),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            // Create a Label to display the message
            Label messageLabel = new Label
            {
                Text = message,
                AutoSize = true,
                Location = new Point(10, 20)
            };
            form.Controls.Add(messageLabel);

            int yOffset = messageLabel.Bottom + 10;

            // Create a LinkLabel for each link provided
            foreach (var (linkText, linkUrl) in links)
            {
                LinkLabel linkLabel = new LinkLabel
                {
                    Text = linkText,
                    AutoSize = true,
                    Location = new Point(10, yOffset)
                };
                linkLabel.Links.Add(0, linkText.Length, linkUrl);
                linkLabel.LinkClicked += LinkLabel_LinkClicked;
                form.Controls.Add(linkLabel);
                yOffset = linkLabel.Bottom + 10;
            }

            // Create an OK button to close the message box
            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK
            };
            okButton.Location = new Point(form.ClientSize.Width / 2 - okButton.Width / 2, yOffset);
            form.Controls.Add(okButton);
            form.AcceptButton = okButton; // Allows Enter key to close the form

            // Show the custom message box as a dialog
            form.ShowDialog();
        }

        private static void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open the URL in the default browser
            string targetUrl = e.Link.LinkData as string;
            if (!string.IsNullOrEmpty(targetUrl))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = targetUrl,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to open the link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
