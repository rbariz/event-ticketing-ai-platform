using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.Exceptions
{
    public sealed class RequestValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public RequestValidationException(IReadOnlyDictionary<string, string[]> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }
}
