using System.Text.RegularExpressions;

namespace Naspinski.FoodTruck.Admin.Extensions
{
    public static class StringExtensions
    {
        public static string CamelCaseToSpacedString(this string s)
        {
            return Regex.Replace(s, "(\\B[A-Z])", " $1");
        }
    }
}
