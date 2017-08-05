using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Client.Extentions
{
    public class RowDataInfoValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            BindingGroup group = (BindingGroup)value;

            StringBuilder error = null;
            foreach (var item in group.Items)
            {                
                IDataErrorInfo info = item as IDataErrorInfo;
                if (!string.IsNullOrEmpty(info?.Error))
                {
                    if (error == null)
                        error = new StringBuilder();
                    error.Append((error.Length != 0 ? ", " : "") + info.Error);
                }
            }

            if (error != null)
                return new ValidationResult(false, error.ToString());

            return ValidationResult.ValidResult;
        }
    }
}
