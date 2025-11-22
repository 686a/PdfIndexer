using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFIndexer
{
    public partial class PdfWebView : Form
    {
        public Uri currentPdf;

        public PdfWebView()
        {
            InitializeComponent();
        }

        // 주어진 PDF 파일을 앱 내에서 염
        // 직접 webview를 조작하여 open 시 잘못된 동작으로 PDF가 열리지 않음.
        // 무조건 이 메소드를 이용해서 열어야 함.
        internal void OpenPDFInApp(string path, int page)
        {
            currentPdf = new Uri($"file://{path}#page={page}");
            webView.Source = currentPdf;
            webView.Visible = true;
        }

        private void webView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            // 초기 웹뷰 세팅
            // 불필요한 UI 요소 및 보안 향상 목적으로 기본 기능 제한

            webView.CoreWebView2.Settings.IsScriptEnabled = false;
            webView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            webView.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            webView.CoreWebView2.Settings.IsReputationCheckingRequired = false;

            // 웹뷰 pdf 뷰어 UI 관련
            webView.CoreWebView2.Settings.HiddenPdfToolbarItems =
                CoreWebView2PdfToolbarItems.FullScreen | CoreWebView2PdfToolbarItems.Save | CoreWebView2PdfToolbarItems.SaveAs | CoreWebView2PdfToolbarItems.MoreSettings;

            // 디버그 빌드에서만 devtools 허용
#if DEBUG
            webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
#else
            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
#endif

            webView.CoreWebView2.DocumentTitleChanged += (s, _) =>
            {
                Text = webView.CoreWebView2.DocumentTitle;
            };
        }

        private void webView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // 다른 페이지로 이동 방지
            // 예: 하이퍼링크, 스크립트 동작으로 인해 외부 페이지로 연결되는 문제를 방지
            if (Uri.Compare(new Uri(e.Uri), currentPdf, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.Ordinal) != 0)
            {
                e.Cancel = true;

                if (currentPdf != null)
                    webView.Source = currentPdf;
                else
                    webView.Visible = false;
            }
        }

        private void webView_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            // Fragment가 변경되어도 페이지가 바뀌지 않는 문제 해결
            if (!e.IsNewDocument)
            {
                webView.Reload();
            }
        }

        private void webView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
        }
    }
}
