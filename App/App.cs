using System;
using System.Diagnostics;
using System.IO;
using System.Text;

class App
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        foreach (var procName in new[] { "Waiter", "Consumer" })
            foreach (var proc in Process.GetProcessesByName(procName))
                try { proc.Kill(); proc.WaitForExit(); } catch { /* ignore */ }
        


        bool runnedConsumer = RunConsole("\"..\\..\\..\\..\\Consumer\\Consumer.csproj\"");
        if (!runnedConsumer)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌🍳 The Chef called in sick today! No delicious dishes will be served 😢");
            Console.ResetColor();
        }

        bool runnedWaiter = RunConsole("\"..\\..\\..\\..\\Waiter\\Waiter.csproj\"");
        if (!runnedWaiter)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌🧾 The Waiter is on strike! Nobody’s taking orders right now 🚫");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n✅ Restaurant has opened successfully. Bon appétit!");
        Console.ResetColor();




        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c exit",  // tells cmd to close immediately
            CreateNoWindow = true,
            UseShellExecute = false
        });

        Environment.Exit(0);
        return;
    }

    static bool RunConsole(string projectPath)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                // Using /c instead of /k so the new console closes automatically after it finishes
                Arguments = $"/c start dotnet run --project {projectPath}",
                UseShellExecute = true,
                CreateNoWindow = true
            };

            Process.Start(psi);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
