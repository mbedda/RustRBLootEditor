using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RustRBLootEditor.Validations
{
    public class ProbabilityRangeRule : ValidationRule
    {
        public float Min { get; set; }
        public float Max { get; set; }

        public ProbabilityRangeRule()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            float probability = 0;

            try
            {
                if (((string)value).Length > 0)
                    probability = float.Parse((String)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters or {e.Message}");
            }

            if ((probability < Min) || (probability > Max))
            {
                return new ValidationResult(false,
                  $"Please enter an probability in the range: {Min}-{Max}.");
            }
            return ValidationResult.ValidResult;
        }
    }

    public class AmountMinRule : ValidationRule
    {
        public float Min { get; set; }

        public AmountMinRule()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            long amount = 0;

            try
            {
                if (((string)value).Length > 0)
                    amount = long.Parse((String)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters or {e.Message}");
            }

            if ((amount < Min))
            {
                return new ValidationResult(false,
                  $"Please enter an amount with min: {Min}.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
