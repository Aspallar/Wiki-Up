using System.Collections.Generic;

namespace WikiUpload
{
    public interface IYoutube
    {
        IEnumerable<string> PlaylistVideos(string playlistId);
    }
}