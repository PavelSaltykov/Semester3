using System.Globalization;
using System.Net;
using System.Windows.Controls;

namespace Gui.ValidationRules
{
    public class PortValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!int.TryParse(value.ToString(), out var port))
                return new ValidationResult(false, "Port must be an integer.");

            if (port < IPEndPoint.MinPort)
                return new ValidationResult(false, $"Port cannot be less than {IPEndPoint.MinPort}.");

            if (port > IPEndPoint.MaxPort)
                return new ValidationResult(false, $"Port cannot be greater than {IPEndPoint.MaxPort}.");

            return new ValidationResult(true, null);
        }
    }
}
