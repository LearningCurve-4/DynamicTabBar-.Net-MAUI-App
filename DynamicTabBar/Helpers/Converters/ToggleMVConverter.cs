namespace DynamicTabBar.Helpers.Converters;

public class ToggleMVConverter : IMultiValueConverter
{
	public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values[0] == null || values[1] == null || values.Length != 3) { return null; }

		string on = (string)values[1];
		string off = (string)values[1];

		switch ((string)values[2])
		{
			case "OO":  //Image as On or Off
				on = on.Replace(".", "on.");
				off = off.Replace(".", "off.");
				break;
			default:
				break;
		}
		return (bool)values[0] ? on : off;
	}

	public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		return null;
	}
}
