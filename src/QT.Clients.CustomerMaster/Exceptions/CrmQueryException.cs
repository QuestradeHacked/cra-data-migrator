using System;
using System.Runtime.Serialization;

namespace QT.Clients.CustomerMaster.Exceptions
{
    [Serializable]
    public class CrmQueryException : Exception
    {
        public CrmQueryException()
        {
        }

        public CrmQueryException(string message)
            : base(message)
        {
        }

        public CrmQueryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CrmQueryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}