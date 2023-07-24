namespace WebApplication1.Models
{
    public class Expense
    {
        //public string id { get; set; }
        public string name { get; set; }
        public string date { get; set; }
        public string description { get; set; }
        public int amount { get; set; }
        public List<Payment> duePaymentsOutbound { get; set; }
        public List<Payment> duePaymentsInbound { get; set; }

        public Expense(/*string id,*/ string name, string date, string description, int amount) 
        {
            this.name = name;
            this.date = date;
            this.description = description;
            this.amount = amount;
            this.duePaymentsInbound = new List<Payment>();
            this.duePaymentsOutbound = new List<Payment>();
        }
    }
}
