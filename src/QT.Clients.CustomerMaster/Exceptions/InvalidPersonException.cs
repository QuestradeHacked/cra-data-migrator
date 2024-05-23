using System;
using System.Runtime.Serialization;

namespace QT.Clients.CustomerMaster.Exceptions
{
    [Serializable]
    public class InvalidPersonException : CrmQueryException
    {
        public InvalidPersonException()
        {
        }

        public InvalidPersonException(string message)
            : base(message)
        {
        }

        public InvalidPersonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidPersonException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}