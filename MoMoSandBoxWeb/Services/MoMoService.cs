using System.Text;
using System.Text.Json;

namespace MoMoSandBoxWeb.Services
{
    public class MoMoService
    {
        private readonly HttpClient _httpClient;

        public MoMoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //Generate access token method here

        public async Task<(bool Success, string Response)> GenerateTokenAsync(
            string basicAuthToken,
            string subscriptionKey)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://sandbox.momodeveloper.mtn.com/collection/token/");

            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.Headers.Add("Authorization", "Basic " + basicAuthToken);

            request.Content = new StringContent("", null, "text/plain");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return (true, await response.Content.ReadAsStringAsync());
            }
            else
            {
                return (false, await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<string?> GetBearerTokenAsync(
            string basicAuthToken,
            string subscriptionKey)
        {
            var (success, response) = await GenerateTokenAsync(basicAuthToken, subscriptionKey);

            if (!success) return null;

            var json = JsonSerializer.Deserialize<JsonElement>(response);
            return json.GetProperty("access_token").GetString();
        }

        public async Task<(bool Success, string Response)> RequestToPayAsync(
    string amount,
    string currency,
    string externalId,
    string payerPhone,
    string referenceId,
    string payerMessage,
    string payeeNote,
    string bearerToken,
    string subscriptionKey)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://sandbox.momodeveloper.mtn.com/collection/v1_0/requesttopay");

            request.Headers.Add("X-Reference-Id", referenceId);
            request.Headers.Add("X-Target-Environment", "sandbox");
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.Headers.Add("Authorization", "Bearer " + bearerToken);

            //  cleaner than string concatenation
            var body = new
            {
                amount,
                currency,
                externalId,
                payer = new
                {
                    partyIdType = "MSISDN",
                    partyId = payerPhone
                },
                payerMessage,
                payeeNote
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return (true, await response.Content.ReadAsStringAsync());
            else
                return (false, await response.Content.ReadAsStringAsync());
        }

        //check Status request to pay method here
        public async Task<(bool Success, string Response)> GetRequestToPayStatusAsync(
            string referenceId,
            string bearerToken,
            string subscriptionKey)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://sandbox.momodeveloper.mtn.com/collection/v1_0/requesttopay/{referenceId}");

            request.Headers.Add("X-Target-Environment", "sandbox");
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.Headers.Add("Authorization", "Bearer " + bearerToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return (true, await response.Content.ReadAsStringAsync());
            }
            else
            {
                return (false, await response.Content.ReadAsStringAsync());
            }
        }
    }
}
