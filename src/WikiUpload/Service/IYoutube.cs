using System.Collections.Generic;
using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IYoutube
    {
        Task<IEnumerable<string>> FetchPlasylistViedeoLinksAsync(string playlistId, int maxPlaylistSize);
        string ExtractPlaylistId(string url);
    }
}