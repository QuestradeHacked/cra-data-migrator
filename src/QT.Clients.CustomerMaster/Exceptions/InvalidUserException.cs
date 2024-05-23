using System;
using System.Runtime.Serialization;

namespace QT.Clients.CustomerMaster.Exceptions
{
    [Serializable]
    public class InvalidUserException : CrmQueryException
    {
        public InvalidUserException()
        {
        }

        public InvalidUserException(string message)
            : base(message)
        {
        }

        public InvalidUserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidUserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}