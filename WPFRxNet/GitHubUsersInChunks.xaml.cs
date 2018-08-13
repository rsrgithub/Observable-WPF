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
        private Queue<GitHubUser> _buffer = new Queue<GitHubUser>();
        private const int RefreshButonUserCount = 5;
        private const int RemoveButonUserCount = 1;
        private long _sinceId = 1;
        public GitHubUsersInChunks()
        {
            InitializeComponent();
            ListBox.ItemsSource = _users;

            SetupStreams();
        }

        private void SetupStreams()
        {
            //this is the refresh button stream
            var refreshButtonStream = Observable.FromEventPattern(RefreshButton, "Click");
            //_responseStream = _gitHubApi.GetGitHubUsers(1);

            /**for each click of Refresh button we need to do following -
             * Check if buffer has requested number of users if not then request from server and fill the buffer
             */
            var refreshClickStream = refreshButtonStream.StartWith((EventPattern<object>)null).Select(click =>
            {
                if (_buffer.Count >= RefreshButonUserCount)
                {
                    //yes buffer has requested number of user
                    //deque those number of user
                    return Observable.Return(_buffer.DequeueChunk(RefreshButonUserCount).ToList());
                }

                return _gitHubApi.GetGitHubUsers(_sinceId)
                                 .Where(x => x != null && x.Count > 0)
                                 .Select(users =>
                                  {
                                      users.ForEach(u => _buffer.Enqueue(u));

                                      _sinceId = users[users.Count - 1].Id;

                                      if (_buffer.Count >= RefreshButonUserCount)
                                      {
                                          //yes buffer has requested number of user
                                          //deque those number of user
                                          return _buffer.DequeueChunk(RefreshButonUserCount).ToList();
                                      }

                                      return new List<GitHubUser>();
                                  });
            })
            .SelectMany(x => x);


            refreshClickStream.ObserveOn(Dispatcher.CurrentDispatcher).Subscribe((users) =>
            {
                _users.Clear();
                users.ForEach(u => _users.Add(u));
            });
        }
    }

    public static class QueueExtensions
    {
        public static IEnumerable<T> DequeueChunk<T>(this Queue<T> queue, int chunkSize)
        {
            for (int i = 0; i < chunkSize && queue.Count > 0; i++)
            {
                yield return queue.Dequeue();
            }
        }
    }
}

