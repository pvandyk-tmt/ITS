using Kapsch.ITS.Gateway.Models.Adjudicate;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Kapsch.ITS.App;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TMT.iAdjudicate
{
    /// <summary>
    /// Interaction logic for Additionals.xaml
    /// </summary>
    public partial class cAdditionals : MetroWindow
    {
        private IList<OffenceCodeModel> mCodes = null;
        private IList<OffenceCodeModel> mOffenseItems = new List<OffenceCodeModel>();

        private struct sOffenseItem
        {
            public OffenceCodeModel mData;

            public override string ToString()
            {
                if (mData.Amount == 99999)
                    return mData.Code.ToString() + "  NAG  " + mData.Description;
                else
                    return mData.Code.ToString() + "  " + mData.Amount.ToString("0.00") + "  " + mData.Description;
            }
        }

        public IList<OffenceCodeModel> pOffenceItems
        {
            set { mOffenseItems = value; }
            get { return mOffenseItems; }
        }

        public IList<OffenceCodeModel> pAdditionalCodes
        {
            set { mCodes = value; }
        }

        public cAdditionals()
        {
            InitializeComponent();

            textBoxAmount.Text = "0.00";
            textBoxDescription.Text = string.Empty;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxCodes.Items.Clear();

            if (mCodes == null)
                return;

            for (int i = 0; i < mCodes.Count; i++)
                comboBoxCodes.Items.Add(mCodes[i]);

            listBox.Items.Clear();
            if (mOffenseItems.Count > 0)
            {
                for (int i = 0; i < mOffenseItems.Count; i++)
                {
                    sOffenseItem itm;

                    itm.mData = mOffenseItems[i];

                    listBox.Items.Add(itm);
                }
                buttonClear.IsEnabled = buttonDone.IsEnabled = true;
            }

            comboBoxCodes.Focus();
        }

        private void buttonDone_Click(object sender, RoutedEventArgs e)
        {
            mOffenseItems.Clear();
            if (listBox.Items.Count > 0)
            {
                sOffenseItem itm;

                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    itm = (sOffenseItem)listBox.Items[i];
                    mOffenseItems.Add(itm.mData);
                }
            }

            DialogResult = true;
            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void comboBoxCodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonAdd.IsEnabled = (comboBoxCodes.SelectedItem != null);

            if (comboBoxCodes.SelectedItem != null)
            {
                OffenceCodeModel off;

                off = (OffenceCodeModel)comboBoxCodes.SelectedItem;
                if (off.Amount == 99999)
                    textBoxAmount.Text = "NAG";
                else
                    textBoxAmount.Text = off.Amount.ToString("0.00");
                textBoxDescription.Text = off.Description;
                buttonAdd.Focus();
            }
            else
            {
                textBoxAmount.Text = "0.00";
                textBoxDescription.Text = string.Empty;
                comboBoxCodes.Focus();
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            buttonAdd.IsEnabled = false;
            if (comboBoxCodes.SelectedItem == null)
                return;

            sOffenseItem itm;
            itm.mData = (OffenceCodeModel)comboBoxCodes.SelectedItem;
            listBox.Items.Add(itm);
            buttonClear.IsEnabled = buttonDone.IsEnabled = true;
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            buttonClear.IsEnabled = buttonRemove.IsEnabled = false;
            listBox.Items.Clear();
            buttonAdd.IsEnabled = (comboBoxCodes.SelectedItem != null);
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            buttonRemove.IsEnabled = false;
            if (listBox.SelectedItem != null)
            {
                listBox.Items.Remove(listBox.SelectedItem);
            }
            buttonAdd.IsEnabled = (comboBoxCodes.SelectedItem != null);
            buttonClear.IsEnabled = (listBox.Items.Count > 0);
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonRemove.IsEnabled = (listBox.SelectedItem != null);
            buttonAdd.IsEnabled = (comboBoxCodes.SelectedItem != null);
        }
    }
}
