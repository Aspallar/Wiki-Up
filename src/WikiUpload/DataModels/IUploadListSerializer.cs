using System.Collections.Generic;
using System.IO;

namespace WikiUpload
{
    internal interface IUploadListSerializer
    {
        List<UploadFile> Deserialize(TextReader textReader);
        List<UploadFile> Deserialize(string fileName);

        void Serialize(string fileName, UploadList uploadList);

        void Serialize(TextWriter textWriter, UploadList uploadList);
    }
}