using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MoMoSandBoxWeb.Model;
using MoMoSandBoxWeb.Services;

namespace MoMoSandBoxWeb.Components.Pages.payment
{
    public class PaymentBase : ComponentBase
    {
        public string phoneNumber = "56733123453";
        public string amount = "600";
        public string statusMessage = "";
        public string responseDetails = "";
        public bool isLoading = false;
        public bool success = false;

        [Inject] public IJSRuntime? JSRuntime { get; set; }
        [Inject] public MoMoService MoMoService { get; set; }
        [Inject] public IOptions<MoMoSettings> MoMoSettings { get; set; }

        public async Task ProcessPayment()
        {
            var subscriptionKey = MoMoSettings.Value.SubscriptionKey;
            var basicAuthToken = MoMoSettings.Value.BasicAuthToken;

            isLoading = true;
            statusMessage = "";
            responseDetails = "";

            try
            {
                // Step 1: Generate token
                var token = await MoMoService.GetBearerTokenAsync(
                    basicAuthToken: basicAuthToken,
                    subscriptionKey: subscriptionKey
                );

                if (token == null)
                {
                    statusMessage = "❌ Failed to generate token!";
                    success = false;
                    return;
                }

                // Step 2: Request to pay
                var referenceId = Guid.NewGuid().ToString();
                var (ok, response) = await MoMoService.RequestToPayAsync(
                    amount: amount,
                    currency: "EUR",
                    externalId: "00004335",
                    payerPhone: phoneNumber,
                    referenceId: referenceId,
                    payerMessage: "MoMo Market Payment",
                    payeeNote: "MoMo Market Payment",
                    bearerToken: token,
                    subscriptionKey: subscriptionKey
                );

                success = ok;
                responseDetails = response;

                if (ok)
                {
                    // Step 3: Check payment status
                    await ProcessCheckStatus(referenceId, token, subscriptionKey);
                }
                else
                {
                    statusMessage = $"❌ Payment failed! {response}";
                }
            }
            catch (Exception ex)
            {
                success = false;
                statusMessage = $"❌ Error: {ex.Message}";
                responseDetails = ex.ToString();
            }
            finally
            {
                isLoading = false;
            }
        }

        public async Task ProcessCheckStatus(string referenceId, string bearerToken, string subscriptionKey)
        {
            isLoading = true;
            statusMessage = "";
            responseDetails = "";

            try
            {
                var (ok, responseBody) = await MoMoService.GetRequestToPayStatusAsync(
                    referenceId: referenceId,
                    bearerToken: bearerToken,
                    subscriptionKey: subscriptionKey
                );

                success = ok;
                responseDetails = responseBody;
                statusMessage = ok ? "✅ Status: " + responseBody : "❌ Error: " + responseBody;
            }
            catch (Exception ex)
            {
                success = false;
                statusMessage = $"❌ Error: {ex.Message}";
                responseDetails = ex.ToString();
            }
            finally
            {
                isLoading = false;
            }
        }

        protected override async Task OnInitializedAsync() { }
    }
}