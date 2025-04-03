using IniParser.Model;

namespace HastyControls.Configuration;

public abstract class IniConfigEntry
{
	protected Property property;

	public IniConfigEntry(Property property, string type, string defaultValue, string? description = null, string? acceptableValues = null)
	{
		this.property = property;
		if (property.Value == null)
			property.Value = defaultValue;

		property.Comments.Clear();
		if (description != null)
			AddComment(description);

		AddComment($"Setting type: {type}");

		if (defaultValue != null)
			AddComment($"Default value: {defaultValue}");

		if (acceptableValues != null)
			AddComment($"Acceptable values: {acceptableValues}");
	}

	void AddComment(string comment)
	{
		if (comment.Contains('\n'))
			property.Comments.AddRange(comment.Split('\n'));
		else
			property.Comments.Add(comment);
	}
}
