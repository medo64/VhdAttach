//Josip Medved <jmedved@jmedved.com> http://www.jmedved.com

//2008-03-29: Initial version.
//2008-11-15: All methods now accept long instead of double (to be in sync with IO classes).


using System;
using System.Globalization;

namespace Medo.Extensions {

	/// <summary>
	/// Conversions to closest binary IEC prefix.
	/// This extension methods are intended for double.
	/// </summary>
    public static class BinaryPrefixExtensions {

        private static readonly double[] prefixBigValues = new double[] { System.Math.Pow(2, 60), System.Math.Pow(2, 50), System.Math.Pow(2, 40), System.Math.Pow(2, 30), System.Math.Pow(2, 20), System.Math.Pow(2, 10) };
        private static readonly string[] prefixBigTexts = new string[] { "exbi", "pebi", "tebi", "gibi", "mebi", "kibi" };
        private static readonly string[] prefixBigSymbols = new string[] { "Ei", "Pi", "Ti", "Gi", "Mi", "Ki" };


		/// <summary>
		/// Converts the value of this instance to its equivalent string representation with measurement unit prefixed with binary IEC prefix symbol.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="measurementUnit">Measurement unit to which prefix will be attached.</param>
		public static string ToBinaryPrefixString(this long value, string measurementUnit) {
			return ConvertToString(value, measurementUnit, null, CultureInfo.CurrentCulture, prefixBigValues , prefixBigSymbols );
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation with measurement unit prefixed with binary IEC prefix symbol.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="measurementUnit">Measurement unit to which prefix will be attached.</param>
		/// <param name="format">A numeric format string for value part.</param>
        public static string ToBinaryPrefixString(this long value, string measurementUnit, string format)
        {
			return ConvertToString(value, measurementUnit, format, CultureInfo.CurrentCulture, prefixBigValues, prefixBigSymbols);
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation with measurement unit prefixed with binary IEC prefix symbol.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="measurementUnit">Measurement unit to which prefix will be attached.</param>
		/// <param name="format">A numeric format string for value part.</param>
		/// <param name="formatProvider">An System.IFormatProvider that supplies culture-specific formatting information for value part.</param>
        public static string ToBinaryPrefixString(this long value, string measurementUnit, string format, IFormatProvider formatProvider)
        {
			return ConvertToString(value, measurementUnit, format, formatProvider, prefixBigValues, prefixBigSymbols);
		}


		/// <summary>
		/// Converts the value of this instance to its equivalent string representation with measurement unit prefixed with binary IEC prefix text.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="measurementUnit">Measurement unit to which prefix will be attached.</param>
        public static string ToLongBinaryPrefixString(this long value, string measurementUnit)
        {
			return ConvertToString(value, measurementUnit, null, CultureInfo.CurrentCulture, prefixBigValues, prefixBigTexts);
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation with measurement unit prefixed with binary IEC prefix text.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="measurementUnit">Measurement unit to which prefix will be attached.</param>
		/// <param name="format">A numeric format string for value part.</param>
        public static string ToLongBinaryPrefixString(this long value, string measurementUnit, string format)
        {
			return ConvertToString(value, measurementUnit, format, CultureInfo.CurrentCulture, prefixBigValues, prefixBigTexts);
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation with measurement unit prefixed with binary IEC prefix text.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="measurementUnit">Measurement unit to which prefix will be attached.</param>
		/// <param name="format">A numeric format string for value part.</param>
		/// <param name="formatProvider">An System.IFormatProvider that supplies culture-specific formatting information for value part.</param>
        public static string ToLongBinaryPrefixString(this long value, string measurementUnit, string format, IFormatProvider formatProvider)
        {
			return ConvertToString(value, measurementUnit, format, formatProvider, prefixBigValues, prefixBigTexts);
		}




        private static string ConvertToString(long value, string measurementUnit, string format, IFormatProvider formatProvider, double[] bigValues, string[] bigStrings)
        {
			for (int i = 0; i < bigValues.Length; ++i) {
				double prefixValue = bigValues[i];
				if (value >= prefixValue) {
					return (value / prefixValue).ToString(format, formatProvider) + " " + bigStrings[i] + measurementUnit;
				}
			}

			return value.ToString(format, formatProvider) + " " + measurementUnit;
		}

    }

}
