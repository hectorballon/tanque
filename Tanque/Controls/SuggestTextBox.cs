using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Tanque.UI.Controls
{
    public partial class SuggestTextBox : Control
    {
        private TextBox _suggestTextBox = new TextBox();
        private string _cueText = string.Empty;
        private Color _borderColor = Color.Green;

        public SuggestTextBox()
        {
            InitializeComponent();

            _suggestTextBox.BorderStyle = BorderStyle.None;
            this.Controls.Add(_suggestTextBox);
            ResizeRedraw = true;
        }

        [DefaultValue("")]
        public new string Text
        {
            get { return _suggestTextBox.Text; }
            set { _suggestTextBox.Text = value; }
        }

        [Description("Test displayed when empty"), Category("Appearance")]
        public string CueText
        {
            get { return _cueText; }
            set { _cueText = value; }
        }

        [Description("Color used for the rounded border"), Category("Appearance")]
        public Color BoderColor
        {
            get { return _borderColor; }
            set
            {
                if (value != _borderColor)
                {
                    // Unhook previous events
                    if (_borderColor != null)
                        this.PropertyChanged -= HandleButtonDataPropertyChanged;
                    // Set private field
                    _borderColor = value;
                    // Hook new events
                    if (_borderColor != null)
                        this.PropertyChanged += HandleButtonDataPropertyChanged;
                    // Immediate update  since we have a new ButtonData object
                    if (_borderColor != null)
                        Update();
                }
            }
        }

        int r = 7;

        [Category(" "), Description(" 。")]
        public int R
        {
            get { return r; }
            set { r = value; }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            int w = R * 2;

            base.OnPaintBackground(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(0, 0, w, w, 180, 90);
                path.AddArc(this.Width - w - 1, 0, w, w, -90, 90);
                path.AddArc(this.Width - w - 1, this.Height - w - 1, w, w, 0, 90);
                path.AddArc(0, this.Height - w - 1, w, w, 90, 90);
                path.CloseFigure();

                e.Graphics.FillPath(Brushes.White, path);
                using (Pen pen = new Pen(_borderColor))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _suggestTextBox.Left = Height / 2;
            _suggestTextBox.Top = Height / 2 - _suggestTextBox.Height / 2;
            _suggestTextBox.Width = this.Width - Height;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (!string.IsNullOrEmpty(_cueText.Trim()))
            {
                Global.SetCueText(this._suggestTextBox, _cueText);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandleButtonDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Handle change in ButtonData
            Update();
        }
    }
}
