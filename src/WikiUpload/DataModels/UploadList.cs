using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace WikiUpload
{
    public class UploadList : ObservableCollection<UploadFile>
    {
        public void RemoveRange(IList<UploadFile> itemsToRemove)
        {
            foreach (var item in itemsToRemove)
                Remove(item);
        }

        public void AddNewRange(IList<string> items)
        {
            foreach (var item in items)
                Add(new UploadFile { FullPath = item });
        }

        public void AddRange(IList<UploadFile> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void AddFromXml(TextReader textReader)
        {
            List<UploadFile> files;
            var serializer = new XmlSerializer(typeof(List<UploadFile>));
            files = (List<UploadFile>)serializer.Deserialize(textReader);
            AddRange(files);
        }

        public void AddFromXml(string fileName)
        {
            using (var sr = new StreamReader(fileName))
                AddFromXml(sr);
        }

        public void SaveToXml(TextWriter textWriter)
        {
            var serializer = new XmlSerializer(typeof(UploadList));
            serializer.Serialize(textWriter, this);
        }

        public void SaveToXml(string fileName)
        {
            using (var sw = new StreamWriter(fileName))
                SaveToXml(sw);
        }
    }
}
