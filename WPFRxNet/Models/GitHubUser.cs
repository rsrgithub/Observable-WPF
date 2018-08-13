using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WPFRxNet.Models
{
    public class GitHubUser
    {
        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }

        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }        
    }
}
