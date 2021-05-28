using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikiUpload
{
    internal class ResponseErrors : IReadOnlyResponseErrors
    {
        private readonly List<ApiError> _errors = new List<ApiError>();

        public void Add(ApiError item) => _errors.Add(item);

        public bool IsAny => _errors.Count > 0;

        public bool IsTokenError => _errors.Any(x => x.Code == "badtoken");

        public bool IsMutsBeLoggedInError => _errors.Any(x => x.Code == "mustbeloggedin");

        public bool IsRateLimitedError => _errors.Any(x => x.Code == "ratelimited");

        public override string ToString()
        {
            var text = new StringBuilder();
            foreach (var error in _errors)
                text.AppendEnclosed(error.Code).Append(error.Info).Append(' ');
            text.RemoveLastCharacter();
            return text.ToString();
        }
    }
}
