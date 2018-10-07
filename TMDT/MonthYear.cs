using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT
{
    public class MonthYear
    {
        private int month;
        private int year;

        public MonthYear(int month, int year)
        {
            this.Month = month;
            this.Year = year;
        }

        public int Month { get => month; set => month = value; }
        public int Year { get => year; set => year = value; }
    }
}