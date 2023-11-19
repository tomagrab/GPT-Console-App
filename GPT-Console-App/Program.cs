using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GPTConsoleAPP
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Chat with OpenAI's GPT model. Type 'exit' to end.");
            string userPrompt;

            while (true)
            {
                Console.Write("You: ");
                userPrompt = Console.ReadLine();

                if (userPrompt.ToLower() == "exit")
                    break;

                string response = await ChatWithOpenAI(userPrompt);
                Console.WriteLine("OpenAI: " + response); // You may want to parse this to make it more readable
            }
        }

        static async Task<string> ChatWithOpenAI(string prompt)
        {
            string apiKey = "YOUR_API_KEY"; // Replace with your API key
            string apiEndpoint = "https://api.openai.com/v1/chat/completions";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new { role = "system", content = "You are a helpful assistant." },
            new { role = "user", content = prompt }
        }
            };

            string json = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(apiEndpoint, content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON response
            ChatResponse chatResponse = JsonConvert.DeserializeObject<ChatResponse>(responseBody);

            // Extract the content from the first choice message
            string responseContent = chatResponse.Choices[0].Message.Content;

            return responseContent;
        }
    }

    public class ChatResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }
        public Choice[] Choices { get; set; }
        public Usage Usage { get; set; }
    }

    public class Choice
    {
        public int Index { get; set; }
        public Message Message { get; set; }
        public string FinishReason { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class Usage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }
}