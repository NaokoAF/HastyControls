using Landfall.Haste;
using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyBoolSetting : BoolSetting, IHastySetting
{
	public event Action<bool>? Applied;
	public IHastySetting? Parent { get; set; }
	public override LocalizedString OffString { get; }
	public override LocalizedString OnString { get; }

	private readonly string category;
	private readonly bool defaultValue;
	private readonly LocalizedString displayName;

	public HastyBoolSetting(
		string category,
		string name,
		string description,
		bool defaultValue
	)
	{
		this.category = category;
		this.defaultValue = defaultValue;
		displayName = HastySettings.CreateDisplayName(name, description);
		OffString = new UnlocalizedString("Off");
		OnString = new UnlocalizedString("On");
	}

	protected override bool GetDefaultValue() => defaultValue;

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	public override void ApplyValue() => Applied?.Invoke(Value);
	public bool CanShow() => Parent?.CanShowChildren() ?? true;
	public void Reset() => Value = defaultValue;
}
