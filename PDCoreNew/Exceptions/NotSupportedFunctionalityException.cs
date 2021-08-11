using PDCoreNew.Extensions;
using System;
using System.Runtime.Serialization;

namespace PDCoreNew.Exceptions
{
    public class NotSupportedFunctionalityException : NotSupportedException
    {
        public NotSupportedFunctionalityException()
        {
        }

        public NotSupportedFunctionalityException(string message) : base(message)
        {

        }

        public NotSupportedFunctionalityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotSupportedFunctionalityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Message
        {
            get
            {
                string message = $"{base.Message} Nazwa funkcjonalności: {TargetSite.Name}. Obiekt: {TargetSite.DeclaringType.GetTypeName()}.";

                return message.TrimStart();
            }
        }
    }
}
