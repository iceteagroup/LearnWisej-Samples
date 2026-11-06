using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AccessOps.Services
{
    /// <summary>
    /// A downstream API called with the signed-in user's access token (the provider's "HR
    /// directory" in the lab). The token comes from <see cref="IExternalSignIn.GetAccessTokenAsync"/>,
    /// which refreshes it in the back channel; it is never handed to the browser.
    /// </summary>
    public sealed class HrProfileClient
    {
        private readonly IExternalSignIn _external;
        private readonly IHttpClientFactory _http;
        private readonly string _url;
        private readonly ILogger<HrProfileClient> _log;

        public HrProfileClient(IExternalSignIn external, IHttpClientFactory http, IConfiguration configuration, ILogger<HrProfileClient> log)
        {
            _external = external;
            _http = http;
            _url = configuration["Oidc:HrProfileUrl"];
            _log = log;
        }

        /// <summary>
        /// The job title the HR directory holds (null when there is no answer), and whether the
        /// session's access token has expired for good - the caller then signs the session out.
        /// </summary>
        public async Task<(string JobTitle, bool TokenExpired)> GetJobTitleAsync()
        {
            if (_external.Current == null || string.IsNullOrEmpty(_url))
                return (null, false);

            var token = await _external.GetAccessTokenAsync();
            if (token == null)
                return (null, true);

            using var request = new HttpRequestMessage(HttpMethod.Get, _url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await _http.CreateClient().SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _log.LogWarning("HR directory answered {Status}", (int)response.StatusCode);
                return (null, false);
            }

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return (json.RootElement.TryGetProperty("jobTitle", out var title) ? title.GetString() : null, false);
        }
    }
}
