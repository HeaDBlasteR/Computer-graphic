using System;
using System.Drawing;
using System.ComponentModel;

namespace Image_Processing
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);
        public virtual Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }

    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R,
                                              255 - sourceColor.G,
                                              255 - sourceColor.B);
            return resultColor;
        }
    }

    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }

            return Color.FromArgb(
            Clamp((int)resultR, 0, 255),
            Clamp((int)resultG, 0, 255),
            Clamp((int)resultB, 0, 255)
            );
        }
    }
    
    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
        }
    }

    class GaussianFilter : MatrixFilter
    {
        public void createGaussianKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0;
            for (int i = -radius; i <= radius; i++)
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }

        public GaussianFilter()
        {
            createGaussianKernel(3, 2);
        }
    }

    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);

            double intensity = 0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B;
            int grayValue = (int)Math.Round(intensity);

            grayValue = Clamp(grayValue, 0, 255);

            return Color.FromArgb(grayValue, grayValue, grayValue);
        }
    }

    class SepiaFilter : Filters
    {
        private readonly int k;
        public SepiaFilter(int k = 30)
        {
            this.k = k;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);

            double intensity = 0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B;

            int newR = (int)(intensity + 2 * k);
            int newG = (int)(intensity + 0.5 * k);
            int newB = (int)(intensity - 1 * k);

            return Color.FromArgb(
                Clamp(newR, 0, 255),
                Clamp(newG, 0, 255),
                Clamp(newB, 0, 255)
            );
        }
    }

    class BrightnessFilter : Filters
    {
        private int brightnessValue;

        public BrightnessFilter(int brightness)
        {
            brightnessValue = brightness;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);

            int r = Clamp(sourceColor.R + brightnessValue, 0, 255);
            int g = Clamp(sourceColor.G + brightnessValue, 0, 255);
            int b = Clamp(sourceColor.B + brightnessValue, 0, 255);

            return Color.FromArgb(r, g, b);
        }
    }

    class SobelFilter : MatrixFilter
    {
        protected float[,] kernelX = null;
        protected float[,] kernelY = null;

        public SobelFilter()
        {
            kernelX = new float[,] {
                {-1, 0, 1},
                {-2, 0, 2},
                {-1, 0, 1}
            };

            kernelY = new float[,] {
                {-1, -2, -1},
                { 0,  0,  0},
                { 1,  2,  1}
            };
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernelX.GetLength(0) / 2;
            int radiusY = kernelX.GetLength(1) / 2;

            float resultRX = 0, resultGX = 0, resultBX = 0;
            float resultRY = 0, resultGY = 0, resultBY = 0;

            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultRX += neighborColor.R * kernelX[k + radiusX, l + radiusY];
                    resultGX += neighborColor.G * kernelX[k + radiusX, l + radiusY];
                    resultBX += neighborColor.B * kernelX[k + radiusX, l + radiusY];

                    resultRY += neighborColor.R * kernelY[k + radiusX, l + radiusY];
                    resultGY += neighborColor.G * kernelY[k + radiusX, l + radiusY];
                    resultBY += neighborColor.B * kernelY[k + radiusX, l + radiusY];
                }
            }

            int resultR = (int)Math.Sqrt(resultRX * resultRX + resultRY * resultRY);
            int resultG = (int)Math.Sqrt(resultGX * resultGX + resultGY * resultGY);
            int resultB = (int)Math.Sqrt(resultBX * resultBX + resultBY * resultBY);

            return Color.FromArgb(
                Clamp(resultR, 0, 255),
                Clamp(resultG, 0, 255),
                Clamp(resultB, 0, 255)
            );
        }
    }

    class SharpnessFilter : MatrixFilter
    {
        public SharpnessFilter()
        {
            kernel = new float[,] {
                { 0, -1,  0 },
                {-1,  5, -1 },
                { 0, -1,  0 }
            };
        }
    }

    class PerfectReflectorFilter : Filters
    {
        private int maxR = 0, maxG = 0, maxB = 0;
        private void FindMaxValues(Bitmap sourceImage)
        {
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color pixel = sourceImage.GetPixel(i, j);
                    if (pixel.R > maxR) maxR = pixel.R;
                    if (pixel.G > maxG) maxG = pixel.G;
                    if (pixel.B > maxB) maxB = pixel.B;
                }
            }

            // Защита от деления на ноль (если изображение полностью черное)
            if (maxR == 0) maxR = 1;
            if (maxG == 0) maxG = 1;
            if (maxB == 0) maxB = 1;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color pixel = sourceImage.GetPixel(x, y);

            int newR = Clamp((int)(pixel.R * 255.0f / maxR), 0, 255);
            int newG = Clamp((int)(pixel.G * 255.0f / maxG), 0, 255);
            int newB = Clamp((int)(pixel.B * 255.0f / maxB), 0, 255);

            return Color.FromArgb(newR, newG, newB);
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            FindMaxValues(sourceImage);
            return base.processImage(sourceImage, worker);
        }
    }

    class LinearHistogramStretchFilter : Filters
    {
        private int minIntensity = 255;
        private int maxIntensity = 0;
        private void FindMinMaxValues(Bitmap sourceImage)
        {
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color pixel = sourceImage.GetPixel(i, j);
                    int intensity = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                    if (intensity < minIntensity) minIntensity = intensity;
                    if (intensity > maxIntensity) maxIntensity = intensity;
                }
            }
            if (minIntensity == maxIntensity) maxIntensity = minIntensity + 1;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color pixel = sourceImage.GetPixel(x, y);
            int intensity = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);

            int stretchedIntensity = (int)((intensity - minIntensity) * 255.0 / (maxIntensity - minIntensity));
            stretchedIntensity = Clamp(stretchedIntensity, 0, 255);

            float scale = stretchedIntensity / (float)intensity;
            int r = Clamp((int)(pixel.R * scale), 0, 255);
            int g = Clamp((int)(pixel.G * scale), 0, 255);
            int b = Clamp((int)(pixel.B * scale), 0, 255);

            return Color.FromArgb(r, g, b);
        }
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            FindMinMaxValues(sourceImage);
            return base.processImage(sourceImage, worker);
        }
    }

    class EmbossFilter : MatrixFilter
    {
        public EmbossFilter()
        {
            kernel = new float[,] {
            { -1, -1,  0 },
            { -1,  0,  1 },
            {  0,  1,  1 }
        };
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color baseColor = base.calculateNewPixelColor(sourceImage, x, y);

            int r = Clamp(baseColor.R + 127, 0, 255);
            int g = Clamp(baseColor.G + 127, 0, 255);
            int b = Clamp(baseColor.B + 127, 0, 255);

            return Color.FromArgb(r, g, b);
        }
    }

    class EdgeDetectionFilter : MatrixFilter
    {
        public EdgeDetectionFilter()
        {
            kernel = new float[,] {
            {  0, -1,  0 },
            { -1,  4, -1 },
            {  0, -1,  0 }
        };
        }
    }

    abstract class MorphologyFilter : Filters
    {
        protected bool[,] kernel;
        protected int kernelSize;

        public bool[,] StructuringElement
        {
            get { return kernel; }
            set
            {
                kernel = value;
                kernelSize = kernel.GetLength(0);
            }
        }

        public MorphologyFilter(bool[,] kernel = null)
        {
            if (kernel == null)
            {
                SetDefaultKernel();
            }
            else
            {
                this.kernel = kernel;
                kernelSize = kernel.GetLength(0);
            }
        }
        protected virtual void SetDefaultKernel()
        {
            kernelSize = 3;
            kernel = new bool[kernelSize, kernelSize];
            for (int i = 0; i < kernelSize; i++)
                for (int j = 0; j < kernelSize; j++)
                    kernel[i, j] = true;
        }
        protected abstract override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);
    }

    // Класс для создания и редактирования структурных элементов
    public static class StructuringElementFactory
    {
        public static bool[,] CreateSquareElement(int size)
        {
            bool[,] element = new bool[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    element[i, j] = true;
            return element;
        }
        public static bool[,] CreateCrossElement(int size)
        {
            bool[,] element = new bool[size, size];
            int center = size / 2;
            for (int i = 0; i < size; i++)
            {
                element[center, i] = true; // Горизонтальная линия
                element[i, center] = true; // Вертикальная линия
            }
            return element;
        }
        public static bool[,] CreateCircleElement(int diameter)
        {
            bool[,] element = new bool[diameter, diameter];
            float radius = diameter / 2f;
            float center = diameter / 2f;

            for (int i = 0; i < diameter; i++)
            {
                for (int j = 0; j < diameter; j++)
                {
                    float distance = (float)Math.Sqrt(Math.Pow(i - center, 2) + Math.Pow(j - center, 2));
                    element[i, j] = distance <= radius;
                }
            }
            return element;
        }
    }

    class DilationFilter : MorphologyFilter
    {
        public DilationFilter() : base() { }
        public DilationFilter(bool[,] kernel) : base(kernel) { }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radius = kernelSize / 2;
            byte maxR = 0, maxG = 0, maxB = 0;

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    if (kernel[i + radius, j + radius])
                    {
                        int idX = Clamp(x + i, 0, sourceImage.Width - 1);
                        int idY = Clamp(y + j, 0, sourceImage.Height - 1);
                        Color neighborColor = sourceImage.GetPixel(idX, idY);

                        if (neighborColor.R > maxR) maxR = neighborColor.R;
                        if (neighborColor.G > maxG) maxG = neighborColor.G;
                        if (neighborColor.B > maxB) maxB = neighborColor.B;
                    }
                }
            }

            return Color.FromArgb(maxR, maxG, maxB);
        }
    }

    class ErosionFilter : MorphologyFilter
    {
        public ErosionFilter() : base() { }
        public ErosionFilter(bool[,] kernel) : base(kernel) { }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radius = kernelSize / 2;
            byte minR = 255, minG = 255, minB = 255;

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    if (kernel[i + radius, j + radius])
                    {
                        int idX = Clamp(x + i, 0, sourceImage.Width - 1);
                        int idY = Clamp(y + j, 0, sourceImage.Height - 1);
                        Color neighborColor = sourceImage.GetPixel(idX, idY);

                        if (neighborColor.R < minR) minR = neighborColor.R;
                        if (neighborColor.G < minG) minG = neighborColor.G;
                        if (neighborColor.B < minB) minB = neighborColor.B;
                    }
                }
            }

            return Color.FromArgb(minR, minG, minB);
        }
    }

    class OpeningFilter : Filters
    {
        public bool[,] StructuringElement { get; set; }

        public OpeningFilter()
        {
            StructuringElement = StructuringElementFactory.CreateSquareElement(3);
        }

        public OpeningFilter(bool[,] element)
        {
            StructuringElement = element;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return sourceImage.GetPixel(x, y);
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            ErosionFilter erosion = new ErosionFilter(StructuringElement);
            Bitmap erodedImage = erosion.processImage(sourceImage, worker);

            if (erodedImage == null) return null;

            DilationFilter dilation = new DilationFilter(StructuringElement);
            return dilation.processImage(erodedImage, worker);
        }
    }

    class ClosingFilter : Filters
    {
        public bool[,] StructuringElement { get; set; }

        public ClosingFilter()
        {
            StructuringElement = StructuringElementFactory.CreateSquareElement(3);
        }

        public ClosingFilter(bool[,] element)
        {
            StructuringElement = element;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return sourceImage.GetPixel(x, y);
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            DilationFilter dilation = new DilationFilter(StructuringElement);
            Bitmap dilatedImage = dilation.processImage(sourceImage, worker);

            if (dilatedImage == null) return null;

            ErosionFilter erosion = new ErosionFilter(StructuringElement);
            return erosion.processImage(dilatedImage, worker);
        }
    }

    class TopHatFilter : Filters
    {
        public bool[,] StructuringElement { get; set; }

        public TopHatFilter()
        {
            StructuringElement = StructuringElementFactory.CreateSquareElement(3);
        }

        public TopHatFilter(bool[,] element)
        {
            StructuringElement = element;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return sourceImage.GetPixel(x, y);
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            OpeningFilter opening = new OpeningFilter(StructuringElement);
            Bitmap openedImage = opening.processImage(sourceImage, worker);

            if (openedImage == null) return null;

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color srcColor = sourceImage.GetPixel(i, j);
                    Color openedColor = openedImage.GetPixel(i, j);

                    int r = Clamp(srcColor.R - openedColor.R, 0, 255);
                    int g = Clamp(srcColor.G - openedColor.G, 0, 255);
                    int b = Clamp(srcColor.B - openedColor.B, 0, 255);

                    resultImage.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }

            return resultImage;
        }
    }

    class MedianFilter : Filters
    {
        private int radius;

        public MedianFilter(int radius = 1)
        {
            this.radius = radius;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int windowSize = 2 * radius + 1;
            int pixelCount = windowSize * windowSize;

            int[] red = new int[pixelCount];
            int[] green = new int[pixelCount];
            int[] blue = new int[pixelCount];

            int index = 0;

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    int idX = Clamp(x + i, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + j, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    red[index] = neighborColor.R;
                    green[index] = neighborColor.G;
                    blue[index] = neighborColor.B;

                    index++;
                }
            }

            Array.Sort(red);
            Array.Sort(green);
            Array.Sort(blue);

            int medianIndex = pixelCount / 2;

            return Color.FromArgb(
                red[medianIndex],
                green[medianIndex],
                blue[medianIndex]
            );
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }

            return resultImage;
        }
    }
}