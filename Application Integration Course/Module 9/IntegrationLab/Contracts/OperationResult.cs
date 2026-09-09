using System;

namespace IntegrationLab.Contracts
{
    /// <summary>
    /// What every server handler returns, whichever entry point called it: an HTTP-like
    /// status code and a JSON-serializable body. The postback handler writes both to the
    /// response; the WebMethod returns the body (the status travels inside it).
    /// </summary>
    public sealed class OperationResult
    {
        private OperationResult(int status, object body, string summary)
        {
            this.Status = status;
            this.Body = body;
            this.Summary = summary;
        }

        /// <summary>200, 400 (contract violation) or 404 (unknown key).</summary>
        public int Status { get; }

        /// <summary>The serializable payload.</summary>
        public object Body { get; }

        /// <summary>Short text for the trace, e.g. "(20/150)" or the error message.</summary>
        public string Summary { get; }

        public bool IsSuccess => this.Status >= 200 && this.Status < 300;

        public static OperationResult Ok(object body, string summary = "")
            => new OperationResult(200, body, summary);

        public static OperationResult Fail(int status, string message)
            => new OperationResult(status, new { status, message }, message);
    }

    /// <summary>
    /// Thrown by the store when a request violates the contract (400) or names an
    /// unknown key (404). The controller turns it into an <see cref="OperationResult"/>;
    /// it never escapes to the vendor as a stack trace.
    /// </summary>
    public sealed class DataContractException : Exception
    {
        public DataContractException(int statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}
