using System;
using System.Runtime.Serialization;

namespace WikiUpload
{
    [Serializable]
    public class MustBeLoggedInException : Exception
    {
        public MustBeLoggedInException()
        {
        }

        public MustBeLoggedInException(string message) : base(message)
        {
        }

        public MustBeLoggedInException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MustBeLoggedInException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}