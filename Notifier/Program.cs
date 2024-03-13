using Microsoft.Win32.TaskScheduler;

namespace Notifier;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Notifier - tool for scheduling messages locally");
            Console.WriteLine("Usage:");
            Console.WriteLine("- by time:     notifier.exe -t 21:30 message");
            Console.WriteLine("- by duration: notifier.exe -d 1m30s message");
            return;
        }
        DoInit(args);
    }

    static void DoInit(string[] args)
    {
        try
        {
            var time = args[0] switch
            {
                "-d" => ParseDuration(args[1]),
                "-t" => ParseTime(args[1]),
                _ => throw new Exception("unknown flag"),
            };
            var taskName = "notifier-task-" + DateTime.Now.Ticks;
            using var ts = new TaskService();
            var task = ts.NewTask();
            task.Triggers.Add(new TimeTrigger { StartBoundary = time });
            string v = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location)!;
            task.Actions.Add(new ExecAction(Path.Combine(v, "notifiernotify.exe"), $"{taskName} {string.Join(" ", args[2..])}"));
            ts.RootFolder.RegisterTaskDefinition(taskName, task);
            Console.WriteLine($"Scheduled at {time}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    static DateTime ParseDuration(string arg)
    {
        var ts = new TimeSpan();
        var str = "";
        foreach (var c in arg)
        {
            if (char.IsDigit(c)) str += c;
            else
            {
                if (!int.TryParse(str, out var integer)) throw new Exception("invalid duration");
                ts = ts.Add(c switch
                {
                    's' => TimeSpan.FromSeconds(integer),
                    'm' => TimeSpan.FromMinutes(integer),
                    'h' => TimeSpan.FromHours(integer),
                    'd' => TimeSpan.FromDays(integer),
                    _ => throw new Exception("invalid duration")
                });
                str = "";
            }
        }
        return DateTime.Now + ts;
    }

    static DateTime ParseTime(string arg)
    {
        try
        {
            var r = DateTime.Today + TimeOnly.Parse(arg).ToTimeSpan();
            if (r < DateTime.Now) r = r.AddDays(1);
            return r;
        }
        catch
        {
            throw new Exception("invalid time");
        }
    }
}
