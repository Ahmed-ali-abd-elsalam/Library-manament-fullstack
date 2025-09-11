using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException()
        {
        }

        public NotAuthorizedException(string? message) : base(message)
        {
        }

        public NotAuthorizedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
}
