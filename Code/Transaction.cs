namespace AMB_Calculator
{
    public class Transaction
    {
        public int ID { get; set; }
        public double amount { get; set; }
        public int tdate { get; set; }
        public string ttype { get; set; }

        private static int IDGenSeed;

        public Transaction(double amount, int tdate, string ttype)
        {
            IDGenSeed++;
            ID = IDGenSeed;
            this.amount = amount;
            this.tdate = tdate;
            this.ttype = ttype;
        }

        public static void SetIDGenSeed(int Seed)
        {
            IDGenSeed = Seed;
        }
    }

    public class DailyBalance
    {
        public int tdate { get; set; }
        public double balance { get; set; }

        public DailyBalance(int tdate, double balance)
        {
            this.tdate = tdate;
            this.balance = balance;
        }
    }
}
