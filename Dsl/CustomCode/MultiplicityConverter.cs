using System;
using System.ComponentModel;
using System.Globalization;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Custom type converter for Multiplicity enum to display simplified notation
    /// </summary>
    public class MultiplicityConverter : EnumConverter
    {
        public MultiplicityConverter() : base(typeof(Multiplicity))
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Multiplicity)
            {
                Multiplicity multiplicity = (Multiplicity)value;
                
                // Convert enum values to simplified notation
                switch (multiplicity)
                {
                    case Multiplicity.ZeroOne:
                        return "0..1";
                    case Multiplicity.One:
                        return "1";
                    case Multiplicity.Many:
                        return "*";
                    default:
                        return multiplicity.ToString();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string stringValue = ((string)value).Trim();
                
                // Parse notation back to enum values
                switch (stringValue)
                {
                    case "0..1":
                        return Multiplicity.ZeroOne;
                    case "1":
                        return Multiplicity.One;
                    case "*":
                        return Multiplicity.Many;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new Multiplicity[] 
            { 
                Multiplicity.ZeroOne, 
                Multiplicity.One, 
                Multiplicity.Many 
            });
        }
    }
}
