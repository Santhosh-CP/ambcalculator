using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB_Calculator
{
    public class Transaction
    {
        public int id { get; set; }
        public double amount { get; set; }
        public int tdate { get; set; }
        public string ttype { get; set; }

        private static int idGenSeed = 0;

        public Transaction(double amount,int tdate,string ttype)
        {
            idGenSeed++;
            id = idGenSeed;
            this.amount = amount;
            this.tdate = tdate;
            this.ttype = ttype;
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
