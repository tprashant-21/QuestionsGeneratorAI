using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class OpenAIService
{
	private readonly string _apiKey;

	// Constructor to set your OpenAI API Key
	public OpenAIService()
	{
		_apiKey = ConfigurationManager.AppSettings["OpenAI_ApiKey"]; // Read from config
	}

	// Method to generate questions using OpenAI
	public async Task<string> GenerateQuestionsAsync(string topic, int grade, int numberOfQuestions)
	{
		using (var client = new HttpClient())
		{
			// Set the authorization header with your API key
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

			// Prepare the request body
			var requestBody = new
			{
				model = "gpt-3.5-turbo", // You can choose a different model, e.g., "gpt-4"
				messages = new[]
				{
					new { role = "system", content = "You are a helpful assistant who generates quiz questions." },
					new { role = "user", content = $"Generate {numberOfQuestions} questions on {topic} for grade {grade}." }
				},
				max_tokens = 500
			};

			var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
			var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

			// Make the POST request to the OpenAI API
			var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

			// Read and parse the response
			if (response.IsSuccessStatusCode)
			{
				var responseString = await response.Content.ReadAsStringAsync();
				var openAiResponse = JsonConvert.DeserializeObject<OpenAIResponse>(responseString);

				// Extract the questions from the response
				var questions = openAiResponse.Choices[0].Message.Content;

				return questions;
			}
			else
			{
				throw new Exception($"OpenAI API request failed: {response.ReasonPhrase}");
			}
		}
	}
}

// Create a class to parse OpenAI API response
public class OpenAIResponse
{
	public Choice[] Choices { get; set; }

	public class Choice
	{
		public Message Message { get; set; }
	}

	public class Message
	{
		public string Role { get; set; }
		public string Content { get; set; }
	}
}
