using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YandexMapParser.Infrastructure
{
    static class FormatAddressService
    {
        public static string ReverseAddressStr(string address)
        {
            IEnumerable<string> addressElements = address.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

            var fliteredAddressElements = addressElements.Where(ae => IncludeAddressElement(ae));

            return string.Join(", ", fliteredAddressElements.Reverse());
        }

        static bool IncludeAddressElement(string address)
        {
            IEnumerable<string> valuesToExclude = new List<string>
            {
                "Россия",
                "Московская область",
            };

            string regexStr = @"^[\d]{5,}$";
            var regex = new Regex(regexStr);

            return valuesToExclude.All(exluded => exluded.Trim().ToLower() != address.Trim().ToLower()) && !regex.IsMatch(address.Trim());
        }
    }
}
