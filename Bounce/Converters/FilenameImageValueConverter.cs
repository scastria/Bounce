﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace Bounce.Converters
{
	public class FilenameImageValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string filename = (value == null) ? "soccer" : value.ToString();
			return (filename);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
