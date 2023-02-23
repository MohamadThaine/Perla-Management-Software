using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using Perla.classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Paragraph = iTextSharp.text.Paragraph;

namespace Perla.Tools
{
    /// <summary>
    /// Interaction logic for Accounting.xaml
    /// </summary>
    public partial class Accounting : Window
    {
        public Accounting()
        {
            InitializeComponent();
        }

        private void GeneratePDF(object sender, RoutedEventArgs e)
        {
            ComboBoxItem TypeCombobox = (ComboBoxItem)DataType.SelectedItem;
            string TypeChoosed = TypeCombobox.Content.ToString() + "";
            string Location = GetSaveLocation();
            int StartDate = GetStartDate();
            int DataTypeInt = DataType.SelectedIndex;
            if (Location == "" || StartDate == -1)
                return;
            var FontColor = new BaseColor(0, 0, 0);
            string FontPath = "D:\\ARIALUNI.ttf";
            BaseFont ArabicFont = BaseFont.CreateFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var Font = new Font(ArabicFont, 10);
            ArabicLigaturizer Arabic = new ArabicLigaturizer();
            FileStream FileCreator = null;
            try
            {
                FileCreator = new FileStream(Location, FileMode.Create);
            }
            catch (Exception ex)
            {
                MessageBox.Show("لا يمكنك تعديل على الملف وهو مفتوح");
                return;
            }
            Document Pdf = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter PdfWriter = PdfWriter.GetInstance(Pdf, FileCreator);
            Pdf.Open();
            Pdf.AddLanguage("Arabic");
            Arabic.IsRTL();
            Paragraph Header = new Paragraph(Arabic.Process(TypeChoosed), Font);
            Paragraph Space = new Paragraph("\n", Font);
            Header.Alignment = Element.ALIGN_CENTER;
            Pdf.Add(Header);
            Pdf.Add(Space);
            GetData(Pdf, Arabic, Font, StartDate, DataTypeInt);
            Pdf.Close();
            PdfWriter.Close();
            FileCreator.Close();
            MessageBoxResult ButtonClicked = MessageBox.Show("هل تريد فتح الملف؟", "فتح الملف", MessageBoxButton.YesNo);
            if (ButtonClicked == MessageBoxResult.Yes)
            {
                Process PdfOpener = new Process();
                PdfOpener.StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = Location,
                };
                PdfOpener.Start();
            }
        }
        private string GetSaveLocation()
        {
            string Location = "";
            SaveFileDialog SaveLocation = new SaveFileDialog();
            SaveLocation.Filter = "PDF File (*.pdf)|*.pdf";
            SaveLocation.RestoreDirectory = true;
            if (SaveLocation.ShowDialog() == true)
            {
                Location = SaveLocation.FileName;
                string Extention = Location.Substring(Location.Length - 4);
                if (!string.Equals(Extention, ".pdf", StringComparison.OrdinalIgnoreCase))
                    Location += ".pdf";
            }
            return Location;
        }

        private int GetStartDate()
        {
            if (GetDataFromDate.SelectedIndex == 0) return 1;
            else if (GetDataFromDate.SelectedIndex == 1) return 7;
            else if (GetDataFromDate.SelectedIndex == 2) return 30;
            else if (GetDataFromDate.SelectedIndex == 3) return 365;
            else if (GetDataFromDate.SelectedIndex == 4) return -2;
            return -1;
        }

