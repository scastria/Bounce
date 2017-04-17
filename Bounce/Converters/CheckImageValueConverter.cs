using System;
using System.Globalization;
using Xamarin.Forms;

namespace Bounce.Converters
{
	public class CheckImageValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool bVal = (bool)value;
			return (bVal ? "row_check" : "row_nocheck");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
