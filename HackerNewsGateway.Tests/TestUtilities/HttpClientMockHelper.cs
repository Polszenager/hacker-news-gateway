using System.Net;
using System.Net.Http;
using System.Text;

namespace HackerNewsGateway.Tests.TestUtilities
{
    /// <summary>
    /// Mock HTTP message handler for unit testing HttpClient dependencies.
    /// Allows setting up responses for specific URLs using a fluent API.
    /// </summary>
    /// <example>
    /// <code>
    /// var mockHttp = new MockHttpMessageHandler();
    /// mockHttp
    ///     .When("https://api.example.com/data")
    ///     .Respond("application/json", "{'key': 'value'}");
    /// var client = new HttpClient(mockHttp);
    /// </code>
    /// </example>
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Dictionary<string, HttpResponseMessage> _responses = new();

        /// <summary>
        /// Sets up a response for a specific URL. Returns a fluent builder to specify the response.
        /// </summary>
        /// <param name="url">The exact URL to match (string equality comparison)</param>
        /// <returns>A MockHttp builder to configure the response</returns>
        public MockHttp When(string url)
        {
            return new MockHttp(this, url);
        }

        internal void AddResponse(string url, string contentType, string content)
        {
            _responses[url] = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content, Encoding.UTF8, contentType)
            };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestUrl = request.RequestUri!.ToString();
            
            if (_responses.TryGetValue(requestUrl, out var response))
            {
                return Task.FromResult(response);
            }

            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Not found", Encoding.UTF8, "text/plain")
            });
        }
    }

    /// <summary>
    /// Fluent builder for configuring HTTP responses in tests.
    /// </summary>
    public class MockHttp
    {
        private readonly MockHttpMessageHandler _handler;
        private readonly string _url;

        internal MockHttp(MockHttpMessageHandler handler, string url)
        {
            _handler = handler;
            _url = url;
        }

        /// <summary>
        /// Configures the response content and content type for the URL specified in When().
        /// </summary>
        /// <param name="contentType">The HTTP content type (e.g., "application/json")</param>
        /// <param name="content">The response body content as a string</param>
        /// <returns>The MockHttpMessageHandler instance for method chaining</returns>
        public MockHttpMessageHandler Respond(string contentType, string content)
        {
            _handler.AddResponse(_url, contentType, content);
            return _handler;
        }
    }
}

