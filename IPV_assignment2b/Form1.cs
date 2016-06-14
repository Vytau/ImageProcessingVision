using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SudokuStartup;

namespace IPV_assignment2b
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            init();

            // Read the color image from file
            Image<Bgr, Byte> image = new Image<Bgr, Byte>(@"..\..\Resources\sudoku-original.jpg");
            ImageViewer.Show(image, "Original image");

            // Convert to gray level image
            Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>();

            // TO DO: Gaussian blur the gray level image; try kernel size 5, and sigma 1
            grayImage._SmoothGaussian(5, 5, 1, 1);
            ;
            ;
            ImageViewer.Show(grayImage, "After Guassian Blur");

            // Make a copy to use later in WarpPerspective
            Image<Gray, Byte> grayImageClone = grayImage.Clone();

            // TO DO: Apply adaptive threshold to the gray level image; try for blockSize: 5, and for param1: new Gray(2)
            grayImage = grayImage.ThresholdAdaptive(new Gray(255), AdaptiveThresholdType.GaussianC,
                ThresholdType.BinaryInv, 5, new Gray(2));
            ;
            ;

            // TO DO: Find the largest object in the thresholded image; use the algorithm you made in assignment 2a
            grayImage = FindLargestObject(grayImage);
            ;
            ;
            ImageViewer.Show(grayImage, "Largest Object");

            // TO DO: Find the corner points of the largest object; use the algorithm you made in assignment 2a
            List<Point> corners = GetCorners(grayImage);
            Point LU, RU, LB, RB;
            LU = corners[0];
            RU = corners[1];
            LB = corners[2];
            RB = corners[3];

            // Draw lines to the corners in the original image
            PointF[] src = {LU, RU, LB, RB};
            PointF[] dst =
            {
                new Point(0, 0), new Point(image.Width - 1, 0), new Point(0, image.Height - 1),
                new Point(image.Width - 1, image.Height - 1)
            };
            image.Draw(new LineSegment2D(LU, new Point(0, 0)), new Bgr(0, 0, 255), 1);
            image.Draw(new LineSegment2D(RU, new Point(image.Width - 1, 0)), new Bgr(0, 255, 0), 1);
            image.Draw(new LineSegment2D(LB, new Point(0, image.Height - 1)), new Bgr(255, 0, 0), 1);
            image.Draw(new LineSegment2D(RB, new Point(image.Width - 1, image.Height - 1)), new Bgr(255, 255, 0), 1);
            ImageViewer.Show(image, "Corners");

            // Stretch the gray level copy such that the corner points of the largest object are in the corners of the image
            imgpersp = new Image<Gray, byte>(grayImageClone.Size);
            CvInvoke.WarpPerspective(grayImageClone, imgpersp, CvInvoke.GetPerspectiveTransform(src, dst),
                grayImage.Size);

            // TO DO: Apply threshold to make it binary again
            imgpersp = imgpersp.ThresholdAdaptive(new Gray(255), AdaptiveThresholdType.GaussianC,
                ThresholdType.BinaryInv, 5, new Gray(2));
            ;
            ;
            ImageViewer.Show(imgpersp, "Warp+threshold");

            // Extract the 81 cells and put them in their ImageBoxes
            Image<Gray, byte> cell = null;

            int cellHeight = (imgpersp.Height/9);
            int cellWidth = (imgpersp.Width/9);

            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    // TO DO: Extract cell [i,j]; use ROI and Copy
                    cell = new Image<Gray, byte>(cellWidth, cellHeight);
                    imgpersp.ROI = new Rectangle(i*cellWidth, j*cellHeight, cellWidth, cellHeight);
                    imgpersp.CopyTo(cell);
                    ;
                    ;
                    imageBoxMatrix[i, j].Image = cell;
                }
            }
        }

        private ImageBox[,] imageBoxMatrix = new ImageBox[9, 9];
        Image<Gray, byte> imgpersp;

        private void init()
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    imageBoxMatrix[i, j] = new ImageBox();
                    ((System.ComponentModel.ISupportInitialize) (imageBoxMatrix[i, j])).BeginInit();
                    SuspendLayout();
                    imageBoxMatrix[i, j].Location = new Point(12 + 55*i, 12 + 55*j);
                    imageBoxMatrix[i, j].Name = "imageBox" + i + j;
                    imageBoxMatrix[i, j].Size = new Size(50, 50);
                    Controls.Add(imageBoxMatrix[i, j]);
                    ((System.ComponentModel.ISupportInitialize) (imageBoxMatrix[i, j])).EndInit();
                    ResumeLayout(false);
                }
        }

        /// <summary>
        /// Moves the content of the image such that the point cog is in the center.
        /// </summary>
        /// <param name="image">The original image. Will be changed in place.</param>
        /// <param name="cog">The whole image will be moved such that this point is in the center of the image.</param>
        /// <returns>The image is returned after centering.</returns>
        private Image<Gray, byte> Center(Image<Gray, byte> image, Point cog)
        {
            Point LUc = new Point(0, 0);
            Point RUc = new Point(image.Width - 1, 0);
            Point LBc = new Point(0, image.Height - 1);
            Point RBc = new Point(image.Width - 1, image.Height - 1);
            Point cc = new Point(-image.Width/2, -image.Height/2);
            PointF[] dst = {LUc, RUc, LBc, RBc};
            cog.Offset(cc);
            LUc.Offset(cog);
            RUc.Offset(cog);
            LBc.Offset(cog);
            RBc.Offset(cog);
            PointF[] src = {LUc, RUc, LBc, RBc};

            Image<Gray, byte> imgpersp = new Image<Gray, byte>(image.Size);
            CvInvoke.WarpPerspective(image, imgpersp, CvInvoke.GetPerspectiveTransform(src, dst), image.Size);
            //Image<Gray, byte> imgpersp = image.WarpPerspective<double>(CameraCalibration.GetPerspectiveTransform(src, dst), INTER.CV_INTER_LINEAR, WARP.CV_WARP_DEFAULT, new Gray(255));
            return imgpersp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Image<Gray, byte> cell;
            int cellHeight = (imgpersp.Height/9);
            int cellWidth = (imgpersp.Width/9);

            DigitRecognizer dr = new DigitRecognizer();
            Console.Write("Training ... ");
            dr.train(@"..\..\Resources\train-images.idx3-ubyte", @"..\..\Resources\train-labels.idx1-ubyte");
            Console.WriteLine("done");


            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    cell = (Image<Gray, byte>) imageBoxMatrix[i, j].Image;

                    // TO DO: Find the largest object in that cell
                    // In order to count as a digit, that object must be:
                    // * not too small: an area of at least 150 pixels
                    // * not too wide: a width of at most 70% of the width of the cell
                    // * not too low: a height of at most 85% of the height of the cell 
                    Rectangle boundingRect = new Rectangle();
                    int area = 0;
                    List<SizeAndPoint> areasList = new List<SizeAndPoint>();
                    ;
                    ;
                    for (int x = 0; x < cell.Cols; x++)
                    {
                        for (int y = 0; y < cell.Rows; y++)
                        {
                            if (cell.Data[y, x, 0] == 255)
                            {
                                int tempArea = CvInvoke.FloodFill(cell, null, new Point(x, y), new MCvScalar(64),
                                    out boundingRect, new MCvScalar(0), new MCvScalar(0));
                                var sap = new SizeAndPoint
                                {
                                    AreaPoint = new Point(x, y),
                                    AreaSize = tempArea,
                                    AreaRectangle = boundingRect
                                };
                                areasList.Add(sap);
                            }
                        }
                    }

                    //Find the area of a number in a cell
                    foreach (var x in areasList)
                    {
                        if (x.AreaSize > 120 && x.AreaSize > area && x.AreaRectangle.Width < 0.7 * cell.Width &&
                            x.AreaRectangle.Height < 0.85*cell.Height)
                        {
                            area = x.AreaSize;
                        }
                    }

                    //FloodFill with white color founded number
                    if (areasList.Find(x => x.AreaSize == area) != null)
                    {
                        var largestArea = areasList.Find(x => x.AreaSize == area);
                        CvInvoke.FloodFill(cell, null, largestArea.AreaPoint, new MCvScalar(255),
                            out boundingRect, new MCvScalar(0), new MCvScalar(0));
                    }

                    if (area > 120 && boundingRect.Width < 0.7*cell.Width && boundingRect.Height < 0.85*cell.Height)
                    {
                        // Put the digit in the center of the cell
                        cell = Center(cell,
                            new Point(boundingRect.X + boundingRect.Width/2, boundingRect.Y + boundingRect.Height/2));

                        imageBoxMatrix[i, j].Image = cell;
                        // Classify the digit
                        int number = (int) dr.classify(cell);
                        Console.Write(number);
                    }
                    else
                    {
                        imageBoxMatrix[i, j].Image = cell;
                        Console.Write("_");
                    }
                }
                Console.WriteLine();
            }
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
                        int temp = CvInvoke.FloodFill(tempImage, null, new Point(x, y), new MCvScalar(64),
                            out boundingBox,
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

            CvInvoke.FloodFill(tempImage, null, largestArea.AreaPoint, new MCvScalar(255), out boundingBox,
                new MCvScalar(5), new MCvScalar(5));

            return tempImage;
        }

        private List<Point> GetCorners(Image<Gray, byte> img)
        {
            // 0 = top left, 1 = top right, 2 = bottom left, 3 = bottom right
            List<double> distance = new List<double>();
            Image<Gray, byte> greyImage = img;
            List<Point> corners = new List<Point>
            {
                new Point(0, 0),
                new Point(0, img.Width),
                new Point(img.Height, 0),
                new Point(img.Height, img.Width)
            };
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

                                distance.Add(Math.Sqrt(dx*dx + dy*dy));
                                objectCorners.Add(new Point(j, i));
                            }
                        }
                        else
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                int dx = i - corners[k].X;
                                int dy = j - corners[k].Y;
                                double dist = Math.Sqrt(dx*dx + dy*dy);
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