using System;
using System.Runtime.Serialization;

namespace WikiUpload
{
    [Serializable]
    public class ServerIsBusyException : Exception
    {
        public ServerIsBusyException()
        {
        }

        public ServerIsBusyException(string message) : base(message)
        {
        }

        public ServerIsBusyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServerIsBusyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}