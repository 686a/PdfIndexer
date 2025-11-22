using PDFIndexer.Journal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

public delegate void ItemClick(string title, string path, int page);

namespace PDFIndexer
{
    internal partial class SearchItemControl : Button
    {
        public string Title { get; set; }
        public string AbsolutePath { get; set; }
        public string Path { get; set; }
        public int MatchPages { get; set; }
        public int[] Pages { get; set; }
        public new FlowLayoutPanel Parent { get; set; }

        private bool IsLoad = false;

        // Children
        private Label titleLabel;
        private Label pathLabel;
        private Label pageLabel;
        private Label matchesLabel;
        private Button MoreButton;
        private FlowLayoutPanel ResultLayout;

        private bool Expanded = false;

        public event ItemClick OnItemClick;

        public SearchItemControl()
        {
            ApplyStyle();
        }

        public SearchItemControl(FlowLayoutPanel parent)
        {
            Height = 0;

            ApplyStyle();
            ApplyParent(parent);
        }

        // FlowLayoutPanel의 사이즈가 변경되면 컨트롤 사이즈도 같이 변경
        private void Parent_ClientSizeChanged(object sender, EventArgs e)
        {
            Width = Parent.ClientSize.Width;
        }

        public SearchItemControl(string title, string absolutePath, string path, int matchPages, DocumentGroup group, FlowLayoutPanel parent)
        {
            ApplyStyle();

            Title = title;
            AbsolutePath = absolutePath;
            Path = path;
            MatchPages = matchPages;
            Pages = group.Documents.Keys.ToArray();
            Array.Sort(Pages);
            Parent = parent;

            ApplyParent(parent);
            AutoSize = true;

            //CreateItems();
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            CreateItems();
        }

        // 컨트롤 사이즈 변경 시 자식 컨트롤의 위치도 업데이트
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (IsLoad)
            {
                int baseHeight = Expanded ? 70 : Height;
                //MaximumSize = new Size(ClientSize.Width - SystemInformation.VerticalScrollBarWidth, MaximumSize.Height);
                //Width = ClientSize.Width - SystemInformation.VerticalScrollBarWidth;

                titleLabel.Location = new Point(32, baseHeight / 2 - titleLabel.Height);
                pathLabel.Location = new Point(32, baseHeight / 2);

                MoreButton.Location = new Point(0, 0);
                ResultLayout.Location = new Point(0, 70);
                ResultLayout.Width = Width;
                var resultLayoutChild = ResultLayout.Controls[0];
                if (resultLayoutChild != null)
                {
                    resultLayoutChild.Width = ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
                }
            }
        }

        private void ApplyStyle()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Margin = Padding.Empty;
            Padding = Padding.Empty;
        }

        private void ApplyParent(FlowLayoutPanel parent)
        {
            Parent = parent;
            Width = Parent.ClientSize.Width;

            Parent.ClientSizeChanged += Parent_ClientSizeChanged;
        }

        private void CreateItems()
        {
            titleLabel = new Label();
            titleLabel.Text = Title;
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font(
                "맑은 고딕",
                titleLabel.Font.Size,
                FontStyle.Bold,
                titleLabel.Font.Unit
            );
            titleLabel.BackColor = Color.Transparent;
            PassthroughEvents(titleLabel);
            Controls.Add(titleLabel);

            pathLabel = new Label();
            pathLabel.Text = Path;
            pathLabel.AutoSize = true;
            pathLabel.BackColor = Color.Transparent;
            PassthroughEvents(pathLabel);
            Controls.Add(pathLabel);

            // 왼쪽 세부 내용 열기 버튼
            MoreButton = new Button()
            {
                Text = "▶",
                Width = 36,
                Height = 70,
                AutoSize = false,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
            };
            MoreButton.FlatAppearance.BorderSize = 0;
            MoreButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            MoreButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            PassthroughEvents(MoreButton, true);
            Controls.Add(MoreButton);
            MoreButton.Click += (sender, e) =>
            {
                Expand(!Expanded);
            };

            ResultLayout = new FlowLayoutPanel()
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(0, 70),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = true,
                Height = 1,
                AutoSize = true,
            };
            Controls.Add(ResultLayout);

            for (int i = 0; i < Math.Min(5, Pages.Length); i++)
            {
                var page = Pages[i];
                ResultLayout.Controls.Add(CreatePageButton(page));
            }

            // Add show more button
            if (Pages.Length > 5)
            {
                var showAllButton = new Button()
                {
                    Text = $"결과 {Pages.Length}개 모두 보기",
                    AutoSize = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    BackColor = base.BackColor
                };
                showAllButton.Click += (sender, e) =>
                {
                    ResultLayout.Controls.Remove(showAllButton);

                    for (int i = 5; i < Pages.Length; i++)
                    {
                        var page = Pages[i];
                        ResultLayout.Controls.Add(CreatePageButton(page));
                    }

                    Height = 70 + ResultLayout.Height;
                };
                ResultLayout.Controls.Add(showAllButton);
            }

            IsLoad = true;
        }

        private Button CreatePageButton(int page)
        {
            var pageButton = new Button()
            {
                FlatStyle = FlatStyle.Flat,
                Text = $"{page} page",
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty,
                Padding = new Padding(16, 0, 16, 0),
            };
            pageButton.FlatAppearance.BorderSize = 0;

            pageButton.Click += (sender, e) =>
            {
                OnItemClick(Title, AbsolutePath, page);
            };

            return pageButton;
        }

        private void PassthroughEvents(Control control, bool withoutInput = false)
        {
            control.MouseHover += (_, e) => OnMouseHover(e);
            control.MouseEnter += (_, e) => OnMouseEnter(e);
            control.MouseLeave += (_, e) => OnMouseLeave(e);
            control.MouseMove += (_, e) => OnMouseMove(e);
            control.GotFocus += (s, e) => OnGotFocus(e);
            control.LostFocus += (s, e) => OnLostFocus(e);

            if (!withoutInput)
            {
                control.MouseClick += (s, e) => OnMouseClick(e);
                control.MouseDoubleClick += (s, e) => OnMouseDoubleClick(e);
                control.MouseDown += (s, e) => OnMouseDown(e);
                control.MouseUp += (s, e) => OnMouseUp(e);
                control.MouseClick += (s, e) => OnMouseClick(e);
            }
        }

        public override string Text => $"\n\n\n\n";

        public override string ToString()
        {
            return $"{Title} - \n{MatchPages} page";
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (!Expanded)
            {
                Expand(true);

                OnItemClick(Title, AbsolutePath, Pages[0]);
            } else
            {
                Expand(false);
            }
        }

        private void Expand(bool expand)
        {
            Expanded = expand;
            Height = expand ? 70 + ResultLayout.Height : 70;
            MoreButton.Text = expand ? "▼" : "▶";
            BackColor = expand ? Color.FromArgb(255, 220, 220, 220) : Color.Transparent;
        }
    }
}
