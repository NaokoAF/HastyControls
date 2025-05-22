using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyButtonSetting : ButtonSetting, IHastySetting, IExposedSetting
{
	public Func<bool>? ShowCondition { get; set; }

	string category;
	LocalizedString displayName;
	string buttonText;
	Action callback;

	public HastyButtonSetting(string category, string name, string description, string buttonText, Action callback)
	{
		this.category = category;
		this.buttonText = buttonText;
		this.callback = callback;
		displayName = HastySettings.CreateDisplayName(name, description);
	}

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;

	public override void OnClicked(ISettingHandler settingHandler) => callback();
	public override string GetButtonText() => buttonText;
	public void Reset() { }
}
