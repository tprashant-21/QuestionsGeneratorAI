using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using RedikerAI.Models; // Adjust this based on your project's actual namespace
using Dapper; // Assuming you have a UserRepository class
//using RedikerAI.Services; // Assuming you have an OpenAIService class
//using RedikerAI.Repositories;

public class HomeController : Controller
{
	private readonly UserRepository _userRepo;
	private readonly OpenAIService _openAIService;

	public HomeController()
	{
		_userRepo = new UserRepository(); // Initialize the repository
		_openAIService = new OpenAIService(); // Initialize the OpenAI service
	}

	// GET: Home (Dashboard after login)
	public ActionResult Index()
	{
		if (Session["UserId"] == null)
		{
			return RedirectToAction("Login", "Account"); // Redirect to login if not logged in
		}

		return View();
	}

	// Optional: Dashboard to display the user info (you could expand it to show the user's previous questions)
	public async Task<ActionResult> Dashboard()
	{
		if (Session["UserId"] == null)
		{
			return RedirectToAction("Login", "Account"); // Ensure user is logged in
		}

		var userId = (int)Session["UserId"];
		var user = await _userRepo.GetUserById(userId);

		return View(user);
	}

	// POST: Home/GenerateQuestions
	[HttpPost]
	public async Task<ActionResult> GenerateQuestions(string topic, int grade, int numberOfQuestions)
	{
		if (Session["UserId"] == null)
		{
			return RedirectToAction("Login", "Account"); // Ensure user is logged in
		}

		try
		{
			// Call the OpenAI service to generate questions
			var questions = await _openAIService.GenerateQuestionsAsync(topic, grade, numberOfQuestions);

			// You can also save the generated questions to the database if needed
			var userId = (int)Session["UserId"];
			await SaveGeneratedQuestionsToDatabase(userId, topic, grade, numberOfQuestions, questions);

			return Json(new { success = true, questions = questions });
		}
		catch (Exception ex)
		{
			// Handle error and log it if needed
			return Json(new { success = false, message = "Error generating questions: " + ex.Message });
		}
	}

	// Optional: Save generated questions to the database (for historical tracking)
	private async Task SaveGeneratedQuestionsToDatabase(int userId, string topic, int grade, int numberOfQuestions, string questions)
	{
		try
		{
			using (var dbConnection = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
			{
				var query = @"
                    INSERT INTO Questions (UserId, Topic, Grade, NumberOfQuestions, GeneratedQuestions, Timestamp)
                    VALUES (@UserId, @Topic, @Grade, @NumberOfQuestions, @GeneratedQuestions, GETDATE())";

				var parameters = new
				{
					UserId = userId,
					Topic = topic,
					Grade = grade,
					NumberOfQuestions = numberOfQuestions,
					GeneratedQuestions = questions
				};

				await dbConnection.ExecuteAsync(query, parameters);
			}
		}
		catch (Exception ex)
		{
			// Handle exception (log or rethrow if needed)
			throw new Exception("Error saving generated questions to the database.", ex);
		}
	}

	
}
