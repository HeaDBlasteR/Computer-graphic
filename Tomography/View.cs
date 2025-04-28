using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace Tomography
{
    internal class View
    {
        private int tfMin = 0;
        private int tfWidth = 2000;

        public void SetupView(int width, int height)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.X, 0, Bin.Y, -1, 1);
            GL.Viewport(0, 0, width, height);
        }

        Color TransferFunction(short value)
        {
            int max = tfMin + tfWidth;
            int newVal = Math.Clamp((value - tfMin) * 255 / (max - tfMin), 0, 255);
            return Color.FromArgb(255, newVal, newVal, newVal);
        }

        public void SetTfMin(int min)
        {
            tfMin = min;
        }

        public void SetTfWidth(int width)
        {
            tfWidth = width;
        }

        public void DrawQuads(int layerNumber)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Отрисовка по горизонтальным полосам (строкам)
            for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
            {
                GL.Begin(PrimitiveType.QuadStrip);

                // Первая вершина в полосе
                short value = Bin.array[0 + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                GL.Color3(TransferFunction(value));
                GL.Vertex2(0, y_coord);

                // Вторая вершина в полосе (начало следующей строки)
                value = Bin.array[0 + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                GL.Color3(TransferFunction(value));
                GL.Vertex2(0, y_coord + 1);

                // Остальные вершины в полосе
                for (int x_coord = 1; x_coord < Bin.X; x_coord++)
                {
                    value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord, y_coord);

                    value = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord, y_coord + 1);
                }

                GL.End();
            }
        }

        Bitmap textureImage;
        int VBOtexture;
        public void Load2DTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

            BitmapData data = textureImage.LockBits(
                new System.Drawing.Rectangle(0, 0, textureImage.Width, textureImage.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, data.Scan0);

            textureImage.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            ErrorCode Er = GL.GetError();
            string str = Er.ToString();
        }

        public void GenerateTextureImage(int layerNumber)
        {
            textureImage = new Bitmap(Bin.X, Bin.Y);

            for (int i = 0; i < Bin.X; i++)
            {
                for (int j = 0; j < Bin.Y; j++)
                {
                    int pixelNumber = i + j * Bin.X + layerNumber * Bin.X * Bin.Y;
                    textureImage.SetPixel(i, j, TransferFunction(Bin.array[pixelNumber]));
                }
            }
        }
        public void DrawTexture()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);

            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);

            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.Y);

            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.X, Bin.Y);

            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Bin.X, 0);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }
    }
}
