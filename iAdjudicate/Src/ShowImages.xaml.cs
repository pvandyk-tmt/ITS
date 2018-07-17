using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TMT.iAdjudicate
{
    /// <summary>
    /// Interaction logic for ShowImages.xaml
    /// </summary>
    public partial class cShowImages : MetroWindow
    {
        public string[] pImages { get; set; }
        private int mImageIndex = 0;

        public cShowImages()
        {
            InitializeComponent();

            pImages = new string[4];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool  multiPage;

            pictureDisplay.loadPicture(pImages[mImageIndex], true, out multiPage);
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            bool multiPage;

            do
            {
                mImageIndex++;

                if (mImageIndex >= pImages.Length)
                    mImageIndex = 0;

                pictureDisplay.loadPicture(pImages[mImageIndex], true, out multiPage);
            }
            while (string.IsNullOrEmpty(pImages[mImageIndex]));
        }
    }
}
