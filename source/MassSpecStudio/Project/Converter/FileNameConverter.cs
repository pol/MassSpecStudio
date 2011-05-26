using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace MassSpecStudio.Modules.Project.Converter
{
	[ValueConversion(typeof(string), typeof(string))]
	public class FileNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (File.Exists(value.ToString()))
			{
				return Path.GetFileNameWithoutExtension(value.ToString());
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}