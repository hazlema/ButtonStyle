using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ButtonStyle {
    class Rounded : Button {
        private Bitmap[] Background;

        public Rounded() {
            this.BackColor = Color.Transparent;
            this.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.FlatAppearance.MouseOverBackColor = Color.Transparent;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.ForeColor = Color.White;
            this.Cursor = Cursors.Hand;
        }

        public int Rounding { get; set; } = 10;
        public Color HoverGradientStart { get; set; } = Color.FromArgb(255, 90, 90, 80);
        public Color NormalGradientStart { get; set; } = Color.FromArgb(255, 60, 60, 60);
        public Color GradientEnd { get; set; } = Color.FromArgb(255, 20, 20, 20);

        private void PrePaint(Graphics g, Color[] GradEnd, Color GradStart) {
            using (GraphicsPath Path = new GraphicsPath()) {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Points
                Rectangle[] Corners = new Rectangle[] {
                new Rectangle(0, 0, Rounding, Rounding),
                new Rectangle(Width - Rounding, 0, Rounding, Rounding),
                new Rectangle(Width - Rounding, Height - Rounding, Rounding, Rounding),
                new Rectangle(0, Height - Rounding, Rounding, Rounding)
            };

                Point[] Lines = new Point[] {
                new Point(Rounding, 0), new Point(Width - Rounding, 0),
                new Point(Width, Rounding), new Point(Width, Height - Rounding),
                new Point(Width - Rounding, Height), new Point(Rounding, Height)
            };

                // Create Path
                Path.StartFigure();
                Path.AddArc(Corners[0], 180, 90);
                Path.AddLine(Lines[0], Lines[1]);
                Path.AddArc(Corners[1], -90, 90);
                Path.AddLine(Lines[2], Lines[3]);
                Path.AddArc(Corners[2], 0, 90);
                Path.AddLine(Lines[4], Lines[5]);
                Path.AddArc(Corners[3], 90, 90);
                Path.CloseFigure();

                // Gradient
                PathGradientBrush GBrush = new PathGradientBrush(Path);
                GBrush.CenterColor = GradStart;
                GBrush.SurroundColors = GradEnd;
                GBrush.CenterPoint = new Point(Width, Height);

                g.FillPath(GBrush, Path);
            }
        }

        public void buildButtonBackgrounds() {
            // Init Backgrounds
            Background = new Bitmap[] {
                new Bitmap(Width, Height, PixelFormat.Format32bppPArgb),
                new Bitmap(Width, Height, PixelFormat.Format32bppPArgb)
            };

            // Normal
            using (Graphics g = Graphics.FromImage(Background[0])) {
                PrePaint(g, 
                    new Color[] { NormalGradientStart },
                    GradientEnd
                );

            }

            // Hover
            using (Graphics g = Graphics.FromImage(Background[1])) {
                PrePaint(g,
                    new Color[] { HoverGradientStart },
                    GradientEnd
                );
            }

            this.BackgroundImage = Background[0];
        }

        protected override void OnMouseLeave(EventArgs e) {
            if (Background[0]?.Size != Size)
                buildButtonBackgrounds();

            this.BackgroundImage = Background[0];
            base.OnMouseLeave(e);
        }

        protected override void OnMouseEnter(EventArgs e) {
            if (Background[1]?.Size != Size)
                buildButtonBackgrounds();

            this.BackgroundImage = Background[1];
            base.OnMouseEnter(e);
        }

        protected override void OnParentVisibleChanged(EventArgs e) {
            buildButtonBackgrounds();
            OnMouseLeave(e);
            base.OnParentVisibleChanged(e);
        }

        protected override void OnResize(EventArgs e) {
            buildButtonBackgrounds();
            base.OnResize(e);
        }
    }
}