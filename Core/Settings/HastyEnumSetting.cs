using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyEnumSetting<T> : EnumSetting<T>, IEnumSetting, IExposedSetting where T : unmanaged, Enum
{
	public event Action<T>? Applied;

	string category;
	string name;
	T defaultValue;
	LocalizedString displayName;
	List<string> choices;

	public HastyEnumSetting(string category, string name, T defaultValue, IEnumerable<string>? choices = null)
	{
		this.category = category;
		this.name = name;
		this.defaultValue = defaultValue;
		displayName = new(ModInfo.Guid, name);
		this.choices = choices != null ? new(choices) : new(Enum.GetNames(typeof(T)));
	}

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	protected override T GetDefaultValue() => defaultValue;
	public override List<LocalizedString> GetLocalizedChoices() => null!;
	List<string> IEnumSetting.GetUnlocalizedChoices() => choices;
	public override void ApplyValue() => Applied?.Invoke(Value);
}