using Kapsch.ITS.Gateway.Models.Capture;
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

namespace TMT.iCapture
{
    /// <summary>
    /// Interaction logic for Reasons.xaml
    /// </summary>
    public partial class cReasons : MetroWindow
    {
        private IList<RejectReasonModel> mReasons = null;
        public IList<RejectReasonModel> pReasons
        {
            set { mReasons = value; }
        }

        private int mPrevReason = -1;
        private int mReasonID = -1;
        public int pReasonID
        {
            get { return mReasonID; }
        }

        public int pPrevReasonID
        {
            set { mPrevReason = value; }
        }

        public cReasons()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            labelReasons.Content = "Reason to reject current Image";

            listBox.Items.Clear();

            for (int i = 0; i < mReasons.Count; i++)
            {
                listBox.Items.Add(mReasons[i]);
                if (mReasons[i].ID == mPrevReason)
                    listBox.SelectedIndex = i;
            }

            listBox.Focus();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void buttonOK_Click(object sender = null, RoutedEventArgs e = null)
        {
            if (listBox.SelectedItem != null)
            {
                RejectReasonModel reas = (RejectReasonModel)listBox.SelectedItem;

                mReasonID = reas.ID;

                DialogResult = true;
                this.Close();
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonOK.IsEnabled = (listBox.SelectedItem != null);
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            buttonOK_Click();
        }
    }
}
