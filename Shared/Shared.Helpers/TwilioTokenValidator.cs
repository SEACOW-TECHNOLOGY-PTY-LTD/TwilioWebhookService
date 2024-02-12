using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared.Helpers;

public class TwilioTokenValidator
{
    private readonly HttpClient _httpClient;

    public TwilioTokenValidator()
    {
        _httpClient = new HttpClient();
    }

    public async Task<JObject> ValidateTokenAsync(string token, string accountSid, string authToken, string? realm = null)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Unauthorized: Token was not provided.");

        if (string.IsNullOrWhiteSpace(accountSid) || string.IsNullOrWhiteSpace(authToken))
            throw new ArgumentException("Unauthorized: AccountSid or AuthToken was not provided.");

        var baseAddress = realm != null ? $"https://iam.{realm}.twilio.com" : "https://iam.twilio.com";
        var requestUri = $"/v1/Accounts/{accountSid}/Tokens/validate";

        _httpClient.DefaultRequestHeaders.Clear();
        
        var authenticationString = $"{accountSid}:{authToken}";
        var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

        // Construct the JSON payload
        var tokenData = new
        {
            token = token
        };
        var jsonContent = JsonConvert.SerializeObject(tokenData);
        var requestData = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeaderValue);

        var response = await _httpClient.PostAsync(new Uri(new Uri(baseAddress), requestUri), requestData);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error validating the token. Status Code: {response.StatusCode}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var parsedResponse = JObject.Parse(responseBody);

        if (parsedResponse["valid"]?.Value<bool>() != true)
        {
            throw new Exception(parsedResponse["message"]?.Value<string>() ?? "Unknown error from Twilio.");
        }

        return parsedResponse;
    }

    // Here is the function similar to 'functionValidator' in your JS code, taking a handler function to call after validation
    public async Task FunctionValidator(Func<JObject, Task> handlerFn, string token, string accountSid, string authToken, string? realm = null)
    {
        try
        {
            var validationResponse = await ValidateTokenAsync(token, accountSid, authToken, realm);
            await handlerFn(validationResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            // Handle error as necessary, e.g., return an error response or throw the exception further.
        }
    }
}