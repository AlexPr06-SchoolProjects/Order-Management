
using System.Diagnostics;
using System.Text;

class App
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        foreach (var procName in new[] { "Waiter", "Consumer" })
            foreach (var proc in Process.GetProcessesByName(procName))
                try { proc.Kill(); proc.WaitForExit(); } catch { }
        


        var runnedConsumerProcess = RunConsole("\"..\\..\\..\\..\\Consumer\\Consumer.csproj\"");
        if (runnedConsumerProcess is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌🍳 The Chef called in sick today! No delicious dishes will be served 😢");
            Console.ResetColor();
            return;
        }

        var runnedWaiterProcess = RunConsole("\"..\\..\\..\\..\\Waiter\\Waiter.csproj\"");
        if (runnedWaiterProcess is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌🧾 The Waiter is on strike! Nobody’s taking orders right now 🚫");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n✅ Restaurant has opened successfully. Bon appétit!");
        Console.ResetColor();

        return;
    }

    static Process? RunConsole(string projectPath)
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

            var proc = Process.Start(psi);
            return proc;
        }
        catch
        {
            return null;
        }
    }
}
