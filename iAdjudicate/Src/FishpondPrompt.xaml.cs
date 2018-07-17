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
    /// Interaction logic for FishpondPrompt.xaml
    /// </summary>
    public partial class cFishpondPrompt : MetroWindow
    {
        public bool pNewCases
        {
            set;
            get;
        }

        public cFishpondPrompt()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void buttNewCases_Click(object sender, RoutedEventArgs e)
        {
            pNewCases = true;
            DialogResult = true;
            this.Close();
        }

        private void buttFishpond_Click(object sender, RoutedEventArgs e)
        {
            pNewCases = false;
            DialogResult = true;
            this.Close();
        }
    }
}
