using System;
using System.Collections.ObjectModel;
using System.Windows;

/*
    AMB Calculator
    Created by Santhosh C P

    Bugs, Further Improvements:-
    * Fix the top Menu alignment
    * Add functionality to the MenuItems
 */


namespace AMB_Calculator
{
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
        }
        private void Initialize()
        {
            Transaction.SetIDGenSeed(0);
            logs = new ObservableCollection<Transaction>();
            TransactionListView.ItemsSource = logs;
            DataContext = this;
            DailyBalances = new ObservableCollection<DailyBalance>();
            DailyBalanceListView.ItemsSource = DailyBalances;
        }


        //Account Details Input
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
            Initialize();
            ComputeDailyBalances();
        }
        private void DisplayAccountDetails()
        {
            MonthDaysDisplay.Text = "Number of Days in the Current Month = " + MonthDays;
            MinimumBalanceDisplay.Text = "Minimum Balance = " + MinimumBalance;
            BalanceDisplay.Text = "Current Balance = " + ClosingBalance;
            OpeningBalanceDisplay.Text = "Opening Balance = " + OpeningBalance;
            UpdateDateDisplay.Text = "Balance Last Updated Date = " + lastUpdatedDate;
        }


        //Transactions
        private void SubmitTransaction_Click(object sender, RoutedEventArgs e)
        {
            int tdate;
            string ttype;
            double amount;

            int.TryParse(TransactionDate.Text, out tdate);
            if ((tdate <= 0) || (tdate > MonthDays))
            {
                // If a valid transaction date is not provided, the transaction date is assumed to be 'today'
                tdate = DateTime.Now.Day;
                TransactionDate.Text = DateTime.Now.Day.ToString();
            }
            Double.TryParse(TransactionAmount.Text, out amount);
            if (DebitRadioButton.IsChecked == true)
            {
                ttype = "Debit";
            }
            else
            {
                ttype = "Credit";
            }

            try
            {
                logs.Add(new Transaction(amount, tdate, ttype));
                ComputeDailyBalances();
            }
            catch (NullReferenceException)
            {
                TextBlockStatus.Text = "Please enter the account details first";
            }
        }
        private void DisplayAMBDetails()
        {
            AverageMonthlyBalanceDisplay.Text = "Average Monthly Balance = " + AverageBalance;

            if (AverageBalance > MinimumBalance)
            {
                double Excess = AverageBalance - MinimumBalance;
                AverageMonthlyBalanceAdviceDisplay.Text = "You are maintaining " + Excess + " more than the minimum balance";
                AverageMonthlyBalanceAdditionNeeded.Text = "";
            }
            else
            {
                double Shortfall = MinimumBalance - AverageBalance;
                double required = Math.Round(computeRequiredAddition(), 2);
                AverageMonthlyBalanceAdviceDisplay.Text = "You are maintaining " + Shortfall + " less than the minimum balance";
                AverageMonthlyBalanceAdditionNeeded.Text = "You need to add atleast " + required + " today and maintain the balance to meet the AMB requirements";
            }
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
                            CurrentDayBalance -= transaction.amount;
                        }
                        else
                        {
                            CurrentDayBalance += transaction.amount;
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
        private double computeRequiredAddition()
        {
            double RequiredDailyBalanceTotal;
            double CurrentDailyBalanceTotal = 0;
            double DailyBalanceTotalShortfall;
            double RequiredAddition = 0;
            int TodayDate;
            int RemainingDays;

            RequiredDailyBalanceTotal = MonthDays * MinimumBalance;
            TodayDate = DateTime.Now.Day;
            RemainingDays = MonthDays - TodayDate;
            foreach (DailyBalance db in DailyBalances)
            {
                CurrentDailyBalanceTotal += db.balance;
            }
            DailyBalanceTotalShortfall = RequiredDailyBalanceTotal - CurrentDailyBalanceTotal;

            if (DailyBalanceTotalShortfall > 0)
            {
                // Currently maintained AMB so far is less than the required amount
                RequiredAddition = DailyBalanceTotalShortfall / RemainingDays;
            }
            return RequiredAddition;
        }

        //Basic QoL Features
        private void ButtonMBAdd1000_Click(object sender, RoutedEventArgs e)
        {
            int value;

            int.TryParse(MinimumBalanceInput.Text, out value);
            value += 1000;
            MinimumBalanceInput.Text = value.ToString();
        }
        private void ButtonMBAdd5000_Click(object sender, RoutedEventArgs e)
        {
            int value;

            int.TryParse(MinimumBalanceInput.Text, out value);
            value += 5000;
            MinimumBalanceInput.Text = value.ToString();
        }
        private void ButtonDays30_Click(object sender, RoutedEventArgs e)
        {
            DaysInput.Text = "30";
        }
        private void ButtonDays31_Click(object sender, RoutedEventArgs e)
        {
            DaysInput.Text = "31";
        }
        private void ButtonUpdateDateToday_Click(object sender, RoutedEventArgs e)
        {
            UpdateDateInput.Text = DateTime.Now.Day.ToString();
        }
    }
}