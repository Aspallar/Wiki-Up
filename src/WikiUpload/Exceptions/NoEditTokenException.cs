using System;
using System.Runtime.Serialization;

namespace WikiUpload
{
    [Serializable]
    internal class NoEditTokenException : Exception
    {
        public NoEditTokenException()
        {
        }

        public NoEditTokenException(string message) : base(message)
        {
        }

        public NoEditTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoEditTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}