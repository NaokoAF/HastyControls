using IniParser.Model;

namespace HastyControls.Configuration;

public class IniConfigEntryEnum<TEnum> : IniConfigEntry where TEnum : struct
{
	public TEnum Value
	{
		get => FromString(property.Value, defaultValue);
		set => property.Value = ToString(value);
	}

	TEnum defaultValue;

	public IniConfigEntryEnum(Property property, TEnum defaultValue, string? description) : base(property, "Enum", ToString(defaultValue), description, GetAcceptableValues())
	{
		this.defaultValue = defaultValue;
	}

	static string ToString(TEnum value) => Enum.GetName(typeof(TEnum), value);

	static TEnum FromString(string value, TEnum defaultValue)
	{
		if (Enum.TryParse(value, true, out TEnum result))
			return result;
		return defaultValue;
	}

	static string GetAcceptableValues() => string.Join(", ", Enum.GetNames(typeof(TEnum)));
}
