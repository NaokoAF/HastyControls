using Unity.Mathematics;
using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyFloatSetting : FloatSetting, IExposedSetting
{
	public event Action<float>? Applied;

	string category;
	string name;
	float defaultValue;
	float2 minMax;
	LocalizedString displayName;

	public HastyFloatSetting(string category, string name, float min, float max, float defaultValue)
	{
		this.category = category;
		this.name = name;
		this.defaultValue = defaultValue;
		this.minMax = new float2(min, max);
		displayName = new(ModInfo.Guid, name);
	}

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	protected override float GetDefaultValue() => defaultValue;
	protected override float2 GetMinMaxValue() => minMax;
	public override void ApplyValue() => Applied?.Invoke(Value);
}
