using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyCollapsibleSetting : ButtonSetting, IHastySetting
{
	public event Action<bool>? Clicked;
	public IHastySetting? Parent { get; set; }

	public bool Collapsed { get; private set; } = true;

	private readonly string category;
	private readonly LocalizedString displayName;

	public HastyCollapsibleSetting(string category, string name, string description)
	{
		this.category = category;
		displayName = HastySettings.CreateDisplayName(name, description);
	}

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	public override string GetButtonText() => null!;
	public bool CanShow() => Parent?.CanShowChildren() ?? true;
	public bool CanShowChildren() => !Collapsed && CanShow();

	public override void OnClicked(ISettingHandler settingHandler)
	{
		Collapsed = !Collapsed;
		Clicked?.Invoke(Collapsed);
	}

	public void Reset()
	{
	}
}