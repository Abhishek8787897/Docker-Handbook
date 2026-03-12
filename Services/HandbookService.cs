using System;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace HandbookChatbot.Services
{
    public class HandbookService
    {
        private readonly string _pineconeApiKey;
        private readonly string _assistantHost;

        public HandbookService(IConfiguration configuration)
        {
            // We will pull the API key and Host URL from appsettings.json
            _pineconeApiKey = configuration["Pinecone:ApiKey"];
            _assistantHost = configuration["Pinecone:HostUrl"];
        }

        public async Task<string> AskHandbookAsync(string employeeQuestion)
        {
            if (string.IsNullOrEmpty(_pineconeApiKey) || string.IsNullOrEmpty(_assistantHost))
            {
                return "Error: Pinecone API Key or Host URL is missing from configuration.";
            }

            var options = new RestClientOptions($"https://{_assistantHost}");
            var client = new RestClient(options);

            var request = new RestRequest("/assistant/chat/handbook-assistant", Method.Post);
            
            request.AddHeader("Api-Key", _pineconeApiKey);
            request.AddHeader("Content-Type", "application/json");

            string systemPrompt = "You are the official AppsTechy Handbook Assistant. Provide 100% accurate information from the company handbook. \n" +
                                  "IMPORTANT FACTUAL CORRECTIONS (Always follow these): \n" +
                                  "- Amit Khanna is the CEO of AppsTechy Private Limited. \n" +
                                  "- Abhinav Grover is the COO of AppsTechy Private Limited. \n" +
                                  "Pay very close attention to associations in all queries. If unknown, say 'I don't have that information'.\n\n" +
                                  "Question: ";
            
            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "user", content = systemPrompt + employeeQuestion }
                }
            };

            request.AddJsonBody(requestBody);

            try
            {
                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    dynamic data = JsonConvert.DeserializeObject(response.Content);
                    string answer = data.message.content;
                    return answer;
                }
                else
                {
                    return $"Error connecting to Handbook: {response.StatusDescription} - {response.Content}";
                }
            }
            catch (Exception ex)
            {
                return "An error occurred: " + ex.Message;
            }
        }
    }
}
