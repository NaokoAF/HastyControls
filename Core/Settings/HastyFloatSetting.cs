using Unity.Mathematics;
using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public abstract class HastyFloatSetting : FloatSetting, IHastySetting
{
	public event Action<float>? Applied;
	public IHastySetting? Parent { get; set; }

	private readonly string category;
	private readonly float defaultValue;
	private readonly float2 minMax;
	private readonly LocalizedString displayName;

	public HastyFloatSetting(string category, string name, string description, float min, float max, float defaultValue)
	{
		this.category = category;
		this.defaultValue = defaultValue;
		minMax = new float2(min, max);
		displayName = HastySettings.CreateDisplayName(name, description);
	}

	protected override float GetDefaultValue() => defaultValue;
	protected override float2 GetMinMaxValue() => minMax;

	public string GetCategory() => category;
	public LocalizedString GetDisplayName() => displayName;
	public override void ApplyValue() => Applied?.Invoke(Value);
	public bool CanShow() => Parent?.CanShowChildren() ?? true;
	public void Reset() => Value = defaultValue;
}