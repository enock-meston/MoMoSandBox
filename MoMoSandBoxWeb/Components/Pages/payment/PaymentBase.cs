using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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

        [Inject]
        public IJSRuntime? JSRuntime { get; set; }
        [Inject]
        public MoMoService MoMoService { get; set; }

        // generate token
        public async Task ProcessPayment()
        {
            //if (JSRuntime != null)
            //{
            //    await JSRuntime.InvokeVoidAsync("alert", "Payment function called!");
            //}

            isLoading = true;
            statusMessage = "";
            responseDetails = "";

            try
            {
                var referenceId = Guid.NewGuid().ToString();
                var (ok, response) = await MoMoService.RequestToPayAsync(
                    amount: amount,
                    currency: "EUR",
                    externalId: "00004335",
                    payerPhone: phoneNumber,
                    referenceId: referenceId,
                    payerMessage: "MoMo Market Payment",
                    payeeNote: "MoMo Market Payment",
                    bearerToken: "eyJ0eXAiOiJKV1QiLCJhbGciOiJSMjU2In0.eyJjbGllbnRJZCI6IjcwZGMxZDBkLWJjNWItNGZjMy04MTBiLTlmYzk4YmI5MDg2NiIsImV4cGlyZXMiOiIyMDI2LTAzLTEzVDE0OjU3OjEzLjYyMjI2MDk5OCIsInNlc3Npb25JZCI6IjY5OGU4NDZmLWFkMDUtNGVjMy04YTdlLWM3MjMxNTg0OGMxYSJ9.IQBQrTQs9Z5TVBjusnjuJGm0-5m4C1Zd1w_oLEOfk7qMwAF2sEbWR4cSX1S3nE_J6elND2F3p2P5g0DN1fdepoLX6A-tkO7qIwx-wn6s5JAxnRv2j0SNBr2F9YO3F35XFtiiUKXa4HzPf7j4XCO5rVr0oLvNXJVqZPH7TJh9e3Vvdv_dpilM4cXjiv5brQE5xke0O8CzJYWBIof1-mkH6JmuMe4BfR-1Etq4qma-XjKtl9B7jUgGDAw1VB6xG0uLdzcR6vbN-C7YuKXxHIhAFOhPDFehJhm15yPimW1jlaCBWMiJp2kZ6Ws2wYVQ8IqUEyusUuRCAmkBGHNRj7ri6Q",
                    subscriptionKey: "25407f5a9eae40528115c71fe5205961"
                );

                success = ok;
                responseDetails = response;

                if (ok)
                {
                    //statusMessage = $"Payment request sent successfully! (202 Accepted)";
                    //call  status Request To Pay method
                    await ProcessCheckStatus(referenceId);
                }
                else
                {
                    statusMessage = $"Payment failed!{response.Count()}";
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

        public async Task ProcessCheckStatus(string referenceId)
        {
            isLoading = true;
            statusMessage = "";
            responseDetails = "";
            try
            {
                var (ok, responseBody) = await MoMoService.GetRequestToPayStatusAsync(
                    referenceId: "158c8952-23e9-490b-b12d-75953200bf12",
                    bearerToken: "eyJ0eXAiOiJKV1QiLCJhbGciOiJSMjU2In0.eyJjbGllbnRJZCI6IjcwZGMxZDBkLWJjNWItNGZjMy04MTBiLTlmYzk4YmI5MDg2NiIsImV4cGlyZXMiOiIyMDI2LTAzLTEzVDE0OjU3OjEzLjYyMjI2MDk5OCIsInNlc3Npb25JZCI6IjY5OGU4NDZmLWFkMDUtNGVjMy04YTdlLWM3MjMxNTg0OGMxYSJ9.IQBQrTQs9Z5TVBjusnjuJGm0-5m4C1Zd1w_oLEOfk7qMwAF2sEbWR4cSX1S3nE_J6elND2F3p2P5g0DN1fdepoLX6A-tkO7qIwx-wn6s5JAxnRv2j0SNBr2F9YO3F35XFtiiUKXa4HzPf7j4XCO5rVr0oLvNXJVqZPH7TJh9e3Vvdv_dpilM4cXjiv5brQE5xke0O8CzJYWBIof1-mkH6JmuMe4BfR-1Etq4qma-XjKtl9B7jUgGDAw1VB6xG0uLdzcR6vbN-C7YuKXxHIhAFOhPDFehJhm15yPimW1jlaCBWMiJp2kZ6Ws2wYVQ8IqUEyusUuRCAmkBGHNRj7ri6Q",
                    subscriptionKey: "25407f5a9eae40528115c71fe5205961"
                );

                success = ok;
                responseDetails = responseBody;

                if (ok)
                {
                    statusMessage = "Status: " + responseBody;
                }
                else
                {
                    statusMessage = "Error: " + responseBody;
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
        protected override async Task OnInitializedAsync()
        {

        }
    }
}
