using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class RecordDoesnotExist : Exception
    {
        public RecordDoesnotExist()
        {
        }

        public RecordDoesnotExist(string? message) : base(message)
        {
        }

        public RecordDoesnotExist(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
}
