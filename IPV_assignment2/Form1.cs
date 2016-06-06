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

namespace IPV_assignment2
{
    public partial class Form1 : Form
    {
        private Image<Bgr, byte> _imageFrame;
        private delegate double Distance(int x, int y);
        Distance d;
        bool gray_in_use = false;

        public Form1()
        {
            InitializeComponent();
            d = new Distance(this.DistanceToBottomLeft);
           
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _imageFrame = new Image<Bgr, byte>(@"..\..\Resources\ipv.bmp");
           // imageBox1.Image = _imageFrame;
            imageBox2.Image = _imageFrame.Clone();

            Image<Bgr, Byte> ThresholdCloneImage = _imageFrame.Clone();
            Image<Gray, Byte> ThresholdImage = ThresholdCloneImage.Convert<Gray, Byte>();
            for (int x = 0; x < ThresholdImage.Rows; x++)
            {
                for (int y = 0; y < ThresholdImage.Cols; y++)
                {
                    if (ThresholdImage[x, y].Intensity > 90)
                    {
                        ThresholdImage.Data[x, y, 0] = 255;
                    }
                    else if (ThresholdImage[x, y].Intensity <= 90)
                    {
                        ThresholdImage.Data[x, y, 0] = 0;
                    }
                }
            }
           
            findLargestObject(ThresholdImage);
            FindCorner(ThresholdImage);


            d = new Distance(this.DistanceToBottomRight);
            FindCorner(ThresholdImage);
            d = new Distance(this.DistanceToTopLeft);
            FindCorner(ThresholdImage);
            d = new Distance(this.DistanceToTopRight);
            FindCorner(ThresholdImage);
            imageBox1.Image = ThresholdImage;
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _imageFrame = new Image<Bgr, byte>(@"..\..\Resources\sudoku-original.jpg");
            imageBox1.Image = _imageFrame;
            imageBox2.Image = _imageFrame.Clone();
        }


        public void findLargestObject(Image<Gray, byte> grayImage)
        {
            double largestarea = 0;
            int area;
            Point location = new Point(0, 0);
            MCvConnectedComp comp = new MCvConnectedComp();
            Rectangle boundingBox = new Rectangle();
            Rectangle boundingBoxLargest = new Rectangle();

            // Floodfill every white pixel with new Gray(64), and while doing that, keep track of the largest area that was filled.
            for (var y = 0; y < grayImage.Height; y++)
            {
                for (var x = 0; x < grayImage.Width; x++)
                {
                    if (grayImage.Data[y, x, 0] >= 128)
                    {
                        // TO DO: perform the floodfill on the pixel
                       area = CvInvoke.FloodFill(grayImage, null, new Point(x, y), new MCvScalar(64), out boundingBox, new MCvScalar(5), new MCvScalar(5));
                        
                        // Check whether the blob is larger then the ones found before
                        if (area > largestarea)
                        {
                            largestarea = area;
                            location = new Point(x, y);
                            boundingBoxLargest = boundingBox;

                        }
                    }
                }
            }

            // TO DO: If there is blob found that has the right area, width and height, then Floodfill it with white 

            largestarea = CvInvoke.FloodFill(grayImage, null, location, new MCvScalar(255), out boundingBoxLargest, new MCvScalar(5), new MCvScalar(5));
            // TO DO: Fill all the other blobs with black to remove them
            for (var y = 0; y < grayImage.Height; y++)
            {
                for (var x = 0; x < grayImage.Width; x++)
                {
                    if (grayImage.Data[y, x, 0] == 64)
                    {

                        // TO DO: perform the floodfill on the pixel
                        area = CvInvoke.FloodFill(grayImage, null, new Point(x, y), new MCvScalar(0), out boundingBox, new MCvScalar(5), new MCvScalar(5));


                    }
                }
            }
           
        }
        
        #region calculateTheDistance
        private double DistanceToBottomLeft(int x, int y)
        {
            return (Math.Pow(x - 0, 2) + Math.Pow(y - _imageFrame.Height, 2));
            //
        }
        private double DistanceToBottomRight(int x, int y)
        {
           
            return (Math.Pow(x - _imageFrame.Width, 2) + Math.Pow(y - _imageFrame.Height, 2));
        }
        private double DistanceToTopLeft(int x, int y)
        {
            return (Math.Pow(x - 0, 2) + Math.Pow(y - 0, 2));
          
        }
        private double DistanceToTopRight(int x, int y)
        {
           
            return (Math.Pow(x - _imageFrame.Width, 2) + Math.Pow(y - 0, 2));
        }
        #endregion   


        private void FindCorner(Image<Gray, Byte> image)
        {
            double min = int.MaxValue;
            int xneed = 0;
            int yneed= 0;
            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    if (image.Data[y, x, 0] == 255)
                    {
                        
                        if(this.d(x,y) < min)
                        {
                            min = this.d(x, y);
                            xneed = x;
                            yneed = y;
                        }
                    }
                }
            }
            image.Data[yneed, xneed, 0] = 128;
         
        }
       
      
    }
}
   