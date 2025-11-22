using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFIndexer.SetupWizard.Phases
{
    internal class ChoosePathPhase : Phase
    {
        private Properties.Settings AppSettings = Properties.Settings.Default;

        private SetupWizardForm MainUI;

        private Label TitleLabel;
        private TableLayoutPanel InputPanel;
        private FolderBrowserDialog SelectFolderDialog;

        private TextBox PathTextBox;

        public ChoosePathPhase(SetupWizardForm mainUI)
        {
            this.MainUI = mainUI;
        }

        public override void NextButton_Click(object sender, EventArgs e)
        {
            // 혹시나 만약에 없다면
            Directory.CreateDirectory(SelectFolderDialog.SelectedPath);

            // 설정 업데이트
            AppSettings.BasePath = SelectFolderDialog.SelectedPath;

            MainUI.NextPhase();
        }

        public override void PrevButton_Click(object sender, EventArgs e)
        {
            MainUI.PrevPhase();
        }

        public override void Setup()
        {
            TableLayoutPanel layout = MainUI.GetMainTableLayout();
            MainUI.ClearMainTable();

            Initialize();

            layout.ColumnCount = 1;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            layout.Controls.Add(TitleLabel, 0, 0);
            layout.Controls.Add(InputPanel, 0, 1);
        }

        protected override void Initialize()
        {
            if (Initialized) return;

            // 메인 텍스트
            TitleLabel = new Label()
            {
                Text = "인덱싱 경로를 지정해주세요",
                Font = new System.Drawing.Font("Malgun Gothic", 12F),
                AutoSize = true
            };

            // 폴더 선택 패널
            InputPanel = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = 1,
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left,
            };
            InputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            InputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // 폴더 선택 dialog
            SelectFolderDialog = new FolderBrowserDialog();
            SelectFolderDialog.SelectedPath = AppSettings.BasePath;

            // 경로 표시 텍스트 박스
            PathTextBox = new TextBox()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = AppSettings.BasePath,
            };
            InputPanel.Controls.Add(PathTextBox, 0, 0);

            Button SelectFolderButton = new Button()
            {
                Text = "폴더 선택",
            };
            InputPanel.Controls.Add(SelectFolderButton, 1, 0);

            PathTextBox.Click += SelectFolderButton_Click;
            SelectFolderButton.Click += SelectFolderButton_Click;

            Initialized = true;
        }

        private void SelectFolderButton_Click(object sender, EventArgs e)
        {
            DialogResult result = SelectFolderDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                var dir = SelectFolderDialog.SelectedPath;
                PathTextBox.Text = dir;
            }
        }
    }
}
