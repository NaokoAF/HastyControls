using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyButtonSetting : ButtonSetting, IHastySetting
{
	public IHastySetting? Parent { get; set; }

	private readonly string category;
	private readonly LocalizedString displayName;
	private readonly string buttonText;
	private readonly Action callback;

	public HastyButtonSetting(string category, string name, string description, string buttonText, Action callback)
	{
		this.category = category;
		this.buttonText = buttonText;
		this.callback = callback;
		displayName = HastySettings.CreateDisplayName(name, description);
	}

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	public override string GetButtonText() => buttonText;
	public override void OnClicked(ISettingHandler settingHandler) => callback();
	public bool CanShow() => Parent?.CanShowChildren() ?? true;

	public void Reset()
	{
	}
}