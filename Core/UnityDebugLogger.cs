using UnityEngine;

namespace HastyControls.Core;

public class UnityDebugLogger : ILogger
{
	const string Prefix = $"[{ModInfo.Name}] ";

	public void Error(string message)
	{
		Debug.LogError(Prefix + message);
	}

	public void Msg(string message)
	{
		Debug.Log(Prefix + message);
	}

	public void Warning(string message)
	{
		Debug.LogWarning(Prefix + message);
	}
}