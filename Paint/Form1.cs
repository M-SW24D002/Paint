
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace Paint
{
    public partial class Form1 : Form
    {
        Bitmap drawingBitmap;
        Graphics graphics;

        bool isDrawing = false;
        Point lastPoint;
        Point startPoint;
        Point currentPoint;
        Bitmap previewBitmap;

        Color currentColor = Color.Black;
        int brushSize = 5;

        string currentTool = "Pencil";


        public Form1()
        {
            InitializeComponent();

            drawingBitmap = new Bitmap(960, 520);
            graphics = Graphics.FromImage(drawingBitmap);

            graphics.Clear(Color.White);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            pnlCanvas.Paint += pnlCanvas_Paint;
            pnlCanvas.MouseDown += pnlCanvas_MouseDown;
            pnlCanvas.MouseMove += pnlCanvas_MouseMove;
            pnlCanvas.MouseUp += pnlCanvas_MouseUp;

            btnPencil.Click += btnPencil_Click;
            btnEraser.Click += btnEraser_Click;
            btnColor.Click += btnColor_Click;
            btnClear.Click += btnClear_Click;
            btnSave.Click += btnSave_Click;
            btnRectangle.Click += btnRectangle_Click;
            btnCircle.Click += btnCircle_Click;

            trackBrush.Scroll += trackBrush_Scroll;
        }

        // Зургийг Canvas дээр харуулах
        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(drawingBitmap, 0, 0);

            if (isDrawing &&
                (currentTool == "Rectangle" || currentTool == "Circle"))
            {
                int x = Math.Min(startPoint.X, currentPoint.X);
                int y = Math.Min(startPoint.Y, currentPoint.Y);

                int width = Math.Abs(currentPoint.X - startPoint.X);
                int height = Math.Abs(currentPoint.Y - startPoint.Y);

                using (Pen pen = new Pen(currentColor, brushSize))
                {
                    if (currentTool == "Rectangle")
                        e.Graphics.DrawRectangle(pen, x, y, width, height);
                    else
                        e.Graphics.DrawEllipse(pen, x, y, width, height);
                }
            }
        }

        // Хулгана дарах
        private void pnlCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                lastPoint = e.Location;
                startPoint = e.Location;
                currentPoint = e.Location;
            }
        }

        // Хулгана хөдөлгөх
        private void pnlCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            if (currentTool == "Pencil" || currentTool == "Eraser")
            {
                Color color = currentTool == "Eraser"
                    ? Color.White : currentColor;

                int size = currentTool == "Eraser"
                    ? brushSize * 3 : brushSize;

                using (Pen pen = new Pen(color, size))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;

                    graphics.DrawLine(pen, lastPoint, e.Location);
                }

                lastPoint = e.Location;
                pnlCanvas.Invalidate();
            }
            else
            {
                currentPoint = e.Location;
                pnlCanvas.Invalidate();
            }
        }

        // Хулгана тавих
        private void pnlCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            currentPoint = e.Location;

            int x = Math.Min(startPoint.X, currentPoint.X);
            int y = Math.Min(startPoint.Y, currentPoint.Y);

            int width = Math.Abs(currentPoint.X - startPoint.X);
            int height = Math.Abs(currentPoint.Y - startPoint.Y);

            using (Pen pen = new Pen(
                currentTool == "Eraser" ? Color.White : currentColor,
                brushSize))
            {
                if (currentTool == "Rectangle")
                    graphics.DrawRectangle(pen, x, y, width, height);

                else if (currentTool == "Circle")
                    graphics.DrawEllipse(pen, x, y, width, height);
            }

            isDrawing = false;
            pnlCanvas.Invalidate();
        }

        // Pencil
        private void btnPencil_Click(object sender, EventArgs e)
        {
            currentTool = "Pencil";
        }

        // Eraser
        private void btnEraser_Click(object sender, EventArgs e)
        {
            currentTool = "Eraser";
        }

        // Өнгө сонгох
        private void btnColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    currentColor = colorDialog.Color;
                }
            }
        }

        // Canvas цэвэрлэх
        private void btnClear_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            pnlCanvas.Invalidate();
        }

        // Brush size
        private void trackBrush_Scroll(object sender, EventArgs e)
        {
            brushSize = trackBrush.Value;
            lblSize.Text = "Size: " + brushSize;
        }

        // Зураг хадгалах
        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
                saveDialog.FileName = "MyDrawing";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    drawingBitmap.Save(saveDialog.FileName);

                    MessageBox.Show("Зураг амжилттай хадгалагдлаа!");
                }
            }
        }

        // Rectangle tool
        private void btnRectangle_Click(object sender, EventArgs e)
        {
            currentTool = "Rectangle";
        }

        // Circle tool
        private void btnCircle_Click(object sender, EventArgs e)
        {
            currentTool = "Circle";
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            graphics.Dispose();
            drawingBitmap.Dispose();

            base.OnFormClosed(e);
        }
    }
}
