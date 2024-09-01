using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using CleanSweep.Interfaces;

public class RecycleBinCleaner : ICleaner
{
    private readonly RichTextBox _outputWindow;
    private long _preCleanupSize;

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

    [DllImport("shell32.dll")]
    private static extern int SHQueryRecycleBin(string pszRootPath, ref SHQUERYRBINFO pSHQueryRBInfo);

    [StructLayout(LayoutKind.Sequential)]
    private struct SHQUERYRBINFO
    {
        public int cbSize;
        public long i64Size;
        public long i64NumItems;
    }

    private const uint SHERB_NOCONFIRMATION = 0x00000001;
    private const uint SHERB_NOPROGRESSUI = 0x00000002;
    private const uint SHERB_NOSOUND = 0x00000004;

    public RecycleBinCleaner(RichTextBox outputWindow)
    {
        _outputWindow = outputWindow;
    }

    public (string FileType, int SpaceInMB) GetReclaimableSpace()
    {
        try
        {
            _preCleanupSize = CalculateRecycleBinSize();
            int spaceInMB = ConvertBytesToMegabytes(_preCleanupSize);
            return ("Recycle Bin", spaceInMB);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return ("Recycle Bin", 0);
        }
    }

    public async Task Reclaim()
    {
        SafeAppendText("Cleaning Recycle Bin...\n", Color.Blue);

        try
        {
            await Task.Run(() =>
            {
                uint result = SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
                if (result == 0)
                {
                    SafeAppendText("Recycle Bin cleaned successfully.\n", Color.Green);
                }
                else
                {
                    SafeAppendText($"Recycle Bin is already empty!\n", Color.Red);
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public long ReportReclaimedSpace()
    {
        try
        {
            long postCleanupSize = CalculateRecycleBinSize();
            long reclaimedSpace = _preCleanupSize - postCleanupSize;
            return ConvertBytesToMegabytes(reclaimedSpace);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 0;
        }
    }

    private long CalculateRecycleBinSize()
    {
        SHQUERYRBINFO shQueryRBInfo = new SHQUERYRBINFO
        {
            cbSize = Marshal.SizeOf(typeof(SHQUERYRBINFO))
        };

        int result = SHQueryRecycleBin(null, ref shQueryRBInfo);
        if (result == 0) // S_OK
        {
            return shQueryRBInfo.i64Size;
        }
        else
        {
            return 0;
        }
    }

    private int ConvertBytesToMegabytes(long bytes)
    {
        return (int)(bytes / 1024 / 1024);
    }

    private void SafeAppendText(string text, Color color)
    {
        if (_outputWindow.InvokeRequired)
        {
            _outputWindow.Invoke(new Action(() => SafeAppendText(text, color)));
        }
        else
        {
            RichTextBoxExtensions.AppendText(_outputWindow, text, color);
        }
    }
}