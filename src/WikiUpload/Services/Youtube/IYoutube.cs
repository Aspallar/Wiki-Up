using System.Collections.Generic;
using System.Threading.Tasks;

namespace WikiUpload
{
    internal interface IYoutube
    {
        Task<IEnumerable<string>> FetchPlasylistViedeoLinksAsync(string playlistId, int maxPlaylistSize);
        string ExtractPlaylistId(string url);
    }
}