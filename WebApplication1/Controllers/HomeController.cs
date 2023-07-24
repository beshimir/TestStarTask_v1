using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
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
            
                
                var RawJSONDataFromAPI = await client.GetStringAsync("");

                Console.WriteLine("[INFO] Raw string from API below:");
                Console.WriteLine(RawJSONDataFromAPI);

                // Trimming off the [ and ] symbol from the beginning and end of string, since it causes serialization problems
                RawJSONDataFromAPI.Remove(RawJSONDataFromAPI.Length-1, 1);
                RawJSONDataFromAPI.Remove(0, 1);

                Console.WriteLine(RawJSONDataFromAPI);
            if (!BagOfData.ContainsData("DataFromAPI"))
            {
                BagOfData.SetData("DataFromAPI", (List<User>)System.Text.Json.JsonSerializer.Deserialize<List<User>>(RawJSONDataFromAPI, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }));
            }
            else 
            {
                BagOfData.ReplaceData("DataFromAPI", (List<User>)System.Text.Json.JsonSerializer.Deserialize<List<User>>(RawJSONDataFromAPI, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }));
            }
        }

        /// <summary>
        /// Adds a new expense via PUT method to the list of expenses for a particular user
        /// </summary>
        /// <param name="user">User to be edited</param>
        /// <param name="newExpense">Expense details</param>
        /// <returns> Task, needs to be awaited </returns>
        public static async Task AddNewExpense(string user_api_id, int amount, string description, string name)
        {
            int user_api_id_AsNum = int.Parse(user_api_id);
            user_api_id_AsNum--;
            User current_user = BagOfData.GetData("DataFromAPI")[user_api_id_AsNum];
            current_user.expenses.Add(new Expense(name, DateTime.Now.ToString(), description, amount));
            var user_json_data = System.Text.Json.JsonSerializer.Serialize<User>(current_user, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            HttpContent request_content = new StringContent(user_json_data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"https://6464b03b127ad0b8f8a53cf7.mockapi.io/users/{user_api_id}", request_content);
  
        }

        /// <summary>
        /// Adds a new expense via PUT method to the list of payments for a particular expense
        /// </summary>
        /// <param name="user">User to be edited</param>
        /// <param name="newPayment">Payment details</param>
        /// <returns> Task, needs to be awaited </returns>
        public static async Task AddNewPayment(string user_api_id, string expense_date, string name, int amountDue, int amountPaid, string type_of_payment)
        {
            int user_api_id_AsNum = int.Parse(user_api_id);
            user_api_id_AsNum--;
            User current_user = BagOfData.GetData("DataFromAPI")[user_api_id_AsNum];
            var current_expense = current_user.expenses.Where(x => x.date == expense_date).First();

            if (type_of_payment == "Outbound")
                current_expense.duePaymentsOutbound.Add(new Payment(name, amountDue, amountPaid));
            else
                current_expense.duePaymentsInbound.Add(new Payment(name, amountDue, amountPaid));

            var user_json_data = System.Text.Json.JsonSerializer.Serialize<User>(current_user, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            HttpContent request_content = new StringContent(user_json_data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"https://6464b03b127ad0b8f8a53cf7.mockapi.io/users/{user_api_id}", request_content);

        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense(string user_api_id, int amount, string description, string name)
        {
            // Perform any necessary validation and model binding
            if (!ModelState.IsValid)
            {
                // Handle validation errors and return the view with appropriate error messages
                //return View();
            }

            // Call the asynchronous method
            await AddNewExpense(user_api_id, amount, description, name);

            // Redirect to a success page or perform other actions
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(string user_api_id, string expense_date, string type_of_payment, string name, int amountDue, int amountPaid)
        {
            // Perform any necessary validation and model binding
            if (!ModelState.IsValid)
            {
                // Handle validation errors and return the view with appropriate error messages
                //return View();
            }

            // Call the asynchronous method
            await AddNewPayment(user_api_id, expense_date, name, amountDue, amountPaid, type_of_payment);

            // Redirect to a success page or perform other actions
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOutboundPayment(string user_api_id, string expense_date, int updated_amountPaid, string payment)
        {
            // Perform any necessary validation and model binding
            if (!ModelState.IsValid)
            {
                // Handle validation errors and return the view with appropriate error messages
                //return View();
            }

            // Call the asynchronous method
            await UpdateExistingOutboundPayment(user_api_id, expense_date, updated_amountPaid, payment);

            // Redirect to a success page or perform other actions
            return RedirectToAction("Index");
        }

        public static async Task UpdateExistingOutboundPayment(string user_api_id, string expense_date, int updated_amountPaid, string payment)
        {
            int user_api_id_AsNum = int.Parse(user_api_id);
            user_api_id_AsNum--;
            User current_user = BagOfData.GetData("DataFromAPI")[user_api_id_AsNum];
            
            current_user.expenses.Where(x => x.date == expense_date).First().duePaymentsOutbound.Where(x => x.id == payment).First().amountPaid = updated_amountPaid;

            var user_json_data = System.Text.Json.JsonSerializer.Serialize<User>(current_user, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            HttpContent request_content = new StringContent(user_json_data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"https://6464b03b127ad0b8f8a53cf7.mockapi.io/users/{user_api_id}", request_content);

        }


        [HttpPost]
        public async Task<IActionResult> UpdateInboundPayment(string user_api_id, string expense_date, int updated_amountPaid, string payment)
        {
            // Perform any necessary validation and model binding
            if (!ModelState.IsValid)
            {
                // Handle validation errors and return the view with appropriate error messages
                //return View();
            }

            // Call the asynchronous method
            await UpdateExistingInboundPayment(user_api_id, expense_date, updated_amountPaid, payment);

            // Redirect to a success page or perform other actions
            return RedirectToAction("Index");
        }

        public static async Task UpdateExistingInboundPayment(string user_api_id, string expense_date, int updated_amountPaid, string payment)
        {
            int user_api_id_AsNum = int.Parse(user_api_id);
            user_api_id_AsNum--;
            User current_user = BagOfData.GetData("DataFromAPI")[user_api_id_AsNum];

            current_user.expenses.Where(x => x.date == expense_date).First().duePaymentsInbound.Where(x => x.id == payment).First().amountPaid = updated_amountPaid;

            var user_json_data = System.Text.Json.JsonSerializer.Serialize<User>(current_user, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            HttpContent request_content = new StringContent(user_json_data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"https://6464b03b127ad0b8f8a53cf7.mockapi.io/users/{user_api_id}", request_content);

        }



        [HttpPost]
        public async Task<IActionResult> Login(string email)
        {
            GetAPIData().GetAwaiter().GetResult();

            foreach (User user in BagOfData.GetData("DataFromAPI"))
            {
                if(user.email == email)
                {
                    if (BagOfData.ContainsData("current_user"))
                    {
                        BagOfData.ReplaceData("current_user", user);
                    }
                    else 
                    { 
                        BagOfData.SetData("current_user", user);
                    }
                    return RedirectToAction("Expenses");
                }
            }

            var NewUser = System.Text.Json.JsonSerializer.Serialize<User>(new User(email), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            HttpContent request_content = new StringContent(NewUser, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"https://6464b03b127ad0b8f8a53cf7.mockapi.io/users/", request_content);

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
        public IActionResult Expenses()
        {
            GetAPIData().GetAwaiter().GetResult();

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