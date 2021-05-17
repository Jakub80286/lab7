using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace lab7
{
    public partial class Form1 : Form
    {
        Image img;
        Bitmap baseBitmap;
        Bitmap normalizedBitmap;
        Bitmap gsBIT;
        int x;
        int avg;
        int y;

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load_1(object sender, EventArgs e)
        {
            modifyHistogram modify = new modifyHistogram(new Bitmap(pictureBox1.Image), new Bitmap(pictureBox1.Image), chart1, chart2);
            Blur blur = new Blur(new Bitmap(pictureBox1.Image), new Bitmap(pictureBox1.Image), chart1, chart2);
            pictureBox2.Image = modify.rozm();
            pictureBox3.Image = blur.wyr_histog(5);
            pictureBox4.Image = blur.gauss(2);
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
    }
    class Blur
    {
        Bitmap podstBit;
        Chart podstHist;
        Chart zmiennionyHist;
        private int[] ilosc = new int[3];
        private int[] odchylenie = new int[3];

        public Blur(Bitmap baseBitmap, Bitmap normalizedBitmap, Chart baseHistogram, Chart normalizedHistogram)
        {
            this.podstBit = baseBitmap;
            this.podstHist = baseHistogram;
            this.zmiennionyHist = normalizedHistogram;
        }


        public Bitmap wyr_histog(int n)
        {
            Color[,] maska = new Color[n, n];
            int[] avg;
            for (int i = n - 1; i < podstBit.Width; i += n)
            {
                for (int j = n - 1; j < podstBit.Height; j += n)
                {
                    for (int x = 0; x < n; x++)
                    {
                        for (int y = 0; y < n; y++)
                        {
                            maska[x, y] = podstBit.GetPixel(i - x, j - y);
                        }
                    }
                    avg = this.srednia(maska);
                    for (int x = 0; x < n; x++)
                    {
                        for (int y = 0; y < n; y++)
                        {
                            podstBit.SetPixel(i - x, j - y, Color.FromArgb(avg[0],
                                avg[1],
                                avg[2]));
                        }
                    }
                }
            }
            return podstBit;
        }

        private int[] srednia(Color[,] mask)
        {
            int maskSize = mask.Length / mask.GetLength(1);
            int[] tabSredn = new int[maskSize];
            for (int i = 0; i < maskSize; i++)
            {
                for (int j = 0; j < maskSize; j++)
                {
                    tabSredn[0] += mask[i, j].R;
                    tabSredn[1] += mask[i, j].G;
                    tabSredn[2] += mask[i, j].B;
                }
            }
            tabSredn[0] /= mask.Length;
            tabSredn[1] /= mask.Length;
            tabSredn[2] /= mask.Length;

            return tabSredn;
        }

        public Bitmap gauss(int promien)
        {
            var wielkosc = (promien * 2) + 1;
            var odchylenie = promien / 2;
            var mask = new double[wielkosc, wielkosc];
            double sum = 0.0;
            for (int i = -promien; i <= promien; i++)
            {
                for (int j = -promien; j <= promien; j++)
                {
                    int xx = -(i * i + j * j);
                    int yy = 2 * odchylenie * odchylenie;
                    var eExpre = Math.Pow(Math.E, xx / yy);
                    var wartosc = (eExpre / (2 * Math.PI * odchylenie * odchylenie));

                    mask[i + promien, j + promien] = wartosc;
                    sum += wartosc;
                }
            }

            for (int i = 0; i < wielkosc; i++)
            {
                for (int j = 0; j < wielkosc; j++)
                {
                    mask[i, j] /= sum;
                }
            }

            for (int x = promien; x < podstBit.Width - promien; x++)
            {
                for (int y = promien; y < podstBit.Height - promien; y++)
                {
                    double red = 0, green = 0, blue = 0;

                    for (int i = -promien; i <= promien; i++)
                    {
                        for (int j = -promien; j <= promien; j++)
                        {
                            double temp = mask[i + promien, j + promien];
                            var pixel = podstBit.GetPixel(x - i, y - j);

                            red += pixel.R * temp;
                            green += pixel.G * temp;
                            blue += pixel.B * temp;
                        }
                    }
                    podstBit.SetPixel(x, y, Color.FromArgb(
                        checkIfInRgb(red), checkIfInRgb(green), checkIfInRgb(blue)));
                }
            }
            return podstBit;
        }

        private int checkIfInRgb(double temp)
        {
            if (temp > 255) return 255;
            else if (temp < 0) return 0;
            return (int)temp;
        }
    }
    class modifyHistogram
    {
        Bitmap podstBit;
        Bitmap zmienionyBit;
        int aa;
        private Chart podstHist;
        private Chart zmienionyHist;
        double[] histogramRed = new double[256];
        double[] histogramGreen = new double[256];
        double[] histogramBlue = new double[256];

        public modifyHistogram(Bitmap podstBit, Bitmap zmienionyBit, Chart podstHist, Chart zmienonyHist)
        {
            this.podstBit = podstBit;
            this.zmienionyBit = zmienionyBit;
            aa = podstBit.Width * podstBit.Height;
            this.podstHist = podstHist;
            this.zmienionyHist = zmienonyHist;
            histogramPodstawa();
            fillHistogram();
        }


        private void histogramPodstawa()
        {
            double[] red = new double[256];
            double[] green = new double[256];
            double[] blue = new double[256];
            for (int x = 0; x < podstBit.Width; x++)
            {
                for (int y = 0; y < podstBit.Height; y++)
                {
                    Color pixel = podstBit.GetPixel(x, y);
                    red[pixel.R]++;
                    green[pixel.G]++;
                    blue[pixel.B]++;
                }
            }


            podstHist.Series["red"].Points.Clear();
            podstHist.Series["green"].Points.Clear();
            podstHist.Series["blue"].Points.Clear();
            for (int i = 0; i < 256; i++)
            {
                podstHist.Series["red"].Points.AddXY(i, red[i] / aa);
                podstHist.Series["green"].Points.AddXY(i, green[i] / aa);
                podstHist.Series["blue"].Points.AddXY(i, blue[i] / aa);
            }
            podstHist.Invalidate();
        }

        private void fillHistogram()
        {

            for (int i = 0; i < 256; i++)
            {
                histogramRed[i] = koncHist(i, "red");
                histogramGreen[i] = koncHist(i, "green");
                histogramBlue[i] = koncHist(i, "blue");
                zmienionyHist.Series["red"].Points.AddXY(i, histogramRed[i]);
                zmienionyHist.Series["green"].Points.AddXY(i, histogramGreen[i]);
                zmienionyHist.Series["blue"].Points.AddXY(i, histogramBlue[i]);
            }
        }

        private double koncHist(int poziom, string kolor)
        {
            if (poziom == 0) return podstHist.Series[kolor].Points[0].YValues[podstHist.Series[kolor].Points[0].YValues.Length - 1];
            else return podstHist.Series[kolor].Points[poziom].YValues[podstHist.Series[kolor].Points[0].YValues.Length - 1]
                    + koncHist(poziom - 1, kolor);
        }

        public Bitmap rozm()
        {

            int r, g, b;
            for (int x = 0; x < podstBit.Width; x++)
            {
                for (int y = 0; y < podstBit.Height; y++)
                {
                    Color pixel = podstBit.GetPixel(x, y);
                    r = (int)(255 * histogramRed[pixel.R]);
                    g = (int)(255 * histogramGreen[pixel.G]);
                    b = (int)(255 * histogramBlue[pixel.B]);
                    zmienionyBit.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
                }
            }
            return zmienionyBit;
        }
    }

}
