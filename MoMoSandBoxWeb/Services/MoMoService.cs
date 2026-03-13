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

        public async Task<(bool Success, string Response)> RequestToPayAsync(
            string amount,
            string currency,
            string externalId,
            string payerPhone,
            string referenceId,
            string payerMessage,
            string payeeNote,
            string bearerToken,
            string subscriptionKey){
            //partyId  is payerPhone
            //var referenceId = Guid.NewGuid().ToString();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://sandbox.momodeveloper.mtn.com//collection/v1_0/requesttopay");
            request.Headers.Add("X-Reference-Id", referenceId);
            request.Headers.Add("X-Target-Environment", "sandbox");
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.Headers.Add("Authorization", "Bearer " + bearerToken);

            var content = new StringContent("{\r\n  \"amount\": \"" + amount + "\",\r\n  \"currency\": \"" + currency + "\",\r\n  \"externalId\": \"" + externalId + "\",\r\n  \"payer\": {\r\n    \"partyIdType\": \"MSISDN\",\r\n    \"partyId\": \"" + payerPhone + "\"\r\n  },\r\n  \"payerMessage\": \"" + payerMessage + "\",\r\n  \"payeeNote\": \"" + payeeNote + "\"\r\n}", null, "application/json");
            request.Content = content;

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
