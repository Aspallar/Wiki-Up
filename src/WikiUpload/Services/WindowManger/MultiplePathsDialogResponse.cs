using System.Collections.Generic;

namespace WikiUpload
{
    internal class MultiplePathsDialogResponse
    {
        public bool Ok { get; set; }
        public IList<string> Paths { get; set; }
    }

}
