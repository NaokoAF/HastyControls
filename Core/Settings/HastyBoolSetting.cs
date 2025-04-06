﻿using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyBoolSetting : BoolSetting, IExposedSetting, IEnumSetting
{
	public event Action<bool>? Applied;

	string category;
	string name;
	bool defaultValue;
	LocalizedString displayName;
	List<string> choices;

	public HastyBoolSetting(string category, string name, bool defaultValue, string offChoice = "Off", string onChoice = "On")
	{
		this.category = category;
		this.name = name;
		this.defaultValue = defaultValue;
		displayName = new(ModInfo.Guid, name);
		choices = [offChoice, onChoice];
	}

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	protected override bool GetDefaultValue() => defaultValue;
	public override LocalizedString OffString => null!;
	public override LocalizedString OnString => null!;
	List<string> IEnumSetting.GetUnlocalizedChoices() => choices;
	public override void ApplyValue() => Applied?.Invoke(Value);
}
