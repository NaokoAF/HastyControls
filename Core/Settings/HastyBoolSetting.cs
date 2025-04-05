using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyBoolSetting : BoolSetting, IExposedSetting, IEnumSetting
{
	string category;
	string name;
	bool defaultValue;
	LocalizedString displayName;

	public HastyBoolSetting(string category, string name, bool defaultValue)
	{
		this.category = category;
		this.name = name;
		this.defaultValue = defaultValue;
		displayName = new(ModInfo.Guid, name);
	}

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	protected override bool GetDefaultValue() => defaultValue;
	public override LocalizedString OffString => null!;
	public override LocalizedString OnString => null!;

	List<string> IEnumSetting.GetUnlocalizedChoices()
	{
		return new List<string> { "Off", "On" };
	}

	public override void ApplyValue() { }
}
