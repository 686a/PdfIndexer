using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PDFIndexer.Journal;
using PDFIndexer.SetupWizard;

namespace PDFIndexer
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
            Logger.Write(JournalLevel.Info, "프로그램 진입점");

            /**
             * 초기 실행 -> run setup wizard
             * 기사용자 no or broken configuration -> run setup wizard
             * 기사용자 -> 일반 실행
             */

            SetupDefaultSettings();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 초기 실행 시 Setup wizard 실행
            if (!AppSettiongs.DoneSetupWizard)
            {
                Logger.Write($"Settings.DoneSetupWizard : false --> Setup wizard 실행");
                Application.Run(new SetupWizardForm());
            }
            else
            {
                Logger.Write($"메인 UI 실행");
                Application.Run(new Form1());
            }
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
