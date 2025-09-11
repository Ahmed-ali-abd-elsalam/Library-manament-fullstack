using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class RoleNotAvailableException : Exception
    {
        public RoleNotAvailableException()
        {
        }

        public RoleNotAvailableException(string? message) : base(message)
        {
        }

        public RoleNotAvailableException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
