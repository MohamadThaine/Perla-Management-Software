using System;

namespace Perla.classes
{
    public class Expenses
    {
        public int ID { get; set; }
        public double MoneySpent { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public Expenses() { }
        public Expenses(int iD, double moneySpent, DateTime date, string? description)
        {
            ID = iD;
            MoneySpent = moneySpent;
            Date = date;
            Description = description;
        }
    }
}
