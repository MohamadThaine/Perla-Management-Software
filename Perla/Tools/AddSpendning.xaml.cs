using Perla.classes;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Perla.Tools
{
    /// <summary>
    /// Interaction logic for AddSpendning.xaml
    /// </summary>
    public partial class AddSpendning : Window
    {
        public AddSpendning()
        {
            InitializeComponent();
        }

        private void AddSpendingToDB(object sender, RoutedEventArgs e)
        {
            if (!isDataValid())
                return;
            string[] Values = new string[4];
            Values[0] = "0";
            Values[1] = MoneyAmount.Text;
            Values[2] = SpendingDate.SelectedDate.Value.ToString("yyyy-MM-dd");
            Values[3] = (string.IsNullOrEmpty(SpendingReason.Text)) ? "0" : SpendingReason.Text;
            try
            {
                DBManager.InsertToDB("spendinginfo", Values);
            }
            catch (MySql.Data.MySqlClient.MySqlException Error)
            {
                MessageBox.Show(Error.Message);
            }
        }
        private bool isDataValid()
        {
            if (MoneyAmount.Text == null || SpendingDate.SelectedDate == null)
            {
                MessageBox.Show("لا تترك الحقول المطلوبه فارغه");
                return false;
            }
            return true;
        }

        private void NumberOnlyTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
