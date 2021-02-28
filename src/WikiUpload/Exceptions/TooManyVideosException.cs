using System;
using System.Runtime.Serialization;

namespace WikiUpload
{
    [Serializable]
    internal class TooManyVideosException : Exception
    {
        public TooManyVideosException()
        {
        }

        public TooManyVideosException(string message) : base(message)
        {
        }

        public TooManyVideosException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TooManyVideosException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}