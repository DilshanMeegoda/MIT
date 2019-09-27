using System;
using System.Net;

namespace WorkFlowManagement.Common
{
    public class CustomHttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public CustomHttpResponseException(HttpStatusCode statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
        }
    }
}