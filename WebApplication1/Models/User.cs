namespace WebApplication1.Models
{
    public class User
    {
        public string id { get; set; }
        public string email { get; set; }
        public List<Expense> expenses { get; set; }

        public User(string id, string email, List<Expense> expenses)
        {
            this.id = id;
            this.email = email;
            this.expenses = expenses;
        }

        public User(string email)
        {
            id = Guid.NewGuid().ToString();
            this.email = email;
            expenses = new List<Expense>();
        }

        public User()
        {
            id = Guid.NewGuid().ToString();
            email = "";
            expenses = new List<Expense>();
        }
    }
}
