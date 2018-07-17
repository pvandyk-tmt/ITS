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
    /// Interaction logic for Notes.xaml
    /// </summary>
    public partial class cNotes : MetroWindow
    {
        public string pTicketNumber
        {
            get;
            set;
        }

        public string pNotes
        {
            set { textBoxNotes.Text = value; }
            get { return textBoxNotes.Text.Trim(); }
        }

        public cNotes()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            labelNotes.Content = "Notes for '" + pTicketNumber + "'";

            textBoxNotes.SelectAll();
            textBoxNotes.Focus();
        }

        private void buttonSubmit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
