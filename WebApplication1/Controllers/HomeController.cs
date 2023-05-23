using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static HttpClient client = new HttpClient() { BaseAddress = new Uri("https://6464b03b127ad0b8f8a53cf7.mockapi.io/users/") };
        public static BagOfData BagOfData = new BagOfData();
        
        /// <summary>
        /// Build in Visual Studio method that tracks/handles logs
        /// </summary>
        /// <param name="logger"></param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets a list of all the users from the mockAPI link
        /// </summary>
        /// <returns> Task, needs to be awaited </returns>
        public static async Task GetAPIData()
        {
            if (!BagOfData.ContainsData("DataFromAPI")) { 
                
                var RawJSONDataFromAPI = await client.GetStringAsync("");

                Console.WriteLine("[INFO] Raw string from API below:");
                Console.WriteLine(RawJSONDataFromAPI);

                // Trimming off the [ and ] symbol from the beginning and end of string, since it causes serialization problems
                RawJSONDataFromAPI.Remove(RawJSONDataFromAPI.Length-1, 1);
                RawJSONDataFromAPI.Remove(0, 1);

                Console.WriteLine(RawJSONDataFromAPI);
            
                BagOfData.SetData("DataFromAPI", (List<User>)System.Text.Json.JsonSerializer.Deserialize<List<User>>(RawJSONDataFromAPI, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }));
            }
        }

        /// <summary>
        /// Adds a new expense via POST method to the list of expenses for a particular user
        /// </summary>
        /// <param name="user">User to be edited</param>
        /// <param name="newExpense">Expense details</param>
        /// <returns> Task, needs to be awaited </returns>
        public static async Task AddNewExpense(string user_api_id, int amount, string description)
        {
            int user_api_id_AsNum = int.Parse(user_api_id);
            user_api_id_AsNum--;
            User current_user = BagOfData.GetData("DataFromAPI")[user_api_id_AsNum];
            current_user.expenses.Add(new Expense(description, amount));
            var user_json_data = System.Text.Json.JsonSerializer.Serialize<User>(current_user, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            HttpContent request_content = new StringContent(user_json_data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"https://6464b03b127ad0b8f8a53cf7.mockapi.io/users/{user_api_id}", request_content);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string user_api_id, int amount, string description)
        {
            // Perform any necessary validation and model binding
            if (!ModelState.IsValid)
            {
                // Handle validation errors and return the view with appropriate error messages
                return View();
            }

            // Call the asynchronous method
            await AddNewExpense(user_api_id, amount, description);

            // Redirect to a success page or perform other actions
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns> The Index view </returns>
        public async Task<IActionResult> Index()
        {
            GetAPIData().GetAwaiter().GetResult();
            
            return View(BagOfData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View(BagOfData);
        }

        /// <summary>
        /// Built in Visual Studio method that handles errors
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}