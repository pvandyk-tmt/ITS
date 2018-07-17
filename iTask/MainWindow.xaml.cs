using Kapsch.Gateway.Models.Shared;
using Kapsch.ITS.Gateway.Clients;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace iTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private string userName = string.Empty;
        private string ipAddress = string.Empty;
        private string footerLabel = string.Empty;
        
        public MainWindow()
        {
            InitializeComponent();

            //MessageBoxOptions = MessageBoxOptions.None;
            //SetupUICulture();

            //DataContext = viewModel;
            //footerLabel = labelFooter.Content.ToString();

            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += dispatcherTimer_Tick;

            userName = "";// UserSecurityService.AuthenticatedUser.UserName;
            ipAddress = Environment.MachineName;

            Timer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Set text content
            //labelFooter.Content = string.Format(CultureInfo.InvariantCulture, footerLabel, userName, ipAddress, DateTime.Now);

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

       

        private DispatcherTimer Timer
        {
            get { return timer; }
        }

        private MessageBoxOptions MessageBoxOptions { get; set; }


        private void ModuleWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();

            //var ok = new ObservableCollection<TestClass>();
            //ok.Add(new TestClass() { Category = "Capture", Number = 100 });
            //ok.Add(new TestClass() { Category = "ZamTIS", Number = 10 });
            //ok.Add(new TestClass() { Category = "Adjudicate", Number = 201 });
            //ok.Add(new TestClass() { Category = "Adjudicate Fishpond", Number = 13 });
            //ok.Add(new TestClass() { Category = "VRS Integration", Number = 20 });

            //var yellow = new ObservableCollection<TestClass>();
            //yellow.Add(new TestClass() { Category = "Capture", Number = 20 });
            //yellow.Add(new TestClass() { Category = "ZamTIS", Number = 7 });
            //yellow.Add(new TestClass() { Category = "Adjudicate", Number = 201 });
            //yellow.Add(new TestClass() { Category = "Adjudicate Fishpond", Number = 13 });
            //yellow.Add(new TestClass() { Category = "VRS Integration", Number = 10 });

            //var red = new ObservableCollection<TestClass>();
            //red.Add(new TestClass() { Category = "Capture", Number = 10 });
            //red.Add(new TestClass() { Category = "ZamTIS", Number = 4 });
            //red.Add(new TestClass() { Category = "Adjudicate", Number = 50 });
            //red.Add(new TestClass() { Category = "Adjudicate Fishpond", Number = 0 });
            //red.Add(new TestClass() { Category = "VRS Integration", Number = 10 });

            //ChartSeries1.ItemsSource = ok;
            //ChartSeries2.ItemsSource = yellow;
            //ChartSeries3.ItemsSource = red;
        }

        private void ChartSeries_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((MainChart.SelectedItem == null) || (MainChart.SelectedItem as TestClass == null)) return;

            var category = ((TestClass)MainChart.SelectedItem).Category;

            
        }

        private void MainChart_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainChart.SelectedItem = null;
        }

        private void Refresh()
        {
            try
            {
                var tasksService = new TasksService(Startup.AuthenticatedUser.SessionToken);
                var tasks = tasksService.GetTasks(false);

                ChartSeries1.ItemsSource = tasks.Select(f => new TestClass { Category = f.Name, Number = f.Low }).ToList();
                ChartSeries2.ItemsSource = tasks.Select(f => new TestClass { Category = f.Name, Number = f.Medium }).ToList();
                ChartSeries3.ItemsSource = tasks.Select(f => new TestClass { Category = f.Name, Number = f.Critical }).ToList();
            }
            catch (GatewayException ex)
            {
                MessageBox.Show(
                     ex.Message,
                     "Tasks",
                     MessageBoxButton.OK,
                     MessageBoxImage.Stop,
                     MessageBoxResult.OK,
                     MessageBoxOptions);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                     ex.Message,
                     "Tasks",
                     MessageBoxButton.OK,
                     MessageBoxImage.Stop,
                     MessageBoxResult.OK,
                     MessageBoxOptions);
            }
        }

        public class TestClass
        {
            public string Category { get; set; }
            public long Number { get; set; }
        }
    }
}
