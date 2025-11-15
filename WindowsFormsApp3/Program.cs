using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp3.Journal;

namespace WindowsFormsApp3
{
    internal static class Program
    {
        private static readonly Properties.Settings AppSettiongs = Properties.Settings.Default;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetupDefaultSettings();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        // 설정 기본값 저장
        private static void SetupDefaultSettings()
        {
            if (string.IsNullOrWhiteSpace(AppSettiongs.BasePath))
            {
                Logger.Write($"BasePath가 비어있음 : {AppSettiongs.BasePath}");

                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                AppSettiongs.BasePath = path;
                AppSettiongs.Save();

                Logger.Write($"BasePath 기본값 설정 : {path}");
            }
        }
    }
}