        private void GetData(Document PDF, ArabicLigaturizer Arabic, Font Font, int StartDate, int DataType)
        {
            Paragraph Space = new Paragraph("\n", Font);
            List<Expenses> ExpensesList = new List<Expenses>();
            Dictionary<DateTime, double> ExpenseMoney = new Dictionary<DateTime, double>();
            Dictionary<DateTime, double> AppoitmentMoney = new Dictionary<DateTime, double>();
            Dictionary<DateTime, double> ProfitMoney = new Dictionary<DateTime, double>();
            DateTime FirstDate;
            double CashOutFlow = 0;
            double CashInFlow = 0;
            double CashFlow = 0;
            if (StartDate == -2)
            {
                FirstDate = PrepareData.appoitmentList.Select(app => app.Appointment_Data.Date).OrderBy(app => app.Date).First().Date;
            }
            else
            {
                FirstDate = DateTime.Today.AddDays(-StartDate).Date;
            }
            if (DataType != 2)
            {
                ExpensesList = DBManager.GetDataFromDB<Expenses>("spendinginfo", "*", null, "0").ToList();
                if (DataType == 0)
                {
                    ExpenseMoney = (from exp in ExpensesList
                                    group exp.MoneySpent by exp.Date.Date)
                                .Where(exp => exp.Key.Date > FirstDate && exp.Key.Date < DateTime.Today.Date)
                                .ToDictionary(exp => exp.Key, exp => exp.Sum(exp => exp));
                    CashOutFlow = ExpenseMoney.Sum(outflow => outflow.Value);
                }
                else
                {
                    ExpensesList = ExpensesList.Where(exp => exp.Date.Date > FirstDate && exp.Date.Date < DateTime.Today.Date).ToList();
                    CashOutFlow = ExpensesList.Sum(outflow => outflow.MoneySpent);
                }
            }
            if (DataType != 1)
            {
                AppoitmentMoney = (from app in PrepareData.appoitmentList
                                   group app.MoneyPaid by app.Appointment_Data.Date)
                                                            .Where(app => app.Key.Date > FirstDate && app.Key.Date < DateTime.Today.Date)
                                                            .ToDictionary(app => app.Key, app => app.Sum(app => app));
                CashInFlow = AppoitmentMoney.Sum(inflow => inflow.Value);
            }
            if (DataType == 0)
            {
                ProfitMoney = (from exp in ExpenseMoney
                               join app in AppoitmentMoney
                               on exp.Key.Date equals app.Key.Date
                               select new
                               {
                                   Key = exp.Key.Date,
                                   Value = app.Value - exp.Value
                               }).ToDictionary(profit => profit.Key, profit => profit.Value);
                foreach (KeyValuePair<DateTime, double> KeyValue in AppoitmentMoney)
                {
                    if (!ProfitMoney.ContainsKey(KeyValue.Key.Date))
                        ProfitMoney.Add(KeyValue.Key, KeyValue.Value);
                }
                foreach (KeyValuePair<DateTime, double> KeyValue in ExpenseMoney)
                {
                    if (!ProfitMoney.ContainsKey(KeyValue.Key.Date))
                        ProfitMoney.Add(KeyValue.Key, -KeyValue.Value);
                }
                CashFlow = CashInFlow - CashOutFlow;
            }
            double MoneyAmountDouble = (DataType == 0) ? CashFlow : (DataType == 1) ? -CashOutFlow : CashInFlow;
            Paragraph MoneyAmount = new Paragraph(Arabic.Process("المبلغ:" + MoneyAmountDouble.ToString()), Font);
            MoneyAmount.Alignment = Element.ALIGN_CENTER;
            PDF.Add(MoneyAmount);
            PDF.Add(Space);
            string DateFrom = "التاريخ من " + FirstDate.ToString("dd/MM/yyyy") +
                              " الى " + DateTime.Today.ToString("dd/MM/yyyy");
            Paragraph DataDate = new Paragraph(Arabic.Process(DateFrom), Font);
            DataDate.Alignment = Element.ALIGN_CENTER;
            PDF.Add(DataDate);
            PDF.Add(Space);
            PdfPTable Table = new PdfPTable(2);
            Phrase Titles = new Phrase(Arabic.Process("المبلغ"), Font);
            Table.AddCell(Titles);
            Titles = new Phrase(Arabic.Process("التاريخ"), Font);
            Table.AddCell(Titles);
            if (DataType == 0)
            {
                foreach (KeyValuePair<DateTime, double> KeyValue in ProfitMoney)
                {
                    Phrase Amount = new Phrase(Arabic.Process(KeyValue.Value.ToString()), Font);
                    Table.AddCell(Amount);
                    Phrase Date = new Phrase(Arabic.Process(KeyValue.Key.Date.ToString("dd/MM/yyyy")), Font);
                    Table.AddCell(Date);
                }
            }
            else if (DataType == 1)
            {
                Table = new PdfPTable(3);
                Titles = new Phrase(Arabic.Process("السبب"), Font);
                Table.AddCell(Titles);
                Titles = new Phrase(Arabic.Process("المبلغ"), Font);
                Table.AddCell(Titles);
                Titles = new Phrase(Arabic.Process("التاريخ"), Font);
                Table.AddCell(Titles);
                foreach (Expenses expense in ExpensesList)
                {

                    Phrase Description = new Phrase(Arabic.Process(expense.Description.ToString()), Font);
                    Table.AddCell(Description);
                    Phrase Amount = new Phrase(Arabic.Process(Convert.ToString(-expense.MoneySpent)), Font);
                    Table.AddCell(Amount);
                    Phrase Date = new Phrase(Arabic.Process(expense.Date.ToString("dd/MM/yyyy")), Font);
                    Table.AddCell(Date);
                }
            }
            else if (DataType == 2)
            {
                foreach (KeyValuePair<DateTime, double> KeyValue in AppoitmentMoney)
                {
                    Phrase Amount = new Phrase(Arabic.Process(KeyValue.Value.ToString()), Font);
                    Phrase Date = new Phrase(Arabic.Process(KeyValue.Key.Date.ToString("dd/MM/yyyy")), Font);
                    Table.AddCell(Amount);
                    Table.AddCell(Date);
                }
            }
            PDF.Add(Table);
        }
    }
}
