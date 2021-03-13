using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Radzen;
using Serilog;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCookie.Components
{
    public partial class LoginComponent
    {
        [Inject] NavigationManager Navigator { get; set; }
        [Inject] HttpClient HttpClient { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }

        protected override void OnInitialized()
        {
            HttpClient.BaseAddress = new Uri(Navigator.BaseUri);
        }

        private async Task OnLogin(LoginArgs args)
        {
            try
            {
                var response = await HttpClient.PostAsync($"/api/auth/signIn",
                    new StringContent(JsonConvert.SerializeObject(args), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    throw new Exception(responseBody);
                }

                response.Headers.TryGetValues("Set-Cookie", out var setCookie);
                var setCookieString = setCookie.OrderByDescending(x => x.Length).FirstOrDefault();
                var firstCookie = setCookieString.Split(';')[0];
                await JSRuntime.InvokeAsync<string>("addCookie", firstCookie);

                Navigator.NavigateTo("/", true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}
