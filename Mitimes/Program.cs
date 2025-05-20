using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Mitimes
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string secretUrl = "https://au.mitimes.com/careers/apply/secret";
        private static readonly string applyUrl = "https://au.mitimes.com/careers/apply";

        static async Task Main(string[] args)
        {
            try
            {
                string authToken = await GetAuthorizationToken();
                Console.WriteLine($"Retrieved authorization token: {authToken}");

                var applicationData = new
                {
                    name = "Jake Gosling",
                    email = "jakemgosling@gmail.com",
                    job_title = "Senior Software Developer",
                    final_attempt = true,
                    extra_information = new
                    {
                        skills = new[] { "C#", ".NET", "SQL Server" },
                        years_experience = 10,
                        certifications = "2x MCSA, 1x MCSE",
                        education = "Masters IT, B. Commerce",
                        reason_to_hire = "I can develop quality front end, back end and data solutions.... and sell good quality beef!"
                    }
                };

                var response = await SubmitApplication(authToken, applicationData);

                Console.WriteLine($"Status Code: {response.StatusCode}");
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Body: {responseBody}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task<string> GetAuthorizationToken()
        {
            var response = await client.GetAsync(secretUrl);
            response.EnsureSuccessStatusCode();
            var rawRoken = await response.Content.ReadAsStringAsync();

            var authToken = JsonSerializer.Deserialize<string>(rawRoken);
            Console.WriteLine($"Parsed authorization token: {rawRoken}");

            return authToken;
        }

        private static async Task<HttpResponseMessage> SubmitApplication(string authToken, object applicationData)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, applyUrl);

            // Add authorization header
            request.Headers.Authorization = new AuthenticationHeaderValue(authToken);

            // Add JSON body
            var jsonContent = JsonSerializer.Serialize(applicationData);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            return await client.SendAsync(request);
        }
    }
}