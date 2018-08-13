using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
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
using System.Reactive.PlatformServices;

namespace WPFRxNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<int> _listOfNumbers = new ObservableCollection<int>();
        private IDisposable _subscription;
        public MainWindow()
        {
            InitializeComponent();
            ListBox.ItemsSource = _listOfNumbers;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            _listOfNumbers.Clear();

            var p = Enumerable.Range(1, 100000).Select(x =>
            {
                Thread.Sleep(100);
                return x * 2;
            }).ToObservable().SubscribeOn(ThreadPoolScheduler.Instance).ObserveOn(DispatcherScheduler.Current);

            _subscription = p.Subscribe(x =>
            {
                _listOfNumbers.Add(x);
            });
        }

        private void OnStop(object sender, RoutedEventArgs e)
        {
            _subscription?.Dispose();
        }

        private void OnOpenModal(object sender, RoutedEventArgs e)
        {
            Modal m = new Modal();
            m.ShowDialog();
        }
    }
}
