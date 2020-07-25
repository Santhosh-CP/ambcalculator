using System;
using System.Collections.ObjectModel;
using System.Windows;


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

            //Debug_PreloadAccountData();
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
        private void Debug_PreloadAccountData()
        {
            //Used to quickly preload data to help in debugging
            DaysInput.Text = "31";
            MinimumBalanceInput.Text = "10000";
            BalanceInput.Text = "9110";
            UpdateDateInput.Text = "14";
            OpeningBalanceInput.Text = "14400";
        }


        //Transactions
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
        private void DisplayAMBDetails()
        {
            AverageMonthlyBalanceDisplay.Text = "Average Monthly Balance = " + AverageBalance;

            if (AverageBalance > MinimumBalance)
            {
                double Excess = AverageBalance - MinimumBalance;
                AverageMonthlyBalanceAdviceDisplay.Text = "You are maintaining " + Excess + " more than the minimum balance";

            }
            else
            {
                double Shortfall = MinimumBalance - AverageBalance;
                double required = Math.Round(computeRequiredAddition(), 2);
                AverageMonthlyBalanceAdviceDisplay.Text = "You are maintaining " + Shortfall + " less than the minimum balance";
                AverageMonthlyBalanceAdditionNeeded.Text = "You need to add atleast " + required + " today to meet the AMB requirements";
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
        private void ButtonMinimumBalance2000_Click(object sender, RoutedEventArgs e)
        {
            MinimumBalanceInput.Text = "2000";
        }
        private void ButtonMinimumBalance10000_Click(object sender, RoutedEventArgs e)
        {
            MinimumBalanceInput.Text = "10000";
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