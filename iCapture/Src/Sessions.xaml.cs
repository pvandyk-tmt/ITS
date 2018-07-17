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
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Win32;
using System.IO;
using Kapsch.ITS.Gateway.Models.Capture;
using MahApps.Metro.Controls;
using System.Collections;

namespace TMT.iCapture
{
    /// <summary>
    /// Interaction logic for Sessions.xaml
    /// </summary>
    public partial class cSessions : MetroWindow
    {
        private string[] mHeadings = null;
        private GridView mGridView = null;
        private IList<SessionModel> mSessions = null;
        private int mSortColumn = -1;     // Sort on column indicator
        private int mPrevSortColumn = -1; // Previously selected sort column
        private int mSortSeq = 1;
        private int mSelectedIndex = -1;

        public int pSelectedIndex
        {
            get { return mSelectedIndex; }
        }

        public bool pLoadNew
        {
            get;
            set;
        }
        
        public cSessions(string[] headings, ref IList<SessionModel> sessions)
        {
            mHeadings = headings;
            mSessions = sessions;

            InitializeComponent();

            if (mSessions != null && mSessions.Count > 0)
                mSortColumn = mPrevSortColumn = mSessions[0].NothingDoneCol;
            mSortSeq = -1;

            listView.Items.Clear();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < mHeadings[i].Length; i++)
                mHeadings[i] = mHeadings[i].Replace("_", "__");

            listViewPaint();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //head.Click -= new RoutedEventHandler(GridViewColumnHeader_Click);
        }

        private void listViewPaint()
        {
            listView.BeginInit();
            mGridView = new GridView();
            for (int i = 0; i < mHeadings.Length; i++)
            {
                GridViewColumn col = new GridViewColumn();
                GridViewColumnHeader head = new GridViewColumnHeader();
                head.Content = mHeadings[i] + " ";
                head.Click += new RoutedEventHandler(GridViewColumnHeader_Click);
                head.FontWeight = FontWeights.Medium;
                if (i == mSortColumn)
                    head.Background = Brushes.LightGray;
                col.Header = head;
                col.DisplayMemberBinding = new Binding(String.Format("[{0}]", i));
                mGridView.Columns.Add(col);
            }
            listView.View = mGridView;
            listView.EndInit();

            if (mSessions == null || mSessions.Count <= 0)
                return;

            var sessions = new SessionCollection();

            listView.BeginInit();

            Binding binding = new Binding();
            binding.Source = sessions;
            listView.SetBinding(ListView.ItemsSourceProperty, binding);

            sessions.Clear();
            for (int i = 0; i < mSessions.Count; i++)
                if ((radioButtonAll.IsChecked == true) || (radioButtonNew.IsChecked == true && mSessions[i].NothingDone > 0))
                {
                    List<string> details = new List<string>();

                    for (int ii = 0; ii < mSessions[i].Columns.Length; ii++)
                    {
                        details.Add(string.Format("{0}", mSessions[i].Columns[ii]));
                    }
                    sessions.Add(details);
                }

            listView.EndInit();
            listView.Focus();
            listView.SelectedIndex = 0;

            buttonExport.IsEnabled = (listView.Items.Count > 0);
        } 

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader col = sender as GridViewColumnHeader;

