using System;

namespace Perla.classes
{
    public class Appoitment
    {
        public int ID { get; set; }
        public DateTime Appointment_Data { get; set; }
        public string Customer_ID { get; set; }
        public string Treatment { get; set; }
        public double MoneyPaid { get; set; }
        public Appoitment() { }
        public Appoitment(int appoitmentID, DateTime appoitmentDate, string customer_id, string treatment, double paidMoney)
        {
            ID = appoitmentID;
            Appointment_Data = appoitmentDate;
            Customer_ID = customer_id;
            Treatment = treatment;
            MoneyPaid = paidMoney;
        }
    }
}
