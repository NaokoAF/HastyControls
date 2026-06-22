namespace HastyControls.Core.Settings;

public interface IHastySetting : IExposedSetting, IConditionalSetting
{
	public IHastySetting? Parent { get; set; }

	bool CanShowChildren() => CanShow();
	void Reset();
}