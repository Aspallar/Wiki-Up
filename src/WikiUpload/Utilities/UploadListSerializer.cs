using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace WikiUpload
{
    public class UploadListSerializer : IUploadListSerializer
    {
        public void Add(TextReader textReader, UploadList uploadList)
        {
            List<UploadFile> files;
            var serializer = new XmlSerializer(typeof(List<UploadFile>));
            files = (List<UploadFile>)serializer.Deserialize(textReader);
            uploadList.AddRange(files);
        }

        public void Add(string fileName, UploadList uploadList)
        {
            using (var sr = new StreamReader(fileName))
                Add(sr, uploadList);
        }

        public void Save(TextWriter textWriter, UploadList uploadList)
        {
            var serializer = new XmlSerializer(typeof(UploadList));
            serializer.Serialize(textWriter, uploadList);
        }

        public void Save(string fileName, UploadList uploadList)
        {
            using (var sw = new StreamWriter(fileName))
                Save(sw, uploadList);
        }
    }
}
