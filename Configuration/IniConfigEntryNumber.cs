using IniParser.Model;
using System.Globalization;

namespace HastyControls.Configuration;

public class IniConfigEntryNumber : IniConfigEntry
{
	public double Value
	{
		get => FromString(property.Value, defaultValue);
		set => property.Value = ToString(value);
	}

	public float ValueFloat
	{
		get => (float)Value;
		set => Value = (double)value;
	}

	double defaultValue;

	public IniConfigEntryNumber(Property property, double defaultValue, string? description) : base(property, "Number", ToString(defaultValue), description)
	{
		this.defaultValue = defaultValue;
	}

	static string ToString(double value) => value.ToString("G17", CultureInfo.InvariantCulture);

	static double FromString(string value, double defaultValue)
	{
		if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
			return result;
		return defaultValue;
	}
}
