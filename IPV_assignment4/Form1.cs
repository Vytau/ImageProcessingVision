using System;
using System.Windows.Forms;
using System.Drawing;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.Runtime.CompilerServices;

namespace Face_Detection
{
    public partial class Form1 : Form
    {
        private const int NMarkers = 6;
        private const int ProbeFrame = 50;
        private Capture cap;
        private Rectangle[] markers = new Rectangle[NMarkers];
        private CircleF[] dots = new CircleF[NMarkers];
        private int frameCounter = 0;
        private Hsv[] hsvLower = new Hsv[NMarkers];
        private Hsv[] hsvUpper = new Hsv[NMarkers];
        private Image<Hsv, byte> nextHsvFrame;
        private Image<Gray, byte> inrange;
        private Hsv[] probe = new Hsv[NMarkers];
        private VectorOfPoint approxContour;
        private Rectangle maxBBox;
        private MCvPoint2D64f maxCOG;

        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            using (Image<Bgr, byte> nextBgrFrame = cap.QueryFrame().ToImage<Bgr, byte>())
            using (Image<Bgr, byte> testImage = new Image<Bgr, byte>(nextBgrFrame.Size))
            {
                // TODO: Start with a Gaussian blur
                ;
                ;
                testImage._SmoothGaussian(5);

                nextHsvFrame = nextBgrFrame.Convert<Hsv, byte>();

                // Show the dots in the first ProbFrame frames
                if (frameCounter < ProbeFrame)
                {
                    for (int i = 0; i < NMarkers; i++)
                    {
                        nextHsvFrame.Draw(dots[i], new Hsv(0, 0, 255), -1); // Draw white filled circle
                        nextHsvFrame.Draw(dots[i], new Hsv(0, 255, 255)); // Draw red border
                    }
                    nextHsvFrame.Draw("Please place your hand such", new Point(0, 25), FontFace.HersheyPlain, 2,
                        new Hsv(0, 0, 255), 3);
                    nextHsvFrame.Draw("that it covers all the dots", new Point(0, 50), FontFace.HersheyPlain, 2,
                        new Hsv(0, 0, 255), 3);
                }
                ;

                // Get the HSV values from the centers of the dots, and put them in array probe
                if (frameCounter == ProbeFrame)
                {
                    for (int i = 0; i < NMarkers; i++)
                    {
                        probe[i] = nextHsvFrame[new Point((int) dots[i].Center.X, (int) dots[i].Center.Y)];
                    }
                    // Also slow down the timer
                    timer1.Interval = 100;
                }
                ;

                // In all following frames, do the hand detection
                if (frameCounter > ProbeFrame)
                {
                    // Define the HSV rang for every mark seperately
                    for (int i = 0; i < NMarkers; i++)
                    {
                        hsvLower[i] = new Hsv(probe[i].Hue - (double) numericUpDownH.Value,
                            Math.Max(20, probe[i].Satuation - (double) numericUpDownS.Value),
                            Math.Max(20, probe[i].Value - (double) numericUpDownV.Value));
                        hsvUpper[i] = new Hsv(probe[i].Hue + (double) numericUpDownH.Value,
                            probe[i].Satuation + (double) numericUpDownS.Value,
                            probe[i].Value + (double) numericUpDownV.Value);
                    }

                    // TODO: Perform InRange for every marker seperately, and add all the resulting binary images using method Image.Add
                    ;
                    ;
                    inrange = new Image<Gray, byte>(nextBgrFrame.Size);
                    for (int i = 0; i < NMarkers; i++)
                    {
                        inrange = inrange.Add(nextHsvFrame.InRange(hsvLower[i], hsvUpper[i]));
                    }
                    // TODO: Find the contours
                    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                    ;
                    ;
                    CvInvoke.FindContours(inrange, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                    // TODO: Find the contour with the biggest area, and with its Center of Gravity in the lower half of the image
                    // Save the COG of that object in maxCOG, and save the boundingrectangle of that object in maxBBox (use CvInvoke.BoundingRectangle)
                    int maxI = -1;
                    double largestArea = 0;
                    ;
                    ;
                    for (int i = 0; i < contours.Size; i++)
                    {
                        if (CvInvoke.ContourArea(contours[i]) > largestArea)
                        {
                            largestArea = CvInvoke.ContourArea(contours[i]);
                            maxI = i;
                        }
                    }

                    if (maxI != -1) // Check if such an image is found
                    {
                        // Draw the contour with the biggest area
                        testImage.Draw(contours[maxI].ToArray(), new Bgr(128, 128, 128), 1);

                        // Approximate it, in order to reduce the number of points
                        approxContour = new VectorOfPoint();
                        CvInvoke.ApproxPolyDP(contours[maxI], approxContour, 15, true);
                        testImage.Draw(approxContour.ToArray(), new Bgr(255, 255, 255), 1);

                        // Find the convex hull; this will be used to find convexity defects later
                        VectorOfInt convexHull = new VectorOfInt();
                        CvInvoke.ConvexHull(approxContour, convexHull);

                        // Find the convex hull again; now just to draw it
                        VectorOfPoint convexHullP = new VectorOfPoint();
                        CvInvoke.ConvexHull(approxContour, convexHullP);
                        nextHsvFrame.Draw(convexHullP.ToArray(), new Hsv(80, 255, 255), 1);
                        testImage.Draw(convexHullP.ToArray(), new Bgr(0, 255, 0), 1);

                        // Find the convexity defects
                        Mat convexityDefect = new Mat();
                        CvInvoke.ConvexityDefects(approxContour, convexHull, convexityDefect);

                        // If there were actually defects found, then analyse them
                        if (convexityDefect.Height != 0)
                            DrawAndComputeFingersNum(convexityDefect);
                    }
                }

                imageBox1.Image = nextHsvFrame;
                imageBox2.Image = testImage;
                frameCounter++;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cap = new Capture(0);

            // Define the dots as CircleF objects
            dots[0] = new CircleF(new PointF(cap.Width/2, cap.Height/2 - 25), 5);
            // TODO: Define more of these dots
            ;
            ;
            dots[1] = new CircleF(new PointF(cap.Width / 2 - 50, cap.Height / 2), 5);
            dots[2] = new CircleF(new PointF(cap.Width / 2 + 50, cap.Height / 2), 5);
            dots[3] = new CircleF(new PointF(cap.Width / 2 + 25, cap.Height / 2 + 100), 5);
            dots[4] = new CircleF(new PointF(cap.Width / 2 - 25, cap.Height / 2 + 100), 5);
            dots[5] = new CircleF(new PointF(cap.Width / 2, cap.Height / 2 + 200), 5);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled) timer1.Stop();
            else timer1.Start();
        }

