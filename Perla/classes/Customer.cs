namespace Perla.classes
{
    public class Customer
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public double MoneyPaid { get; set; }
        public Customer() { }
        public Customer(string idCustomer, string Name, string PhoneNumber, double MoneyPaid)
        {
            this.ID = idCustomer;
            this.Name = Name;
            this.PhoneNumber = PhoneNumber;
            this.MoneyPaid = MoneyPaid;
        }
        public Customer(Customer customer)
        {
            this.ID = customer.ID;
            this.Name = customer.Name;
            this.PhoneNumber = customer.PhoneNumber;
            this.MoneyPaid = customer.MoneyPaid;
        }
    }
}
