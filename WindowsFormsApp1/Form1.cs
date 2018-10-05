using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string userName = Environment.UserName;
        public string tempDirectory;
        long tempDirLength;
        long sizeInMegaBytes;
        public bool isBox1Checked = false;

        public Form1()
        {
            InitializeComponent();
            Console.WriteLine("User, " + userName + " located!");
            tempDirectory = "C:\\Users\\" + userName + "\\AppData\\Local\\Temp\\";
            Console.WriteLine("Temp directory located at " + tempDirectory);
            tempDirLength = Directory.GetFiles(tempDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
            sizeInMegaBytes = tempDirLength / 1024 / 1024;
            textBox1.Text = ("Temp Directory size: " + sizeInMegaBytes + "MB");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (isBox1Checked == true)
            {
                DirectoryInfo di = new DirectoryInfo(tempDirectory);

                foreach (FileInfo file in di.GetFiles())
                    try
                    {
                        {
                            file.Delete();

                            foreach (DirectoryInfo dir in di.GetDirectories())
                            {
                                dir.Delete(true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is IOException)
                        {
                            //Print the exception to our status strip and continue.
                            toolStripStatusLabel1.Text = "Error 1: IO exception.";
                        }
                        else if (ex is UnauthorizedAccessException)
                        {
                            //Print the exception to our status strip and continue.
                            toolStripStatusLabel1.Text = "Error 2: Access denied.";
                        }
                    }
                tempDirLength = Directory.GetFiles(tempDirectory, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                sizeInMegaBytes = tempDirLength / 1024 / 1024;
                textBox1.Text = "Temp Directory size: " + sizeInMegaBytes + "MB";
                toolStripStatusLabel1.Text = "Successfully deleted temp files!";
            }
            else if (isBox1Checked == false)
            {
                //Let the user know we did nothing because they selected no action to perform.
                toolStripStatusLabel1.Text = "You must select an action before using the 'Sweep It' button";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            //If checked, uncheck it.
            if (checkBox1.Checked == false)
            {
                checkBox1.Checked = false;
                isBox1Checked = false;
            }
            //If unchecked, check it.
            else
            {
                if (checkBox1.Checked == true)
                {
                    checkBox1.Checked = true;
                    isBox1Checked = true;
                }
            }
        }
    }
}