            if (col.Content is string && mSessions.Count > 0)
            {
                for(int i = 0; i < mHeadings.Length; i++)
                    if (mHeadings[i].Equals(col.Content.ToString().Trim()))
                    {
                        mSortColumn = i;
                        break;
                    }

                // Sort ascending (== 1) or descending (== -1) if the same column has been chosen
                if (mPrevSortColumn != mSortColumn)
                    mSortSeq = 1;
                else if (mSortSeq == 1)
                    mSortSeq = -1;
                else
                    mSortSeq = 1;
                
                //sorteer hierdie uit:
                if (mSortColumn != -1) 
                    ((List<SessionModel>)mSessions).Sort(lstCompare);

                mPrevSortColumn = mSortColumn;

                listViewPaint();
            }
        }

        private int lstCompare(SessionModel a, SessionModel b)
        {
            int diff = 0;

            if (mSessions[0].Columns[mSortColumn] is DateTime)
            {
                DateTime d1 = (DateTime)a.Columns[mSortColumn];
                DateTime d2 = (DateTime)b.Columns[mSortColumn];

                diff = DateTime.Compare(d1, d2);
            }
            else if (mSessions[0].Columns[mSortColumn] is decimal)
            {
                decimal d1 = (decimal)a.Columns[mSortColumn];
                decimal d2 = (decimal)b.Columns[mSortColumn];

                diff = decimal.Compare(d1, d2);
            }
            else if (mSessions[0].Columns[mSortColumn] is int)
            {
                int d1 = (int)a.Columns[mSortColumn];
                int d2 = (int)b.Columns[mSortColumn];

                diff = d1 - d2;
            }
            else
            {
                diff = string.Compare(a.Columns[mSortColumn].ToString(), b.Columns[mSortColumn].ToString());
            }

            if (diff != 0)
                return mSortSeq * diff;

            // if selected column values are equal then sort thereafter on Date
            if (mSortColumn != mSessions[0].CamDateCol)
            {
                if (mSortSeq > 0)
                    diff = string.Compare(a.Columns[mSessions[0].CamDateCol].ToString(), b.Columns[mSessions[0].CamDateCol].ToString());
                else
                    diff = string.Compare(b.Columns[mSessions[0].CamDateCol].ToString(), a.Columns[mSessions[0].CamDateCol].ToString());
            }

            return diff;
        }

        private void radioButtonNew_Click(object sender, RoutedEventArgs e)
        {
            listViewPaint();
        }

        private void radioButtonAll_Click(object sender, RoutedEventArgs e)
        {
            listViewPaint();
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonAll.IsEnabled = buttonNew.IsEnabled = (listView.SelectedIndex >= 0);
            if (buttonNew.IsEnabled)
            {
                setSelectedIndex();
                pLoadNew = buttonNew.IsEnabled = (mSessions[mSelectedIndex].NothingDone > 0);
            }
        }

        private void setSelectedIndex()
        {
            List<string> itm = (List<string>)listView.SelectedItem;

            for(int i = 0; i < mSessions.Count; i++)
                if (mSessions[i].Columns[0].ToString() == itm[0] &&
                    mSessions[i].Columns[1].ToString() == itm[1] &&
                    mSessions[i].Columns[2].ToString() == itm[2])
                {
                    mSelectedIndex = i;
                    break;
                }
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listView.SelectedIndex >= 0)
            {
                setSelectedIndex();
                pLoadNew = (mSessions[mSelectedIndex].NothingDone > 0);
                DialogResult = true;
                this.Close();
            }
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex >= 0)
            {
                setSelectedIndex();
                pLoadNew = (mSessions[mSelectedIndex].NothingDone > 0);
                DialogResult = true;
                this.Close();
            }
        }

        private void buttonAll_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex >= 0)
            {
                setSelectedIndex();
                pLoadNew = false;
                DialogResult = true;
                this.Close();
            }
        }

        private void buttonExport_Click(object sender, RoutedEventArgs e)
        {
            buttonExport.IsEnabled = false;

            var dlg = new SaveFileDialog();
            //dlg.FileName = "Document";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV files (.csv)|*.csv";
            if (dlg.ShowDialog() == true)
            {
                var str = new StringBuilder();

                this.Cursor = Cursors.Wait;

                if (File.Exists(dlg.FileName))
                    File.Delete(dlg.FileName);

                for (int i = 0; i < mHeadings.Length; i++)
                {
                    if (i > 0)
                        str.Append(",");
                    str.Append(mHeadings[i].Replace(",", "_"));
                }

                var file = new StreamWriter(dlg.FileName);

                file.WriteLine(str.ToString());

                try
                {
                    foreach (SessionModel ses in mSessions)
                    {
                        if ((radioButtonAll.IsChecked == true) || (radioButtonNew.IsChecked == true && ses.NothingDone > 0))
                        {
                            str.Clear();
                            for (int i = 0; i < ses.Columns.Length; i++)
                            {
                                if (i > 0)
                                    str.Append(",");
                                
                                str.Append(ses.Columns[i] == null ? string.Empty : ses.Columns[i].ToString().Replace(",", "_"));
                            }
                            file.WriteLine(str.ToString());
                        }
                    }

                    file.Close();

                    MessageBox.Show(this, "Export to file '" + dlg.FileName + "' completed.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Export to file '" + dlg.FileName + "' failed. \n\n" + ex.Message, "Export error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                this.Cursor = null;
            }

            buttonExport.IsEnabled = true;
        }

    }

    public class SessionCollection : ObservableCollection<List<string>>
    {
        public SessionCollection()
            : base()
        {
        }
    } 
}
