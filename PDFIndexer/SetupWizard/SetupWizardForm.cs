using PDFIndexer.SetupWizard.Phases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFIndexer.SetupWizard
{
    public partial class SetupWizardForm : Form
    {
        private Phase CurrentPhase;
        private int CurrentPhaseIndex;
        private Phase[] Phases;

        public bool CloseWarning = true;

        public SetupWizardForm()
        {
#if DEBUG
            new DebugForm().Show();
#endif

            InitializeComponent();

            Phases = new Phase[] {
                new WelcomePhase(this),
                new ChoosePathPhase(this),
                new DonePhase(this),
            };

            LoadPhase(0);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (CurrentPhase != null)
            {
                CurrentPhase.NextButton_Click(sender, e);
            }
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            if (CurrentPhase != null)
            {
                CurrentPhase.PrevButton_Click(sender, e);
            }
        }

        private void DefaultSetup()
        {
            NextButton.Enabled = HasNextPhase();
            PrevButton.Enabled = HasPrevPhase();

            ClearMainTable();

            MainTableLayout.ColumnCount = 1;
            MainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        }

        private void LoadPhase(int index)
        {
            CurrentPhase = Phases[index];
            CurrentPhaseIndex = index;

            DefaultSetup();
            CurrentPhase.Setup();
        }

        public void NextPhase()
        {
            // 다음 페이즈 없음
            if (Phases.Length == CurrentPhaseIndex + 1) return;

            LoadPhase(++CurrentPhaseIndex);
        }

        public void PrevPhase()
        {
            // 다음 페이즈 없음
            if (CurrentPhaseIndex == 0) return;

            LoadPhase(--CurrentPhaseIndex);
        }

        public bool HasNextPhase()
        {
            return Phases.Length != CurrentPhaseIndex + 1;
        }

        public bool HasPrevPhase()
        {
            return CurrentPhaseIndex != 0;
        }

        public TableLayoutPanel GetMainTableLayout()
        {
            return MainTableLayout;
        }

        public void ClearMainTable()
        {
            MainTableLayout.Controls.Clear();
            MainTableLayout.ColumnStyles.Clear();
            MainTableLayout.RowStyles.Clear();
        }

        public Button GetNextButton()
        {
            return NextButton;
        }

        public Button GetPrevButton()
        {
            return PrevButton;
        }

        private void SetupWizardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseWarning)
            {
                var result = MessageBox.Show(
                    "정말로 닫으시겠습니까?\n\n지금 닫으면 변경사항이 저장되지 않습니다.",
                    "정말로 닫으시겠습니까?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
