using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;

namespace WikiUpload
{
    public partial class Youtube : IYoutube, IDisposable
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

        public IEnumerable<string> PlaylistVideos(string playlistId)
        {
            List<string> videos = new List<string>();

            _playlistItems.PlaylistId = playlistId;

            var results = _playlistItems.Execute();
            foreach (var item in results.Items)
                videos.Add("https://youtube.com/watch?v=" + item.ContentDetails.VideoId);

            return videos;
        }

        public void Dispose()
        {
            _youtubeService.Dispose();
        }
    }
}
