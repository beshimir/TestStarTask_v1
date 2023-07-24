namespace WebApplication1.Models
{
    public class Payment
    {
        public string id { get; set; }
        public string email { get; set; }
        public int amountDue { get; set; }
        public int amountPaid { get; set; }

        public Payment(string email, int amountDue, int amountPaid)
        {
            this.id = Guid.NewGuid().ToString();
            this.email = email;
            this.amountDue = amountDue;
            this.amountPaid = amountPaid;
        }

        public Payment()
        {
            id = Guid.NewGuid().ToString();
            email = string.Empty;
            amountDue = 0;
            amountPaid = 0;
        }
    }
}
