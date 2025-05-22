using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyCollapsibleSetting : ButtonSetting, IHastySetting, IExposedSetting
{
	public event Action<bool>? Clicked;
	public Func<bool>? ShowCondition { get; set; }

	public bool Collapsed { get; private set; } = true;

	string category;
	LocalizedString displayName;

	public HastyCollapsibleSetting(string category, string name, string description)
	{
		this.category = category;
		displayName = HastySettings.CreateDisplayName(name, description);
	}

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	public override string GetButtonText() => null!;
	public void Reset() { }

	public override void OnClicked(ISettingHandler settingHandler)
	{
		Collapsed = !Collapsed;
		Clicked?.Invoke(Collapsed);
	}
}
