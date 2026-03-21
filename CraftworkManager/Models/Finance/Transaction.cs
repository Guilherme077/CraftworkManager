namespace CraftworkManager.Models.Finance
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public bool Pending { get; set; }

        public decimal GetSignedAmount()
        {
            return Type == TransactionType.Income ? Amount : -Amount;
        }
    }

    public enum TransactionType
    {
        Income,
        Expense
    }
}
