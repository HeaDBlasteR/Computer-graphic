using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System;

namespace Ray_Tracing
{
    public partial class Form1 : Form
    {
        private View view;

        public Form1()
        {
            InitializeComponent();
            glControl1.Load += GLControl_Load;
            glControl1.Paint += GLControl_Paint;
        }

        private void GLControl_Load(object sender, EventArgs e)
        {
            view = new View();
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            view.InitShaders();
            GL.ClearColor(Color.Black);
        }

        private void GLControl_Paint(object sender, PaintEventArgs e)
        {
            glControl1.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            view.Draw();
            glControl1.SwapBuffers();
        }
    }
}