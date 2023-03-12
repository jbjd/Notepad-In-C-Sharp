using System;
using System.Windows.Forms;

namespace Notepad
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new Form1(args.Length > 1 ? args[1] : ""));
        }
    }
}
