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

namespace TMT.iVerify
{
    /// <summary>
    /// Interaction logic for FishpondPrompt.xaml
    /// </summary>
    public partial class cFishpondPrompt : MetroWindow
    {
        public enum eLoadTypes
        {
            None,
            NewCases,
            Fishpond,
            Summons
        }

        public eLoadTypes pCaseType
        {
            set;
            get;
        }

        public cFishpondPrompt()
        {
            pCaseType = eLoadTypes.None;

            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void buttNewCases_Click(object sender, RoutedEventArgs e)
        {
            pCaseType = eLoadTypes.NewCases;
            DialogResult = true;
            this.Close();
        }

        private void buttFishpond_Click(object sender, RoutedEventArgs e)
        {
            pCaseType = eLoadTypes.Fishpond;
            DialogResult = true;
            this.Close();
        }

        private void buttSummons_Click(object sender, RoutedEventArgs e)
        {
            pCaseType = eLoadTypes.Summons;
            DialogResult = true;
            this.Close();
        }
    }
}
