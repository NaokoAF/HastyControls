using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyEnumSetting<T> : EnumSetting<T>, IExposedSetting where T : unmanaged, Enum
{
	string category;
	string name;
	T defaultValue;
	LocalizedString displayName;

	public HastyEnumSetting(string category, string name, T defaultValue)
	{
		this.category = category;
		this.name = name;
		this.defaultValue = defaultValue;
		displayName = new(ModInfo.Guid, name);
	}

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	protected override T GetDefaultValue() => defaultValue;
	public override List<LocalizedString> GetLocalizedChoices() => null!;
	public override void ApplyValue() { }
}