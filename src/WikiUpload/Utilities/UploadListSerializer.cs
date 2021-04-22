using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace WikiUpload
{
    public class UploadListSerializer : IUploadListSerializer
    {
        public List<UploadFile> Deserialize(TextReader textReader)
        {
            List<UploadFile> files;
            var serializer = new XmlSerializer(typeof(List<UploadFile>));
            files = (List<UploadFile>)serializer.Deserialize(textReader);
            return files;
        }

        public List<UploadFile> Deserialize(string fileName)
        {
            using var sr = new StreamReader(fileName);
            return Deserialize(sr);
        }

        public void Serialize(TextWriter textWriter, UploadList uploadList)
        {
            var serializer = new XmlSerializer(typeof(UploadList));
            serializer.Serialize(textWriter, uploadList);
        }

        public void Serialize(string fileName, UploadList uploadList)
        {
            using var sw = new StreamWriter(fileName);
            Serialize(sw, uploadList);
        }
    }
}
