using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class BookDoesnotExist : Exception
    {
        public BookDoesnotExist()
        {
        }

        public BookDoesnotExist(string? message) : base(message)
        {
        }

        public BookDoesnotExist(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
