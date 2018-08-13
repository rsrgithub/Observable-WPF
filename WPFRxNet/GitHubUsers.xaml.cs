using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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
    /// Interaction logic for GitHubUsers.xaml
    /// </summary>
    public partial class GitHubUsers : Window
    {
        private readonly GitHubApi _gitHubApi = new GitHubApi();
        private readonly ObservableCollection<GitHubUser> _users = new ObservableCollection<GitHubUser>();
        private IObservable<GitHubUser> _suggestion1Stream;
        private IObservable<GitHubUser> _suggestion2Stream;
        private IObservable<GitHubUser> _suggestion3Stream;
        private IObservable<EventPattern<object>> _refreshButtonStream;
        private IObservable<long> _requestStream;
        private IObservable<List<GitHubUser>> _responseStream;
        private IObservable<EventPattern<object>> _close1ButtonStream;
        private IObservable<EventPattern<object>> _close2ButtonStream;
        private IObservable<EventPattern<object>> _close3ButtonStream;
        private long _sinceId = 1;
        private Random _random = new Random();
        public GitHubUsers()
        {
            InitializeComponent();
            //ListBox.ItemsSource = _users;

            //set up the stream
            SetupStreams();
        }

        private void SetupStreams()
        {
            _refreshButtonStream = Observable.FromEventPattern(RefreshButton, "Click");
            _close1ButtonStream = Observable.FromEventPattern(Suggestion1CloseButton, "Click");
            _close2ButtonStream = Observable.FromEventPattern(Suggestion2CloseButton, "Click");
            _close3ButtonStream = Observable.FromEventPattern(Suggestion3CloseButton, "Click");

            _requestStream = _refreshButtonStream.StartWith((EventPattern<object>) null)
                .Select((x) =>
                {
                    var randomOffset = _random.Next();
                    return (long) randomOffset;
                });

            _responseStream = _requestStream.SelectMany((sinceId) => _gitHubApi.GetGitHubUsers(_sinceId));

            var suggestion1Stream = CreateSuggestionStream(_close1ButtonStream);
            var suggestion2Stream = CreateSuggestionStream(_close2ButtonStream);
            var suggestion3Stream = CreateSuggestionStream(_close3ButtonStream);


            suggestion1Stream.ObserveOn(Dispatcher.CurrentDispatcher).Subscribe((user) =>
            {
                if (user != null)
                {
                    Suggestion1Name.Text = user.Login;
                }
            });

            suggestion2Stream.ObserveOn(Dispatcher.CurrentDispatcher).Subscribe((user) =>
            {
                if (user != null)
                    Suggestion2Name.Text = user.Login;
            });

            suggestion3Stream.ObserveOn(Dispatcher.CurrentDispatcher).Subscribe((user) =>
            {
                if (user != null)
                    Suggestion3Name.Text = user.Login;
            });
        }

        private IObservable<GitHubUser> CreateSuggestionStream(IObservable<EventPattern<object>> closeClickStream)
        {
            return closeClickStream.StartWith(new EventPattern<object>(null, null))
                .CombineLatest(_responseStream, (pattern, list) => { return list[_random.Next(0, list.Count)]; })
                .Merge(_refreshButtonStream.Select(x => (GitHubUser) null))
                .StartWith((GitHubUser) null);
            //return closeClickStream.StartWith(new EventPattern<object>(null, null))
            //    .CombineLatest(_responseStream, (pattern, list) => { return list[_random.Next(0, list.Count)]; });
        }


        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            _users.Clear();

            var gitHubUsers = _gitHubApi.GetGitHubUsers(_sinceId);

            gitHubUsers.ObserveOn(Dispatcher.CurrentDispatcher).Subscribe(users => {
                users.ForEach(user => _users.Add(user));

                if (users.Count > 0)
                {
                    _sinceId = users[users.Count - 1].Id;
                }
            });
        }
    }
}
