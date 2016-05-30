using Emgu.CV;
using Emgu.CV.Structure;
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
        private readonly Image<Bgr, byte> _imageFrame = new Image<Bgr, byte>(@"..\..\Resources\sudoku-original.jpg");

        public Form1()
        {
            InitializeComponent();
            imageBox1.Image = _imageFrame;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
