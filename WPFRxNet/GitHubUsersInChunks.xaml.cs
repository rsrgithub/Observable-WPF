using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPFRxNet.API;
using WPFRxNet.Models;

namespace WPFRxNet
{
    /// <summary>
    /// Interaction logic for GitHubUsersInChunks.xaml
    /// </summary>
    public partial class GitHubUsersInChunks : Window
    {
        private readonly GitHubApi _gitHubApi = new GitHubApi();
        private readonly ObservableCollection<GitHubUser> _users = new ObservableCollection<GitHubUser>();
        private IObservable<List<GitHubUser>> _responseStream;
        public GitHubUsersInChunks()
        {
            InitializeComponent();
            ListBox.ItemsSource = _users;

            SetupStreams();
        }

        private void SetupStreams()
        {
            var refreshButtonStream = Observable.FromEventPattern(RefreshButton, "Click");

            _responseStream = _gitHubApi.GetGitHubUsers(1);

            var refreshStream = refreshButtonStream.StartWith((EventPattern<object>) null)
                .CombineLatest(_responseStream, (click, listOfUsers) => { return listOfUsers.Take(5).ToList(); });

            refreshStream.ObserveOn(Dispatcher.CurrentDispatcher).Subscribe((users) =>
            {
                _users.Clear();
                users.ForEach(u => _users.Add(u));
            });
        }
    }
}