        private void DrawAndComputeFingersNum(Mat convexityDefects)
        {
            int fingerNum = 1; // Variable to count fingers

            // Copy the defects to a Mat, otherwise they will not be easily accessible
            using (
                Matrix<int> m = new Matrix<int>(convexityDefects.Rows, convexityDefects.Cols,
                    convexityDefects.NumberOfChannels))
            {
                convexityDefects.CopyTo(m);

                // For each defect, do the following
                for (int i = 0; i < m.Rows; i++)
                {
                    // Get start point, defect point end point and depth
                    Point startPoint = approxContour[m.Data[i, 0]];
                    Point endPoint = approxContour[m.Data[i, 1]];
                    Point defectPoint = approxContour[m.Data[i, 2]];
                    double depth = m.Data[i, 3]/256.0;

                    // Some simple check to count the fingers; this can probably be improved a lot
                    if ((startPoint.Y < maxCOG.Y || endPoint.Y < maxCOG.Y) &&
                        // start point or end point above center of gravity
                        (startPoint.Y < defectPoint.Y) && // start point above defect point
                        (depth > maxBBox.Height/6.5)) // depth at least maxBBox.Height / 6.5))
                    {
                        // Draw the lines between the points only if a suitable defect has been found
                        nextHsvFrame.Draw(new LineSegment2D(startPoint, defectPoint), new Hsv(60, 255, 255), 2);
                        nextHsvFrame.Draw(new LineSegment2D(defectPoint, endPoint), new Hsv(120, 255, 255), 2);
                        fingerNum++;
                    }

                    // Draw all the points
                    nextHsvFrame.Draw(new CircleF(startPoint, 5f), new Hsv(0, 255, 255), 2);
                    nextHsvFrame.Draw(new CircleF(defectPoint, 5f), new Hsv(80, 255, 255), 5);
                    nextHsvFrame.Draw(new CircleF(endPoint, 5f), new Hsv(160, 255, 255), 4);
                    nextHsvFrame.Draw(new CircleF(new Point((int) maxCOG.X, (int) maxCOG.Y), 5f), new Hsv(160, 0, 255),
                        4);
                }

                // Most people don't have more than 5 fingers :-)
                if (fingerNum > 5) fingerNum = 5;
                nextHsvFrame.Draw(fingerNum.ToString(), new Point(0, 110), FontFace.HersheyPlain, 10, new Hsv(0, 0, 255),
                    3);
            }
        }
    }
}