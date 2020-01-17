using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AMB_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int MonthDays { get; set; }
        public double MinimumBalance { get; set; }
        public double ClosingBalance { get; set; }
        public double OpeningBalance { get; set; }
        public int lastUpdatedDate { get; set; }
        ObservableCollection<Transaction> logs { get; set; }
        ObservableCollection<DailyBalance> DailyBalances { get; set; }
        public double AverageBalance { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            logs = new ObservableCollection<Transaction>();
            TransactionListView.ItemsSource = logs;
            DataContext = this;
            DailyBalances = new ObservableCollection<DailyBalance>();
            DailyBalanceListView.ItemsSource = DailyBalances;

            //Debug_PreloadAccountData();
        }

        private void SubmitTransaction_Click(object sender, RoutedEventArgs e)
        {
            int tdate;
            string ttype;
            double amount;

            int.TryParse(TransactionDate.Text, out tdate);
            Double.TryParse(TransactionAmount.Text, out amount);
            if (DebitRadioButton.IsChecked == true)
            {
                ttype = "Debit";
            }
            else
            {
                ttype = "Credit";
            }
            logs.Add(new Transaction(amount, tdate, ttype));
            ComputeDailyBalances();
        }

        private void SubmitDetails_Click(object sender, RoutedEventArgs e)
        {
            double dtemp;
            int itemp;

            int.TryParse(DaysInput.Text, out itemp);
            MonthDays = itemp;

            Double.TryParse(MinimumBalanceInput.Text, out dtemp);
            MinimumBalance = dtemp;

            Double.TryParse(BalanceInput.Text, out dtemp);
            ClosingBalance = dtemp;

            int.TryParse(UpdateDateInput.Text, out itemp);
            lastUpdatedDate = itemp;

            Double.TryParse(OpeningBalanceInput.Text, out dtemp);
            OpeningBalance = dtemp;

            DisplayAccountDetails();
            DailyBalances.Clear();
        }

        private void DisplayAccountDetails()
        {
            MonthDaysDisplay.Text = "Number of Days in the Current Month = " + MonthDays;
            MinimumBalanceDisplay.Text = "Minimum Balance = " + MinimumBalance;
            BalanceDisplay.Text = "Current Balance = " + ClosingBalance;
            OpeningBalanceDisplay.Text = "Opening Balance = " + OpeningBalance;
            UpdateDateDisplay.Text = "Balance Last Updated Date = " + lastUpdatedDate;
        }

        private void DisplayAMBDetails()
        {


            AverageMonthlyBalanceDisplay.Text = "Average Monthly Balance = " + AverageBalance;

            if (AverageBalance > MinimumBalance)
            {
                double Excess = AverageBalance - MinimumBalance;
                AverageMonthlyBalanceAdviveDisplay.Text = "You are maintaining " + Excess + " more than the minimum balance";
            }
            else
            {
                double Shortfall = MinimumBalance - AverageBalance;
                AverageMonthlyBalanceAdviveDisplay.Text = "You are maintaining " + Shortfall + " less than the minimum balance";
            }
        }
        private void Debug_PreloadAccountData()
        {
            //Used to quickly preload data to help in debugging
            MonthDays = 30;
            MinimumBalance = 5000;
            ClosingBalance = 2000;
            OpeningBalance = 1000;
            lastUpdatedDate = 30;

            DisplayAccountDetails();
        }
        private void ComputeDailyBalances()
        {
            double CurrentDayBalance;
            int CurrentDate;
            double[] LeastDailyBal;
            double TotalBalance;

            LeastDailyBal = new double[MonthDays + 1];
            CurrentDate = 1;
            CurrentDayBalance = OpeningBalance;
            DailyBalances.Clear();
            TotalBalance = 0;

            while (CurrentDate <= MonthDays)
            {
                LeastDailyBal[CurrentDate] = CurrentDayBalance;
                foreach (Transaction transaction in logs)
                {
                    if (transaction.tdate == CurrentDate)
                    {
                        if (transaction.ttype == "Debit")
                        {
                            CurrentDayBalance = CurrentDayBalance - transaction.amount;
                        }
                        else
                        {
                            CurrentDayBalance = CurrentDayBalance + transaction.amount;
                        }

                        if (LeastDailyBal[CurrentDate] > CurrentDayBalance)
                        {
                            LeastDailyBal[CurrentDate] = CurrentDayBalance;
                        }
                    }
                }

                DailyBalances.Add(new DailyBalance(CurrentDate, LeastDailyBal[CurrentDate]));
                TotalBalance = TotalBalance + LeastDailyBal[CurrentDate];
                CurrentDate++;
            }
            AverageBalance = TotalBalance / MonthDays;
            DisplayAMBDetails();

            DailyBalanceListView.ItemsSource = DailyBalances;
        }

    }
}