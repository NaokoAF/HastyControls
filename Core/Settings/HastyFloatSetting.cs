using Unity.Mathematics;
using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyFloatSetting : FloatSetting, IHastySetting, IExposedSetting
{
	public event Action<float>? Applied;
	public Func<bool>? ShowCondition { get; set; }

	string category;
	float defaultValue;
	float2 minMax;
	LocalizedString displayName;

	public HastyFloatSetting(string category, string name, string description, float min, float max, float defaultValue)
	{
		this.category = category;
		this.defaultValue = defaultValue;
		minMax = new float2(min, max);
		displayName = HastySettings.CreateDisplayName(name, description);
	}

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	protected override float GetDefaultValue() => defaultValue;
	protected override float2 GetMinMaxValue() => minMax;
	public override void ApplyValue() => Applied?.Invoke(Value);
	public void Reset() => Value = defaultValue;
}
