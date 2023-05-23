namespace WebApplication1.Models
{
    public class Expense
    {
        public string description { get; set; }
        public int amount { get; set; }
        //public List<PaymentRequest> debts_outgoing { get; set; }
        //public List<PaymentRequest> debts_incoming { get; set; }

        public Expense(string description, int amount/*, List<PaymentRequest> debts_outgoing, List<PaymentRequest> debts_incoming*/)
        {
            this.description = description;
            this.amount = amount;
          //  this.debts_outgoing = debts_outgoing;
          //  this.debts_incoming = debts_incoming;
        }
    }
}
