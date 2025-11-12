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
using static Lucene.Net.Util.Fst.Builder;
using static Lucene.Net.Util.Packed.PackedInt32s;
using Directory = System.IO.Directory;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private readonly string basePath = @"E:\강의";

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
            indexer = new Indexer(basePath);

            InitializeComponent();

            MainForm mainform = new MainForm();
            mainform.Show();

            //BackColor = Color.White;
            //TransparencyKey = Color.White;

            pdfWebView = new PdfWebView();
            AttachWebView();

            duplicateManagerView = new DuplicateManagerView(indexer);
        }

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

        private void QueryInputBox_TextChanged(object sender, EventArgs e)
        {
            // 검색 쿼리 입력 시 자동 검색
            flowLayoutPanel1.Controls.Clear();

            var topDocs = indexer.SearchQuery(QueryInputBox.Text, 10);
            if (topDocs == null) return;

            List<Document> docs = new List<Document>();
            for (int i = 0; i < topDocs.ScoreDocs.Length; i++)
            {
                docs.Add(indexer.SearchDocument(topDocs.ScoreDocs[i].Doc));
                //string title = doc.Get("title");
                //string absolutePath = doc.Get("path");
                //string path = absolutePath.Replace($"{basePath}\\", "");
                //int page = int.Parse(doc.Get("page"));

                //var searchItem = new SearchItem(title, absolutePath, path, page, flowLayoutPanel1);
                //searchItem.Click += SearchItem_Click;
                //flowLayoutPanel1.Controls.Add(searchItem);
            }

            DocumentGroup[] group = Indexer.GroupDocuments(docs.ToArray());
            foreach (var item in group)
            {
                string relativePath = item.Path.Replace($"{basePath}\\", "");
                var searchItem = new SearchItemControl(item.Title, item.Path, relativePath, item.Page, item.Matches, flowLayoutPanel1);
                searchItem.Click += SearchItem_Click;
                flowLayoutPanel1.Controls.Add(searchItem);
            }
        }

        private void SearchItem_Click(object sender, EventArgs e)
        {
            // 리스트 박스 선택 시 해당 파일 오픈
            SearchItemControl item = (SearchItemControl)sender;
            OpenPDFInApp(item);
        }

        // 주어진 PDF 파일을 앱 내에서 염
        // 직접 webview를 조작하여 open 시 잘못된 동작으로 PDF가 열리지 않음.
        // 무조건 이 메소드를 이용해서 열어야 함.
        private void OpenPDFInApp(SearchItemControl item)
        {
            FilenameLabel.Text = item.Name;

            pdfWebView.OpenPDFInApp(item);
        }

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

        private void OpenInNewWindowButton_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", pdfWebView.currentPdf.LocalPath.ToString());
         }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            indexer.Dispose();
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

        private void DetachButton_Click(object sender, EventArgs e)
        {
            if (WebViewIsDetached) AttachWebView();
            else DetachWebView();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            duplicateManagerView.ShowDialog();
        }
    }
}
