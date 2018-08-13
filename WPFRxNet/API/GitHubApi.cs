using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using WPFRxNet.Models;

namespace WPFRxNet.API
{
    public class GitHubApi : RemoteApi
    {
        public IObservable<List<GitHubUser>> GetGitHubUsers(long sinceValue)
        {
            var request = new RestRequest {Resource = "users"};

            request.AddQueryParameter("since", sinceValue.ToString());
            request.RootElement = "users";
            return ExecuteAsync<List<GitHubUser>>(request).ToObservable();
        }
    }
}
