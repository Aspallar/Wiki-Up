using System.IO;

namespace WikiUpload
{
    public interface IUploadListSerializer
    {
        void Add(string fileName, UploadList uploadList);
        void Add(TextReader textReader, UploadList uploadList);
        void Save(string fileName, UploadList uploadList);
        void Save(TextWriter textWriter, UploadList uploadList);
    }
}