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
using System.IO;
using MahApps.Metro.Controls;
using Kapsch.ITS.Gateway.Models.Verify;

namespace TMT.iVerify
{
    /// <summary>
    /// Interaction logic for FishpondList.xaml
    /// </summary>
    public partial class cFishpondList : MetroWindow
    {
        public IList<FishpondInfoModel> pFishpondCases { set; get; }
        public string pTicketNo { set; get; }
        public bool pDoExit { set; get; }
        public string pSortColumn { set; get; }
        public int pSortOrder { set; get; }
        public int pSelectIndex { set; get; }

        public cFishpondList()
        {
            InitializeComponent();

            pDoExit = true; // If user presses 'x' top right to close, handle as a 'exit' choice
            pTicketNo = string.Empty;
            pSortColumn = string.Empty;
            pSortOrder = 1;
            pSelectIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (pFishpondCases.Count > 0 && !pSortColumn.Equals(string.Empty))
                ((List <FishpondInfoModel> )pFishpondCases).Sort(compare);

            listCases.ItemsSource = pFishpondCases;
            buttExport.IsEnabled = (pFishpondCases.Count > 0);

            position();
        }

        private int compare(FishpondInfoModel a, FishpondInfoModel b)
        {
            int retVal = 0;

            switch (pSortColumn)
            {
                case "VehicleRegNo":
                    retVal = string.Compare(a.VehicleRegistration, b.VehicleRegistration);
                    break;
                case "VerifyDate":
                    retVal = string.Compare(a.VerifyDate.ToString("yyyy MM dd"), b.VerifyDate.ToString("yyyy MM dd"));
                    break;
                case "TicketDate":
                    retVal = string.Compare(a.TicketDate.ToString("yyyy MM dd"), b.TicketDate.ToString("yyyy MM dd"));
                    break;
                case "Rejected":
                    retVal = string.Compare(a.TimesRejected.ToString("0000"), b.TimesRejected.ToString("0000"));
                    break;
                case "VehicleMake":
                    retVal = string.Compare(a.VehicleMake, b.VehicleMake);
                    break;
                case "VehicleModel":
                    retVal = string.Compare(a.VehicleModel, b.VehicleModel);
                    break;
                case "RejectReason":
                    retVal = string.Compare(a.RejectReason, b.RejectReason);
                    break;
                case "RejectedBy":
                    retVal = string.Compare(a.RejectBy, b.RejectBy);
                    break;
                case "LockedBy":
                    retVal = string.Compare(a.LockedBy, b.LockedBy);
                    break;
                default: // TicketNo
                    retVal = string.Compare(a.TicketNumber, b.TicketNumber);
                    break;
            }

            retVal *= pSortOrder;

            if (retVal == 0)
                retVal = string.Compare(a.TicketNumber, b.TicketNumber);

            return retVal;
        }

        private void position()
        {
            if (pSelectIndex < listCases.Items.Count && pSelectIndex >= 0)
            {
                listCases.SelectedIndex = pSelectIndex;
                listCases.ScrollIntoView(listCases.SelectedItem);
            }
        }

        private void listCases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttChoose.IsEnabled = (listCases.SelectedIndex >= 0);
            pSelectIndex = listCases.SelectedIndex;
            if (pSelectIndex >= 0)
            {
                cDataAccess.cFishpondInfo itm = (cDataAccess.cFishpondInfo)listCases.SelectedItem;
                pTicketNo = itm.pTicketNo;
            }
        }

        private void listCases_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            buttChoose_Click(null, null);
        }

        private void buttChoose_Click(object sender, RoutedEventArgs e)
        {
            pDoExit = false;
            if (listCases.SelectedIndex >= 0)
            {
                cDataAccess.cFishpondInfo itm = (cDataAccess.cFishpondInfo)listCases.SelectedItem;
                pTicketNo = itm.pTicketNo;
                this.Close();
            }
            else
                pTicketNo = string.Empty;
        }

        private void buttExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Exit the Fishpond Verifications?", "Verify Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                pDoExit = true;
                this.Close();
            }
        }

        /// Converts a value to how it should output in a csv file
        /// If it has a comma, it needs surrounding with double quotes
        /// Eg Sydney, Australia -> "Sydney, Australia"
        /// Also if it contains any double quotes ("), then they need to be replaced with quad quotes[sic] ("")
        /// Eg "Dangerous Dan" McGrew -> """Dangerous Dan"" McGrew"
        /// </summary>
        private string makeCsvFriendly(object value)
        {
            if (value == null) 
                return "";

            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }

            string output = value.ToString();
            if (output.Contains(",") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';
            return output;
        }


        private void buttSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = txtSearch.Text.Trim();

            for (int i = 0; i < pFishpondCases.Count; i++)
                if (txtSearch.Text == pFishpondCases[i].TicketNumber)
                {
                    pSelectIndex = listCases.SelectedIndex = i;
                    buttChoose.IsEnabled = true;
                    break;
                }
        }

        private void buttExport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            buttExport.IsEnabled = false;
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV files (*.csv)|*.csv";
            dlg.CheckFileExists = false;
            dlg.CreatePrompt = true;

            if (dlg.ShowDialog(this) == true)
            {
                try
                {
                    if (File.Exists(dlg.FileName))
                        File.Delete(dlg.FileName);

                    using (StreamWriter sw = File.CreateText(dlg.FileName))
                    {
                        foreach (FishpondInfoModel F in pFishpondCases)
                        {
                            sw.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                makeCsvFriendly(F.TicketNumber),
                                makeCsvFriendly(F.VehicleRegistration),
                                makeCsvFriendly(F.TicketDate),
                                makeCsvFriendly(F.VehicleMake),
                                makeCsvFriendly(F.VehicleModel),
                                makeCsvFriendly(F.RejectReason),
                                makeCsvFriendly(F.RejectBy),
                                makeCsvFriendly(F.VerifyDate),
                                makeCsvFriendly(F.TimesRejected),
                                makeCsvFriendly(F.LockedBy));
                        }

                        sw.Close();
                    }

                    MessageBox.Show("Exported to file '" + dlg.FileName + "' done.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error exporting fishpond data to '" + dlg.FileName + "' \n" + ex.Message, "Export", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            buttExport.IsEnabled = true;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            buttSearch.IsEnabled = txtSearch.Text.Length > 0;
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader hdr = (GridViewColumnHeader)sender;

            if (hdr.Name != pSortColumn)
            {
                pSortColumn = hdr.Name;
                pSortOrder = 1;
            }
            else
                pSortOrder *= -1;

            listCases.ItemsSource = null;
            ((List< FishpondInfoModel>)pFishpondCases).Sort(compare);
            listCases.ItemsSource = pFishpondCases;
            listCases.InvalidateVisual();

            position();
        }

    }
}
