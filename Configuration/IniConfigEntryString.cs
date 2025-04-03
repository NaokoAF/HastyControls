using IniParser.Model;

namespace HastyControls.Configuration;

public class IniConfigEntryString : IniConfigEntry
{
	public string Value
	{
		get => property.Value;
		set => property.Value = value;
	}

	public IniConfigEntryString(Property property, string defaultValue, string? description) : base(property, "String", defaultValue, description)
	{
	}
}