namespace Perla.classes
{
    public class CustomerAppoitments
    {
        public Customer Customer { get; set; }
        public Appoitment Appoitment { get; set; }
        public CustomerAppoitments() { }
        public CustomerAppoitments(Customer customer, Appoitment appoitment)
        {
            Customer = customer;
            Appoitment = appoitment;
        }
    }
}
