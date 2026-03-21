using CraftworkManager.Models;

namespace CraftworkManager.Models.Finance
{
    public class FinanceDashboardViewModel
    {
        public List<Transaction> LastTransactions { get; set; } = new();
        public decimal TotalProfit { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public List<Shipment> ShipmentsWithoutIncome { get; set; } = new();
    }
}