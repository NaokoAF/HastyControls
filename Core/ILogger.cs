namespace HastyControls.Core;

public interface ILogger
{
	public void Msg(string message);
	public void Warning(string message);
	public void Error(string message);
}
