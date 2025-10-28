using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Results
{
    public class Error
    {
        public string error { get; set; }
        public Error(string message)
        {
            error = message;
        }
        public static implicit operator Error(string message) => new Error(message);
        public bool equals(Error? obj)
        {
            return error.Equals(obj.error);
        }
    }


    
}