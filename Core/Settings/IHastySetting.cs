namespace HastyControls.Core.Settings;

public interface IHastySetting : IExposedSetting
{
	public Func<bool>? ShowCondition { get; set; }
}