using System;

namespace Utils
{
    public class MonthConverter
    {
        public string DateConverter(int year, int month)
        {
            return new DateTime(year, month, 1).ToString("MMM yyyy");
        }
    }
}