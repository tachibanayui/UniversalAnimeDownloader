using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UniversalAnimeDownloader.Validation
{
    class IntergerValidation : ValidationRule
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int inputVal = 0;

            try
            {
                if(value.ToString().Length > 0)
                    inputVal = int.Parse(value.ToString());
            }
            catch(Exception e)
            {
                return new ValidationResult(false, "Invalid character or " + e.ToString());
            }

            return (inputVal < Min) || (inputVal > Max)
                ? new ValidationResult(false, $"Value is not in range from {Min} to {Max}")
                : ValidationResult.ValidResult;
        }
    }
}
