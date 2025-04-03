using IniParser.Model;
using System.Globalization;

namespace HastyControls.Configuration;

public class IniConfigEntryBoolean : IniConfigEntry
{
	public bool Value
	{
		get => FromString(property.Value, defaultValue);
		set => property.Value = ToString(value);
	}

	bool defaultValue;

	public IniConfigEntryBoolean(Property property, bool defaultValue, string? description) : base(property, "Boolean", ToString(defaultValue), description)
	{
		this.defaultValue = defaultValue;
	}

	static string ToString(bool value) => value.ToString(CultureInfo.InvariantCulture);

	static bool FromString(string value, bool defaultValue)
	{
		if (bool.TryParse(value, out bool result))
			return result;
		return defaultValue;
	}
}
