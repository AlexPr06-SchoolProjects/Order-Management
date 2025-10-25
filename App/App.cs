using System;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_MAXIMIZE = 3;

    static void Main(string[] args)
    {
        IntPtr handle = GetConsoleWindow();
        ShowWindow(handle, SW_MAXIMIZE);

        Console.WriteLine("This is a fullscreen console application.");
        Console.ReadLine();
    }
}

//using System;
//using System.Diagnostics;
//using System.Runtime.InteropServices;
//using System.Threading;

//class Program
//{
//    [DllImport("user32.dll")]
//    static extern bool SetWindowPos(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

//    [DllImport("user32.dll")]
//    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

//    [DllImport("user32.dll")]
//    static extern int GetSystemMetrics(int nIndex);

//    const int SM_CXSCREEN = 0;
//    const int SM_CYSCREEN = 1;
//    const int SW_RESTORE = 9;

//    static void Main()
//    {
//        // Get current screen size
//        int screenWidth = GetSystemMetrics(SM_CXSCREEN);
//        int screenHeight = GetSystemMetrics(SM_CYSCREEN);

//        // Launch Waiter console in left half
//        StartAndPositionCmd(
//            @"..\..\..\..\Waiter\bin\Debug\net9.0\Waiter.exe",
//            0, 0, screenWidth / 2, screenHeight
//        );

//        // Launch Consumer console in right half
//        StartAndPositionCmd(
//            @"..\..\..\..\Consumer\bin\Debug\net9.0\Consumer.exe",
//            screenWidth / 2, 0, screenWidth / 2, screenHeight
//        );
//    }

//    static Process StartAndPositionCmd(string exePath, int x, int y, int width, int height)
//    {
//        // Start cmd.exe with /k so the console stays open
//        var proc = Process.Start(new ProcessStartInfo
//        {
//            FileName = "cmd.exe",
//            Arguments = $"/k \"{exePath}\"",
//            UseShellExecute = true,
//            CreateNoWindow = false,
//            WorkingDirectory = System.IO.Path.GetDirectoryName(exePath)
//        });

//        // Wait for the cmd window to appear
//        while (proc!.MainWindowHandle == IntPtr.Zero)
//        {
//            Thread.Sleep(100);
//            proc.Refresh();
//        }

//        // Extra wait for window initialization
//        Thread.Sleep(300);

//        // Restore in case minimized
//        ShowWindow(proc.MainWindowHandle, SW_RESTORE);
//        Thread.Sleep(100);

//        // Move and resize window to desired half-screen
//        SetWindowPos(proc.MainWindowHandle, x, y, width, height, true);

//        return proc;
//    }
//