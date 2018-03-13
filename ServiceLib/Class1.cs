using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServiceLib
{
    public class ServiceClass
    {
        private static HttpClient Client { get; } = new HttpClient();

        public async Task RunAvailabilityTestsAsync()
        {
            List<Exception> exs = new List<Exception>(2);

            System.Diagnostics.Trace.TraceInformation("Beginning is google available method.");
            try
            {
                await TryGetResponseWithRetryAsync("https://google.com").ConfigureAwait(false);
                System.Diagnostics.Trace.TraceInformation("Google was successful.");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Google failed with message: {ex.Message}");
                exs.Add(ex);
            }

            try
            {
                await TryGetResponseWithRetryAsync("https://thisisagarbageendpoint.com").ConfigureAwait(false);
                System.Diagnostics.Trace.TraceInformation("Garbage endpoint was successful somehow.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Garbage endpoint failed with message: {ex.Message}");
                exs.Add(ex);
            }

            if (exs.Any())
            {
                throw new AggregateException(exs);
            }
        }

        private static async Task<HttpResponseMessage> TryGetResponseWithRetryAsync(string url)
        {
            int attempt = 0;
            while (attempt++ < 4)
            {
                var response = await Client.GetAsync(url).ConfigureAwait(false);

                System.Diagnostics.Trace.TraceInformation($"Send request to {url} and received a response.");

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Trace.TraceWarning($"{url} response was not successful.");
                }

                return response;
            }

            System.Diagnostics.Trace.TraceError($"{url} failed after retries.");
            throw new Exception($"Unable to get successful response after {attempt} attempts.");
        }
    }
}
