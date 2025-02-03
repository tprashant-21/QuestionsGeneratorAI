using Dapper;
using RedikerAI.Models;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

public class UserRepository
{
	private readonly string _connectionString;

	public UserRepository()
	{
		_connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
	}

	// Method to add a user to the database
	public int AddUser(User user)
	{
		using (IDbConnection dbConnection = new SqlConnection(_connectionString))
		{
			string query = @"INSERT INTO Users (Email, FirstName, LastName, SchoolName, State, Country, PasswordHash) 
                             VALUES (@Email, @FirstName, @LastName, @SchoolName, @State, @Country, @PasswordHash); 
                             SELECT CAST(SCOPE_IDENTITY() as int);";

			var result = dbConnection.Query<int>(query, user).FirstOrDefault();
			return result;
		}
	}

	// Method to get a user by email
	public User GetUserByEmail(string email)
	{
		using (IDbConnection dbConnection = new SqlConnection(_connectionString))
		{
			string query = "SELECT * FROM Users WHERE Email = @Email";
			return dbConnection.Query<User>(query, new { Email = email }).FirstOrDefault();
		}
	}

	// Get user by ID
	public async Task<User> GetUserById(int userId)
	{
		using (var connection = new SqlConnection(_connectionString))
		{
			await connection.OpenAsync();

			// Use Dapper to query the Users table for a user by ID
			var query = "SELECT * FROM Users WHERE Id = @UserId";
			var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { UserId = userId });

			return user;
		}
	}
}
