using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Cvb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV.UI;

namespace IPV_assignment2
{
    public partial class Form1 : Form
    {
        private Image<Bgr, byte> _imageFrame;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _imageFrame = new Image<Bgr, byte>(@"..\..\Resources\ipv.bmp");
            imageBox2.Image = _imageFrame.Clone();
            Image<Gray, Byte> tempImage = _imageFrame.Convert<Gray, Byte>();
            tempImage = tempImage.ThresholdBinary(new Gray(200), new Gray(255));

            tempImage = FindLargestObject(tempImage);

            // TO DO: Find the corner points of the largest object; use the algorithm you made in assignment 2a
            List<Point> corners = GetCorners(tempImage);
            Point LU, RU, LB, RB;
            LU = corners[0];
            RU = corners[1];
            LB = corners[2];
            RB = corners[3];
            Console.WriteLine(corners[0] +" " + corners[1] + " " + corners[2] + " " + corners[3]);

            tempImage.Data[LU.Y, LU.X, 0] = 64;
            tempImage.Data[RU.Y, RU.X, 0] = 64;
            tempImage.Data[LB.Y, LB.X, 0] = 64;
            tempImage.Data[RB.Y, RB.X, 0] = 64;

            imageBox1.Image = tempImage;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _imageFrame = new Image<Bgr, byte>(@"..\..\Resources\sudoku-original.jpg");
            imageBox1.Image = _imageFrame;
            imageBox2.Image = _imageFrame.Clone();
        }

        public Image<Gray, Byte> FindLargestObject(Image<Gray, byte> tempImage)
        {
            //Find all objects in image
            Rectangle boundingBox;
            List<SizeAndPoint> areasList = new List<SizeAndPoint>();
            for (int x = 0; x < tempImage.Cols; x++)
            {
                for (int y = 0; y < tempImage.Rows; y++)
                {
                    if (tempImage.Data[y, x, 0] == 255)
                    {
                        int temp = CvInvoke.FloodFill(tempImage, null, new Point(x, y), new MCvScalar(64), out boundingBox,
                            new MCvScalar(5), new MCvScalar(5));
                        var sap = new SizeAndPoint
                        {
                            AreaPoint = new Point(x, y),
                            AreaSize = temp
                        };
                        areasList.Add(sap);
                    }
                }
            }

            //Find the largest object
            int largestAreaSize = areasList.Max(x => x.AreaSize);
            var largestArea = areasList.Find(x => x.AreaSize == largestAreaSize);
            
            foreach (var x in areasList)
            {
                if (x.AreaSize != largestAreaSize)
                {
                    CvInvoke.FloodFill(tempImage, null, x.AreaPoint, new MCvScalar(0), out boundingBox,
                            new MCvScalar(5), new MCvScalar(5));
                }
            }

            CvInvoke.FloodFill(tempImage, null, largestArea.AreaPoint , new MCvScalar(255), out boundingBox,
                            new MCvScalar(5), new MCvScalar(5));

            return tempImage;
        }

        private List<Point> GetCorners(Image<Gray, byte> img)
        {
            // 0 = top left, 1 = top right, 2 = bottom left, 3 = bottom right
            List<double> distance = new List<double>();
            Image<Gray, byte> greyImage = img;
            List<Point> corners = new List<Point> { new Point(0, 0), new Point(0, img.Width), new Point(img.Height, 0), new Point(img.Height, img.Width) };
            List<Point> objectCorners = new List<Point>();

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (greyImage[i, j].Intensity > 200)
                    {
                        if (distance.Count == 0)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                int dx = i - corners[k].X;
                                int dy = j - corners[k].Y;

                                distance.Add(Math.Sqrt(dx * dx + dy * dy));
                                objectCorners.Add(new Point(j, i));
                            }
                        }
                        else
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                int dx = i - corners[k].X;
                                int dy = j - corners[k].Y;
                                double dist = Math.Sqrt(dx * dx + dy * dy);
                                if (dist < distance[k])
                                {
                                    distance[k] = dist;
                                    objectCorners[k] = new Point(j, i);
                                }
                            }
                        }
                    }
                }
            }
            return objectCorners;
        }
    }
}