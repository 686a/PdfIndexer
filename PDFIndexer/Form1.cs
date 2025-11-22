using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using PDFIndexer.Journal;
using static Lucene.Net.Util.Fst.Builder;
using static Lucene.Net.Util.Packed.PackedInt32s;
using Directory = System.IO.Directory;

namespace PDFIndexer
{
    public partial class Form1 : Form
    {
        private static readonly Properties.Settings AppSettiongs = Properties.Settings.Default;

        private string basePath
        {
            get { return AppSettiongs.BasePath; }
        }

        private readonly List<string> pdfs = new List<string>();

        // private Uri currentPdf;

        private Indexer indexer;

        private bool _IsIndexing = false;
        private bool IsIndexing
        {
            get { return _IsIndexing; }
            set
            {
                _IsIndexing = value;

                //listBox1.Enabled = !value;
                OpenInNewWindowButton.Enabled = !value;
                IndexAllButton.Enabled = !value;
                QueryInputBox.Enabled = !value;
            }
        }

        PdfWebView pdfWebView;
        bool _WebViewIsDetached = false;
        bool WebViewIsDetached
        {
            get { return _WebViewIsDetached; }
            set
            {
                _WebViewIsDetached = value;
                if (value) DetachButton.Image = Properties.Resources.OpenInNewDownIcon;
                else DetachButton.Image = Properties.Resources.OpenInNewIcon;
            }
        }

        DuplicateManagerView duplicateManagerView;

        public Form1()
        {
#if DEBUG
            new DebugForm().Show();
#endif

            indexer = new Indexer(basePath);

            InitializeComponent();

            MainForm mainform = new MainForm();
            mainform.Show();

            //BackColor = Color.White;
            //TransparencyKey = Color.White;

            pdfWebView = new PdfWebView();
            pdfWebView.FormClosing += PdfWebView_FormClosing;
            AttachWebView();

            duplicateManagerView = new DuplicateManagerView(indexer);
        }

        #region 웹뷰 관련
        private void AttachWebView()
        {
            pdfWebView.TopLevel = false;
            pdfWebView.FormBorderStyle = FormBorderStyle.None;
            pdfWebView.Location = WebViewVirtualPanel.Location;
            pdfWebView.Size = WebViewVirtualPanel.ClientSize;

            Controls.Add(pdfWebView);
            if (!pdfWebView.Visible) pdfWebView.Show();

            WebViewIsDetached = false;
        }

        private void DetachWebView()
        {
            Controls.Remove(pdfWebView);
            pdfWebView.TopLevel = true;
            pdfWebView.FormBorderStyle = FormBorderStyle.Sizable;

            WebViewIsDetached = true;
        }

        private void PdfWebView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WebViewIsDetached)
            {
                e.Cancel = true;
                AttachWebView();
            }
        }

        // 주어진 PDF 파일을 앱 내에서 염
        // 직접 webview를 조작하여 open 시 잘못된 동작으로 PDF가 열리지 않음.
        // 무조건 이 메소드를 이용해서 열어야 함.
        private void OpenPDFInApp(string title, string path, int page)
        {
            noFileLabel.Visible = false;

            FilenameLabel.Text = title;

            pdfWebView.OpenPDFInApp(path, page);
        }

        private void DetachButton_Click(object sender, EventArgs e)
        {
            if (WebViewIsDetached) AttachWebView();
            else DetachWebView();
        }
        #endregion 웹뷰 관련

        #region 인덱서 관련
        private void FindAllPdfFiles(string path, bool recursive = false)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (file.EndsWith(".pdf"))
                {
                    pdfs.Add(file);
                }
            }

            if (recursive)
            {
                var dirs = Directory.GetDirectories(path);
                foreach (var dir in dirs)
                {
                    FindAllPdfFiles(dir, true);
                }
            }
        }

        private async void IndexAll()
        {
            IsIndexing = true;

            await Task.Run(() =>
            {
                pdfs.Clear();
                FindAllPdfFiles(basePath, true);
                indexer.IndexPdfs(pdfs.ToArray());
            });

            IsIndexing = false;
        }
        #endregion 인덱서 관련

        #region 검색 관련 이벤트
        private void QueryInputBox_TextChanged(object sender, EventArgs e)
        {
            // 검색 쿼리 입력 시 자동 검색
            flowLayoutPanel1.Controls.Clear();

var query = QueryInputBox.Text;
            var topDocs = indexer.SearchQuery(query, 50);
            if (topDocs == null) return;

            var groups = Indexer.GroupDocuments(indexer, topDocs.ScoreDocs);
            foreach (var group in groups)
            {
                string relativePath = group.Path.Replace($"{basePath}\\", "").Replace($"\\{group.Title}.pdf", "");
                var searchItem = new SearchItemControl(group.Title, group.Path, relativePath, group.MatchPages, group, flowLayoutPanel1);
                searchItem.OnItemClick += SearchItem_OnItemClick;
                flowLayoutPanel1.Controls.Add(searchItem);
            }
        }

        private void SearchItem_OnItemClick(string title, string path, int page)
        {
                        OpenPDFInApp(title, path, page);
        }
        #endregion 검색 관련 이벤트

        #region 인덱서 관련 이벤트
        private void IndexAllButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "정말로 모두 새로 인덱싱하겠습니까?\n\n주의: 이 작업은 오래걸릴 수도 있습니다.",
                "모두 새로 인덱싱",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            IndexAll();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 인덱서 정리
            indexer.Dispose();
        }
        #endregion 인덱서 관련 이벤트

        private void OpenInNewWindowButton_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", pdfWebView.currentPdf.LocalPath.ToString());
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            IsIndexing = true;

            await Task.Run(() =>
            {
                FindAllPdfFiles(basePath, true);
                var missing = Indexer.GetMissingPdfs(indexer, pdfs.ToArray());
                Debug.WriteLine("Missing:");
                foreach (var m in missing) Debug.WriteLine(m);
            });

            IsIndexing = false;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = indexer.ready ? "Ready" : "Not Ready";
            indexer.OnReady += () =>
            {
                label1.BeginInvoke((MethodInvoker)delegate
                {
                    label1.Text = indexer.ready ? "Ready" : "Not Ready";
                });
            };

            await Task.Run(() => Thread.Sleep(100));

            if (!indexer.ready)
            {
                var result = MessageBox.Show("인덱싱이 없습니다.\n\n인덱싱을 지금 시작할까요?", "환영합니다", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    IndexAll();
                }
            }
        }

        private void DuplicateMangerButton_Click(object sender, EventArgs e)
        {
            duplicateManagerView.ShowDialog();
        }
    }
}
