using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HastyControls.Core;

public class GyroPauser
{
	Config config;
	float gyroPauseTime;
	bool gyroPauseOnce;

	public GyroPauser(Config config, HasteEvents events)
	{
		this.config = config;

		// pause gyro at the start of a level. very jank
		SceneManager.activeSceneChanged += (_, _) => AddPauseTime();
		GM_API.NewLevel += AddPauseTime;
		GM_API.LevelRestart += AddPauseTime;
	}

	public bool Update()
	{
		// pause the very first frame as it can have a very large delta time due to stutter
		if (gyroPauseOnce)
		{
			gyroPauseOnce = false;
			return true;
		}

		// pause until timer reaches 0
		if (gyroPauseTime > 0f)
		{
			gyroPauseTime -= Time.unscaledDeltaTime;
			return true;
		}
		gyroPauseTime = 0f;

		// pause if gyro is disabled or the window is unfocused
		if (!config.GyroEnabled || !Application.isFocused)
			return true;

		// if GyroDisableWhenWalking is enabled, disable gyro when SlowMovement is active, but re-enable when charging up fast run
		var player = PlayerCharacter.localPlayer;
		if (config.GyroDisableWhenWalking && player != null && player.refs.slowMovement.enabled && !player.data.enteringFastRun)
			return true;

		return false;
	}

	void AddPauseTime()
	{
		gyroPauseOnce = true;
		gyroPauseTime = config.GyroLevelStartPauseLength;
	}
}