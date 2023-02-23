using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;

namespace Perla.classes
{
    public class DBManager
    {
        private static MySqlConnection Connection = new MySqlConnection();
        public DBManager()
        {

        }
        public static long InsertToDB(string TableName, string[] Values)
        {
            if (TableName == null) return 0;
            CheckConnetion();
            string SQLstatment = "INSERT INTO `" + TableName + "` VALUES (";
            for (int i = 0; i < Values.Length; i++)
            {
                SQLstatment += "'" + Values[i] + "'";
                if (i != Values.Length - 1)
                    SQLstatment += " ,";
                else
                    SQLstatment += ")";
            }
            MySqlCommand SqlCommand = new MySqlCommand(SQLstatment, Connection);
            SqlCommand.ExecuteNonQuery();
            return SqlCommand.LastInsertedId;
        }

        public static ObservableCollection<T> GetDataFromDB<T>(string TableName, string SelectedAttributes, string WhereAttributeName, string AttributeValue)
        {
            if (TableName == null) return null;
            string SQLstatment = "";
            CheckConnetion();
            ObservableCollection<T> ReturnedData = new ObservableCollection<T>();
            if (WhereAttributeName == null) { SQLstatment = "SELECT " + SelectedAttributes + " FROM " + TableName; }
            else { SQLstatment = "SELECT " + SelectedAttributes + " FROM " + TableName + " WHERE " + WhereAttributeName + " = " + AttributeValue; }
            MySqlDataReader DataReader = null;
            MySqlCommand SqlCommand;
            using (SqlCommand = new MySqlCommand(SQLstatment, Connection))
            {
                DataReader = SqlCommand.ExecuteReader();
                while (DataReader.Read())
                {
                    var ObjectType = typeof(T);
                    T Object = (T)Activator.CreateInstance(ObjectType);
                    foreach (var Properity in ObjectType.GetProperties())
                    {
                        var PropType = Properity.PropertyType;
                        Properity.SetValue(Object, Convert.ChangeType(DataReader[Properity.Name].ToString(), PropType));
                    }
                    ReturnedData.Add(Object);
                }
                DataReader.Close();
            }
            return ReturnedData;
        }

        public static int GetAppoitmentID(double CustomerID, DateTime AppoitmentDate)
        {
            CheckConnetion();
            int ID = 0;
            string SQLstatment = "";
            if (CustomerID != 0)
            {
                SQLstatment = "SELECT ID FROM appointment WHERE Customer_ID = " + CustomerID + " AND Appointment_Data BETWEEN '" +
                    AppoitmentDate.Date.ToString("yyyy-MM-dd") + "' AND '" + AppoitmentDate.Date.ToString("yyyy-MM-dd") + " 23:59:59'";
            }
            else
            {
                SQLstatment = "SELECT ID FROM appointment WHERE Appointment_Data = '" + AppoitmentDate.ToString("yyyy-MM-dd HH-mm-ss") + "'";
            }
            MySqlDataReader DataReader = null;
            MySqlCommand SqlCommand = new MySqlCommand(SQLstatment, Connection);
            DataReader = SqlCommand.ExecuteReader();
            try
            {
                DataReader.Read();
                ID = DataReader.GetInt32(0);
                DataReader.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException Error)
            {
                DataReader.Close();
            }
            return ID;
        }

        public static void UpdateAppoitmentDate(int ID, string AppoitmentDate)
        {
            CheckConnetion();
            string SQLstatment = "UPDATE appointment SET `Appointment_Data` = '" + AppoitmentDate + "' WHERE (`ID` = '" + ID + "')";
            MySqlCommand SqlCommand = new MySqlCommand(SQLstatment, Connection);
            SqlCommand.ExecuteNonQuery();
        }

        public static void UpdateCustomerData(double OldID, string[] newValues)
        {
            CheckConnetion();
            string SQLstatement = "SET FOREIGN_KEY_CHECKS=0";
            MySqlCommand SqlCommand = new MySqlCommand(SQLstatement, Connection);
            SqlCommand.ExecuteNonQuery();
            SQLstatement = "UPDATE customer SET `ID` = '" + newValues[0] + "', `Name` = '" + newValues[1] + "', `PhoneNumber` = '"
                + newValues[2] + "', `MoneyPaid` = '" + newValues[3] + "'  WHERE (`ID` = '" + OldID + "')";
            SqlCommand = new MySqlCommand(SQLstatement, Connection);
            SqlCommand.ExecuteNonQuery();
            if (OldID != Convert.ToDouble(newValues[0]))
            {
                SQLstatement = "UPDATE appointment SET `Customer_ID` = '" + newValues[0] + "' WHERE (`Customer_ID` = '" + OldID + "')";
                SqlCommand = new MySqlCommand(SQLstatement, Connection);
                SqlCommand.ExecuteNonQuery();
            }
            SQLstatement = "SET FOREIGN_KEY_CHECKS=1";
            SqlCommand = new MySqlCommand(SQLstatement, Connection);
            SqlCommand.ExecuteNonQuery();
        }

        public static void AddPayment(double CustomerID, int AppoitmentID, double PaidMoney)
        {
            CheckConnetion();
            string SQLstatement = "UPDATE customer SET MoneyPaid = MoneyPaid + " + PaidMoney + " WHERE ID = " + CustomerID;
            MySqlCommand SqlCommand = new MySqlCommand(SQLstatement, Connection);
            SqlCommand.ExecuteNonQuery();
            SQLstatement = "UPDATE appointment SET MoneyPaid = " + PaidMoney + " WHERE ID = " + AppoitmentID;
            SqlCommand = new MySqlCommand(SQLstatement, Connection);
            SqlCommand.ExecuteNonQuery();
        }
        public static void DeleteRow(string TableName, string AttributeName, string AttributeValue)
        {
            if (TableName == null) return;
            CheckConnetion();
            string SQLStatement = "DELETE FROM " + TableName + " WHERE (`" + AttributeName + "` = '" + AttributeValue + "')";
            MySqlCommand SqlCommand = new MySqlCommand(SQLStatement, Connection);
            SqlCommand.ExecuteNonQuery();
        }

        private static void CheckConnetion()
        {
            if (Connection.State != System.Data.ConnectionState.Open)
            {
                string ConnetionString = @"user id=root;password=123321Aa.;server=localhost;database=perla;persistsecurityinfo=True";
                Connection = new MySqlConnection(ConnetionString);
                Connection.Open();
            }
        }
    }
}
