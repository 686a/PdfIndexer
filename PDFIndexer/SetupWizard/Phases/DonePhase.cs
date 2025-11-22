using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFIndexer.SetupWizard.Phases
{
    internal class DonePhase : Phase
    {
        private Properties.Settings AppSettings = Properties.Settings.Default;

        private SetupWizardForm MainUI;

        private Label TitleLabel;
        private Label TextLabel;

        public DonePhase(SetupWizardForm mainUI)
        {
            MainUI = mainUI;
        }

        public override void NextButton_Click(object sender, EventArgs e)
        {
            // 설정 저장 -> 닫기 -> 메인 UI 오픈
            AppSettings.DoneSetupWizard = true;
            AppSettings.Save();

            // 닫기 경고 suppress
            MainUI.CloseWarning = false;

            // 재시작
            Application.Restart();
        }

        public override void PrevButton_Click(object sender, EventArgs e)
        {
            MainUI.PrevPhase();
        }

        public override void Setup()
        {
            TableLayoutPanel layout = MainUI.GetMainTableLayout();
            MainUI.ClearMainTable();

            MainUI.GetNextButton().Enabled = true;

            Initialize();

            // 메인 테이블 스타일
            layout.ColumnCount = 1;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            layout.RowCount = 4;
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            layout.Controls.Add(TitleLabel, 0, 1);
            layout.Controls.Add(TextLabel, 0, 2);
        }

        protected override void Initialize()
        {
            if (Initialized) return;

            TitleLabel = new Label
            {
                Text = "완료\n ",
                AutoSize = true,
                Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Malgun Gothic", 12F),
            };

            TextLabel = new Label
            {
                Text = "모든 설정이 완료되었습니다.",
                AutoSize = true,
                Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                //Font = new System.Drawing.Font("Malgun Gothic", 12F),
            };

            Initialized = true;
        }
    }
}
