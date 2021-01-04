using System.Globalization;
using System.Net;
using System.Windows.Controls;

namespace Gui.ValidationRules
{
    public class IpValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var valueString = value as string;
            var splitValue = valueString.Split('.');
            if (!IPAddress.TryParse(valueString, out _) || splitValue.Length != 4)
                return new ValidationResult(false, "Incorrect ip address.");

            return new ValidationResult(true, null);
        }
    }
}
