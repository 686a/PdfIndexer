using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFIndexer.SetupWizard
{
    internal class WelcomePhase : Phase
    {
        private SetupWizardForm MainUI;

        private PictureBox LogoPictureBox;
        private Label AppNameLabel;
        private Label WelcomeLabel;

        public WelcomePhase(SetupWizardForm form)
        {
            this.MainUI = form;
        }

        public override void NextButton_Click(object sender, EventArgs e)
        {
            MainUI.NextPhase();
        }

        public override void PrevButton_Click(object sender, EventArgs e) { }

        public override void Setup()
        {
            TableLayoutPanel MainTableLayout = MainUI.GetMainTableLayout();
            MainUI.ClearMainTable();

            Initialize();

            // 메인 테이블 스타일
            MainTableLayout.ColumnCount = 1;
            MainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            MainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));

            MainTableLayout.RowCount = 4;
            MainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            MainTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            MainTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            MainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));

            MainTableLayout.Controls.Add(LogoPictureBox, 0, 0);
            MainTableLayout.Controls.Add(AppNameLabel, 0, 1);
            MainTableLayout.Controls.Add(WelcomeLabel, 0, 2);
        }

        protected override void Initialize()
        {
            if (Initialized) return;

            // 로고
            LogoPictureBox = new PictureBox();
            LogoPictureBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            // 앱 이름
            AppNameLabel = new Label()
            {
                Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true,
                Font = new System.Drawing.Font("Malgun Gothic", 12F),
                Text = "\nPDFIndexer\n ",
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            };

            // 환영 텍스트
            WelcomeLabel = new Label()
            {
                Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right),
                AutoSize = true,
                Text = "환영합니다",
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            };

            Initialized = true;
        }
    }
}
