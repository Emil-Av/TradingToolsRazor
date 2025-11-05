using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Result<T>
    {
        public T Value { get; private set; }
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }    

        private Result(T value, bool success, string errorMessage)
        {
            Value = value;
            Success = success;
            ErrorMessage = errorMessage;
        }

        public static Result<T> SuccessResult(T value)
        {
            return new Result<T>(value, true, null!);
        }

        public static Result<T> ErrorResult(string errorMessage)
        {
            return new Result<T>(default(T), false, errorMessage);
        }
    }
}
