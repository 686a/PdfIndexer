using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class IconTextButtonControl : Button
    {
        public Image Icon { get; set; }
        public string Content { get; set; }

        private int _IconSizeOffset;

        [DefaultValue(8)]
        public int IconSizeOffset
        {
            get { return _IconSizeOffset; }
            set
            {
                _IconSizeOffset = value;
            }
        }

        [DefaultValue(null)]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        private bool IsLoad = false;

        // Children
        private PictureBox pictureBox;
        private Label label;

        public IconTextButtonControl()
        {
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            CreateItems();
        }

        protected override void OnParentFontChanged(EventArgs e)
        {
            base.OnParentFontChanged(e);

            Font = Parent.Font;
        }

        private void CreateItems()
        {
            // Icon
            pictureBox = new PictureBox
            {
                Image = Icon,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
            };

            pictureBox.MouseHover += (_, e) => base.OnMouseHover(e);
            pictureBox.MouseEnter += (_, e) => base.OnMouseEnter(e);
            pictureBox.MouseLeave += (_, e) => base.OnMouseLeave(e);
            pictureBox.MouseMove += (_, e) => base.OnMouseMove(e);
            pictureBox.MouseClick += (s, e) => base.OnMouseClick(e);
            pictureBox.MouseDoubleClick += (s, e) => base.OnMouseDoubleClick(e);
            pictureBox.MouseDown += (s, e) => base.OnMouseDown(e);
            pictureBox.MouseUp += (s, e) => base.OnMouseUp(e);
            pictureBox.GotFocus += (s, e) => base.OnGotFocus(e);
            pictureBox.LostFocus += (s, e) => base.OnLostFocus(e);

            Controls.Add(pictureBox);

            // Label
            label = new Label
            {
                Text = Content,
                BackColor = Color.Transparent,
                AutoSize = true,
            };

            Controls.Add(label);

            IsLoad = true;
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            var iconSize = Math.Min(ClientSize.Width, ClientSize.Height) - 8;
            pictureBox.Size = new Size(iconSize, iconSize);

            // Get Boundary
            Size bound = new Size(0, 0);
            bound.Width = pictureBox.Width;
            bound.Height = pictureBox.Height;

            bound.Width += label.Width;
            bound.Height = Math.Max(pictureBox.Height, label.Height);

            // Set location from boundary
            Point center = new Point(Size.Width / 2, Size.Height / 2);

            Rectangle rect = new Rectangle(0, 0, 0, 0);
            rect.X = center.X - bound.Width / 2;
            rect.Y = center.Y - bound.Height / 2;
            rect.Width = bound.Width;
            rect.Height = bound.Height;

            pictureBox.Location = new Point(rect.X, rect.Y);
            label.Location = new Point(rect.X + pictureBox.Width, center.Y - label.Height / 2);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (IsLoad)
            {
                UpdateLayout();
            }
        }
    }
}
