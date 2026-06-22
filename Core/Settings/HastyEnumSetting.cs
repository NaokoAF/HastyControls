using Landfall.Haste;
using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyEnumSetting<T> : EnumSetting<T>, IHastySetting where T : unmanaged, Enum
{
	public event Action<T>? Applied;
	public IHastySetting? Parent { get; set; }

	private readonly string category;
	private readonly T defaultValue;
	private readonly LocalizedString displayName;
	private readonly List<LocalizedString> choices;

	public HastyEnumSetting(string category, string name, string description, T defaultValue, IEnumerable<string>? choices = null)
	{
		this.category = category;
		this.defaultValue = defaultValue;
		displayName = HastySettings.CreateDisplayName(name, description);
		this.choices = (choices ?? Enum.GetNames(typeof(T))).Select(x => (LocalizedString)new UnlocalizedString(x)).ToList();
	}

	protected override T GetDefaultValue() => defaultValue;

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	public override List<LocalizedString> GetLocalizedChoices() => choices;
	public override void ApplyValue() => Applied?.Invoke(Value);
	public bool CanShow() => Parent?.CanShowChildren() ?? true;
	public void Reset() => Value = defaultValue;
}