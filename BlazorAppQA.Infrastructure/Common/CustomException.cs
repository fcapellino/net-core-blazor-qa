using System;

namespace BlazorAppQA.Infrastructure.Common
{
    public class CustomException : Exception
    {
        public CustomException(string message)
            : base(message) { }
    }
}
