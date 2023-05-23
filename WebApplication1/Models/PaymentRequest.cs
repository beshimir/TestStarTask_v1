namespace WebApplication1.Models
{
    public class PaymentRequest
    {
        public string to { get; set; }
        public string from { get; set; }
        public int amountDue { get; set; }
        public int amountPaid { get; set; }

        public PaymentRequest(string to, string from, int amountDue, int amountPaid) 
        {
            this.to = to;
            this.from = from;
            this.amountDue = amountDue;
            this.amountPaid = amountPaid;
        }
    }
}
