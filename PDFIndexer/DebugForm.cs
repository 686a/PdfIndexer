using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PDFIndexer.Journal;

namespace PDFIndexer
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();
        }

        private void AppendLog(JournalMessage log)
        {
            if (!Visible) return;
            logListView.BeginInvoke((MethodInvoker) delegate
            {
                ListViewItem item = new ListViewItem(log.Level.ToString());
                item.SubItems.Add(log.Message);
                logListView.Items.Add(item);
            });
        }

        private void Logger_OnMessage(JournalMessage log)
        {
            AppendLog(log);
        }

        private void UpdateListViewWidth()
        {
            int total = logListView.ClientSize.Width;
            int width = total - logListView.Columns[0].Width;
            logListView.Columns[1].Width = width;
        }

        private void logListView_Resize(object sender, EventArgs e)
        {
            UpdateListViewWidth();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            Logger.Clear();
            logListView.Items.Clear();
        }

        private void DebugForm_Load(object sender, EventArgs e)
        {
            // 리스트 칼럼 사이즈 업데이트
            UpdateListViewWidth();

            // 로그 레벨 콤보박스 setup
            var levels = Enum.GetValues(typeof(JournalLevel));
            foreach (JournalLevel level in levels)
            {
                logLevelComboBox.Items.Add(level.ToString());
            }
            logLevelComboBox.SelectedIndex = 0;

            // 폼 로드 이전 로그 불러오기
            var logs = Logger.RetrieveRecentLogs();
            foreach (var log in logs)
            {
                AppendLog(log);
            }

            // 이벤트 리스닝
            Logger.OnMessage += Logger_OnMessage;
        }
    }
}
