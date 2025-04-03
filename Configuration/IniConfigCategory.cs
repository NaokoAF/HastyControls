using IniParser;
using IniParser.Model;

namespace HastyControls.Configuration;

public class IniConfigCategory
{
	IniData ini;
	string sectionName;

	public IniConfigCategory(IniData ini, string sectionName, string? description)
	{
		this.ini = ini;
		this.sectionName = sectionName;

		Section section = GetOrCreateSection();
		section.Comments.Clear();

		if (description != null)
			section.Comments.Add(description);
	}

	Section GetOrCreateSection()
	{
		Section? section = ini.Sections.FindByName(sectionName);
		if (section == null)
		{
			section = new(sectionName);
			ini.Sections.Add(section);
		}
		return section;
	}

	Property GetOrCreateProperty(string name)
	{
		Section section = GetOrCreateSection();
		Property? property = section.Properties.FindByKey(name);
		if (property == null)
		{
			property = new(name, null); // null value so it gets replaced
			section.Properties.Add(property);
		}
		return property;
	}

	public IniConfigEntryString CreateEntry(string name, string defaultValue, string? description = null)
	{
		return new IniConfigEntryString(GetOrCreateProperty(name), defaultValue, description);
	}

	public IniConfigEntryEnum<TEnum> CreateEntryEnum<TEnum>(string name, TEnum defaultValue, string? description = null) where TEnum : struct
	{
		return new IniConfigEntryEnum<TEnum>(GetOrCreateProperty(name), defaultValue, description);
	}

	public IniConfigEntryNumber CreateEntry(string name, double defaultValue, string? description = null)
	{
		return new IniConfigEntryNumber(GetOrCreateProperty(name), defaultValue, description);
	}

	public IniConfigEntryNumber CreateEntry(string name, float defaultValue, string? description = null)
	{
		return new IniConfigEntryNumber(GetOrCreateProperty(name), defaultValue, description);
	}

	public IniConfigEntryBoolean CreateEntry(string name, bool defaultValue, string? description = null)
	{
		return new IniConfigEntryBoolean(GetOrCreateProperty(name), defaultValue, description);
	}
}
