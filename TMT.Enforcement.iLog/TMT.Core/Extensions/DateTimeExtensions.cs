using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMT.Core.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Check to see if date1 plus days is greater than date2 plus days. Zero for no addition days.
        /// </summary>
        /// <returns><c>True</c> if valid</returns>
        public static bool IsGreater(this DateTime dateForValidation, DateTime dateAgainstValidation, int numOfDaysFor = 0, int numOfDaysAgainst = 0)
        {
            return DateTime.Compare(dateForValidation.AddDays(numOfDaysFor), dateAgainstValidation.AddDays(numOfDaysAgainst)) > 0;
        }

        /// <summary>
        ///     Check to see if date1 plus days is greater than/or equal to date2 plus days. Zero for no addition days.
        /// </summary>
        /// <returns><c>True</c> if Valid</returns>
        public static bool IsGreaterOrEquals(this DateTime dateForValidation, DateTime dateAgainstValidation, int numOfDaysFor = 0, int numOfDaysAgainst = 0)
        {
            return (DateTime.Compare(dateForValidation.AddDays(numOfDaysFor), dateAgainstValidation.AddDays(numOfDaysAgainst)) >= 0);
        }

        /// <summary>
        ///     Check to see if date1 plus days is less than date2 plus days. Zero for no addition days.
        /// </summary>
        /// <returns><c>True</c> if valid</returns>
        public static bool IsLess(this DateTime dateForValidation, DateTime dateAgainstValidation, int numOfDaysFor = 0, int numOfDaysAgainst = 0)
        {
            return (DateTime.Compare(dateForValidation.AddDays(numOfDaysFor), dateAgainstValidation.AddDays(numOfDaysAgainst)) < 0);
        }

        /// <summary>
        ///     Check to see if date1 plus days is less than/or equal to date2 plus days. Zero for no addition days.
        /// </summary>
        /// <returns><c>True</c> if in future</returns>
        public static bool IsLessOrEquals(this DateTime dateForValidation, DateTime dateAgainstValidation, int numOfDaysFor = 0, int numOfDaysAgainst = 0)
        {
            return (DateTime.Compare(dateForValidation.AddDays(numOfDaysFor), dateAgainstValidation.AddDays(numOfDaysAgainst)) <= 0);
        }

        /// <summary>
        ///     Check to see if date1 plus days is equal to date2 plus days. Zero for no addition days.
        /// </summary>
        /// <returns><c>True</c> if valid</returns>
        public static bool IsEquals(this DateTime dateForValidation, DateTime dateAgainstValidation, int numOfDaysFor = 0, int numOfDaysAgainst = 0)
        {
            return (DateTime.Compare(dateForValidation.Date.AddDays(numOfDaysFor), dateAgainstValidation.Date.AddDays(numOfDaysAgainst)) == 0);
        }

        /// <summary>
        ///     Check to see if date1 is greater than date2 and less than date3.
        /// </summary>
        /// <returns><c>True</c> if valid</returns>
        public static bool IsBetween(this DateTime dateForValidation, DateTime dateStartValidation, DateTime dateEndValidation)
        {
            return (dateForValidation > dateStartValidation && dateForValidation < dateEndValidation);
        }

        /// <summary>
        ///     Check to see if date1 is greater than date2 and less than date3.
        /// </summary>
        /// <returns><c>True</c> if valid</returns>
        public static bool IsBetweenIncluding(this DateTime dateForValidation, DateTime dateStartValidation, DateTime dateEndValidation)
        {
            return (dateForValidation >= dateStartValidation && dateForValidation <= dateEndValidation);
        }

        /// <summary>
        ///     Check to see if date is in the week
        /// </summary>
        /// <returns><c>True</c> if valid weekday</returns>
        public static bool IsWeekday(this DateTime dateForValidation)
        {
            if (dateForValidation.DayOfWeek == DayOfWeek.Saturday || dateForValidation.DayOfWeek == DayOfWeek.Sunday)
                return false;

            return true;
        }
    }
}
