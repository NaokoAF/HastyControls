using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyBoolSetting : BoolSetting, IHastySetting, IExposedSetting, IEnumSetting
{
	public event Action<bool>? Applied;
	public Func<bool>? ShowCondition { get; set; }

	string category;
	bool defaultValue;
	LocalizedString displayName;
	List<string> choices;

	public HastyBoolSetting(string category, string name, string description, bool defaultValue, string offChoice = "Off", string onChoice = "On")
	{
		this.category = category;
		this.defaultValue = defaultValue;
		displayName = HastySettings.CreateDisplayName(name, description);
		choices = [offChoice, onChoice];
	}

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	protected override bool GetDefaultValue() => defaultValue;
	public override LocalizedString OffString => null!;
	public override LocalizedString OnString => null!;
	List<string> IEnumSetting.GetUnlocalizedChoices() => choices;
	public override void ApplyValue() => Applied?.Invoke(Value);
	public void Reset() => Value = defaultValue;
}
