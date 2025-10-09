using UnityEngine;
using UnityEngine.SceneManagement;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core;

public class GyroPauser
{
	public bool IsGyroPaused => gyroPaused;

	float gyroPauseTime;
	bool gyroPauseOnce;
	bool gyroPaused;

	public GyroPauser(HasteEvents events)
	{
		// pause gyro at the start of a level to prevent looking at the floor on accident
		SceneManager.activeSceneChanged += (_, _) => AddPauseTime();
		GM_API.NewLevel += AddPauseTime;
		GM_API.LevelRestart += AddPauseTime;
	}

	public void Update()
	{
		gyroPaused = true; // gyro is assumed paused by default. must pass all checks to be unpaused

		// pause the very first frame as it can have a very large delta time due to stutter
		if (gyroPauseOnce)
		{
			gyroPauseOnce = false;
			return;
		}

		// pause until timer reaches 0
		if (gyroPauseTime > 0f)
		{
			gyroPauseTime -= Time.unscaledDeltaTime;
			return;
		}
		gyroPauseTime = 0f;

		// pause if gyro is disabled or the window is unfocused
		if (GetSetting<GyroSensitivitySetting>().Value == 0f || !Application.isFocused)
			return;

		// if GyroDisableWhenWalking is enabled, disable gyro when SlowMovement is active, but re-enable when charging up fast run
		var player = PlayerCharacter.localPlayer;
		if (GetSetting<GyroDisableWhenWalkingSetting>().Value && player != null && player.refs.slowMovement.enabled && !player.data.enteringFastRun)
			return;

		gyroPaused = false;
	}

	void AddPauseTime()
	{
		gyroPauseOnce = true;
		gyroPauseTime = 0.1f;
	}
}