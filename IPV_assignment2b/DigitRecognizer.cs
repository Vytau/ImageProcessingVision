using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using System;
using System.IO;

namespace IPV_assignment2b
{
    class DigitRecognizer
    {
        private KNearest knn = new KNearest();
        private const int MAX_NUM_IMAGES = 60000;

        private int readFlippedInteger(BinaryReader fp)
        {
            byte[] temp = new byte[4];
            fp.Read(temp, 0, 4);
            Array.Reverse(temp);
            return BitConverter.ToInt32(temp, 0);
        }

        public float classify(Image<Gray, byte> img)
        {
            Image<Gray, byte> imgResized = img.Resize(28, 28, Inter.Linear);
            imgResized._ThresholdBinary(new Gray(128), new Gray(255));
            Matrix<float> cloneImg = new Matrix<float>(1, 28 * 28);
            for (int i = 0; i < 28; i++)
                for (int j = 0; j < 28; j++)
                {
                    float tmp = (float)imgResized[i, j].Intensity;
                    cloneImg[0, i * 28 + j] = tmp;
                }
            knn.DefaultK = 10;
            float t = knn.Predict(cloneImg);
            return t;
        }

        public bool train(string trainFileName, string labelFileName)
        {
            FileStream trainFile = new FileStream(trainFileName, FileMode.Open);
            FileStream labelFile = new FileStream(labelFileName, FileMode.Open);
            BinaryReader fp = new BinaryReader(trainFile);
            BinaryReader fp2 = new BinaryReader(labelFile);

            int magicNumber = readFlippedInteger(fp);
            int numImages = readFlippedInteger(fp);
            int numRows = readFlippedInteger(fp);
            int numCols = readFlippedInteger(fp);
            if (numImages > MAX_NUM_IMAGES) numImages = MAX_NUM_IMAGES;
            int size = numRows * numCols;


            Matrix<float> trainingVectors = new Matrix<float>(numImages, size);
            Matrix<float> trainingClasses = new Matrix<float>(numImages, 1);

            byte[] temp = new byte[size];
            byte[] tempClass = new byte[1];
            fp2.ReadInt64();            
            for (int i = 0; i < numImages; i++)
            {
                fp.Read(temp, 0, size);
                fp2.Read(tempClass, 0, 1);

                trainingClasses[i, 0] = (float)tempClass[0];

                for (int k = 0; k < size; k++)
                    trainingVectors[i, k] = (float)temp[k];
            }

            knn.Train(new TrainData(trainingVectors, Emgu.CV.ML.MlEnum.DataLayoutType.RowSample, trainingClasses));
            fp.Close();
            fp2.Close();

            return true;
        }
    }
}
