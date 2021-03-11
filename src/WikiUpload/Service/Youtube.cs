using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace WikiUpload
{
    public sealed partial class Youtube : IYoutube, IDisposable
    {
        private YouTubeService _youtubeService;
        private PlaylistItemsResource.ListRequest _playlistItems;

        public Youtube(IHelpers helpers)
        {
            _youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApplicationName = helpers.UserAgent,

                // Create the following class in a separate file to supply
                // the google api key. Mark it as ignored by source controll.
                // 
                // public partial class Youtube
                // {
                //    private const string key = "";
                // }
                ApiKey = key,
            }); ;
            _playlistItems = _youtubeService.PlaylistItems.List("contentDetails");
            _playlistItems.MaxResults = 100;
        }

        public async Task<IEnumerable<string>> FetchPlasylistViedeoLinksAsync(string playlistId, int maxPlaylistLength)
        {
            List<string> videoLinks = new List<string>();
            PlaylistItemListResponse results;

            _playlistItems.PlaylistId = playlistId;
            _playlistItems.PageToken = null;

            do
            {
                results = await _playlistItems.ExecuteAsync().ConfigureAwait(false);
                if (results.PageInfo.TotalResults > maxPlaylistLength)
                    throw new TooManyVideosException();
                foreach (var item in results.Items)
                    videoLinks.Add("https://youtube.com/watch?v=" + item.ContentDetails.VideoId);
                _playlistItems.PageToken = results.NextPageToken;
            } while (results.NextPageToken != null);

            return videoLinks;
        }

        public string ExtractPlaylistId(string url)
        {
            string playlistId = null;

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                if (uri.Scheme == "https" && uri.Host.EndsWith("youtube.com"))
                {
                    var queryParams = HttpUtility.ParseQueryString(uri.Query);
                    playlistId = queryParams.Get("list");
                }
            }

            return playlistId;
        }


        public void Dispose()
        {
            _youtubeService.Dispose();
        }
    }
}
