using OpenTK.Graphics.OpenGL;

namespace Tomography
{
    public partial class Form1 : Form
    {
        private Bin bin = new Bin();
        private View view = new View();
        private bool loaded = false;
        private int currentLayer = 0;
        private bool useQuads = true; // false - текстура, true - квады

        private int frameCount = 0;
        private System.Diagnostics.Stopwatch fpsTimer = System.Diagnostics.Stopwatch.StartNew();

        private void UpdateFPS()
        {
            frameCount++;

            if (fpsTimer.Elapsed.TotalSeconds >= 1.0)
            {
                double fps = frameCount / fpsTimer.Elapsed.TotalSeconds;
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.Text = $"CT Visualizer (fps = {fps:0})";
                    }));
                }
                else
                {
                    this.Text = $"CT Visualizer (fps = {fps:0})";
                }

                frameCount = 0;
                fpsTimer.Restart();
            }
        }

        public Form1()
        {
            InitializeComponent();
            fpsTimer.Start();

            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 1;
            timer.Tick += (s, args) =>
            {
                glControl2.Invalidate();
            };
            timer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
            System.Windows.Forms.Application.ThreadException += (s, e) => { }; // Игнорировать ошибки UI
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                view.SetupView(glControl2.Width, glControl2.Height);
                loaded = true;

                trackBar1.Minimum = 0;
                trackBar1.Maximum = Bin.Z - 1; // Устанавливаем максимум на количество слоев минус 1 (так как индексация с 0)
                trackBar1.Value = 0;

                trackBarMin.Minimum = 0;
                trackBarMin.Maximum = 5000;
                trackBarMin.Value = 0;

                trackBarWidth.Minimum = 1;
                trackBarWidth.Maximum = 5000;
                trackBarWidth.Value = 2000;

                glControl2.Invalidate();
            }
        }

        bool needReload = false;
        private void glControl2_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                GL.Finish();

                if (needReload)
                {
                    if (useQuads)
                    {
                        // Для режима квадов не нужно генерировать текстуру
                    }
                    else
                    {
                        view.GenerateTextureImage(currentLayer);
                        view.Load2DTexture();
                    }
                    needReload = false;
                }

                if (useQuads)
                {
                    view.DrawQuads(currentLayer);
                }
                else
                {
                    view.DrawTexture();
                }

                GL.Flush();
                glControl2.SwapBuffers();

                UpdateFPS();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
            glControl2.Invalidate();
        }

        void Application_Idle(object sender, EventArgs e)
        {
            glControl2.Invalidate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            useQuads = checkBox1.Checked;
            if (useQuads) checkBox2.Checked = false;
            needReload = true;
            glControl2.Invalidate();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            useQuads = !checkBox2.Checked;
            if (checkBox2.Checked) checkBox1.Checked = false;
            needReload = true;
            glControl2.Invalidate();
        }

        private void trackBarMin_Scroll(object sender, EventArgs e)
        {
            view.SetTfMin(trackBarMin.Value);
            needReload = true;
            glControl2.Invalidate();
        }

        private void trackBarWidth_Scroll(object sender, EventArgs e)
        {
            view.SetTfWidth(trackBarWidth.Value);
            needReload = true;
            glControl2.Invalidate();
        }
    }
}
