using Microsoft.Win32.TaskScheduler;
using System.Windows.Forms;

namespace NotifierNotify
{
    internal class Program
    {
        // task.Actions.Add(new ExecAction("notifier-notify.exe", $"{taskName} '{args[2]}'"));
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                MessageBox.Show("Not allowed to call externally");
                return;
            }
            using var ts = new TaskService();
            ts.RootFolder.DeleteTask(args[0]);
            MessageBox.Show(
                string.Join(" ", args[1..]),
                "Notifier",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification
            );
        }
    }
}
