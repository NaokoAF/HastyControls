using HastyControls.SDL3;
using System.Numerics;
using UnityEngine.InputSystem.LowLevel;

namespace HastyControls.Core;

public unsafe class ControllerManager
{
	public SDLController? ActiveController => activeController;
	public Vector2 GyroDelta => gyroDelta;
	public bool GyroPaused { get; set; }

	public event Action<SDLController, Vector3>? GyroBiasCalibrated;
	public event Action<SDLController?>? ActiveControllerChanged;

	SDLController? activeController;
	GyroState? activeControllerGyro;
	SDLManager sdl;
	Config config;
	Vector2 gyroDelta;
	bool gyroButtonState = true;
	Dictionary<SDLController, GyroState> gyroStates = new();

	public ControllerManager(SDLManager sdl, Config config)
	{
		this.sdl = sdl;
		this.config = config;
		sdl.ControllerAdded += Sdl_ControllerAdded;
		sdl.ControllerRemoved += Sdl_ControllerRemoved;
		sdl.ControllerButtonUpdated += Sdl_ControllerButtonUpdated;
		sdl.ControllerSensorUpdated += Sdl_ControllerSensorUpdated;
	}

	public void Update(float deltaTime)
	{
		int gyroButton = (int)config.GyroButton;
		int gyroCalibrateButton = (int)config.GyroCalibrateButton;

		// add up gyro on all controllers
		gyroDelta = Vector2.Zero;
		if (activeController != null && activeControllerGyro != null)
		{
			switch (config.GyroButtonMode)
			{
				case GyroButtonMode.Off: gyroButtonState = !activeController.GetButton(gyroButton); break;
				case GyroButtonMode.On: gyroButtonState = activeController.GetButton(gyroButton); break;
			}

			if (activeController.GetButton(gyroCalibrateButton))
				activeControllerGyro.BiasCalibrationTime = 0.1f;

			if (gyroButtonState && !GyroPaused)
			{
				gyroDelta = activeControllerGyro.Update(deltaTime);
			}
			else
			{
				activeControllerGyro.Flush();
			}
		}
	}

	public void UpdateConfig(Config config)
	{
		this.config = config;
		foreach (var gyro in gyroStates.Values)
		{
			gyro.UpdateConfig(config);
		}
	}

	private void Sdl_ControllerAdded(SDLController controller)
	{
		if (controller.HasGyro)
		{
			GyroState gyro = new();
			gyro.BiasCalibrationTime = 1f; // calibrate once added
			gyro.BiasCalibrated += bias => GyroBiasCalibrated?.Invoke(controller, bias);

			gyro.UpdateConfig(config);
			gyroStates.Add(controller, gyro);
		}

		if (activeController == null)
			SetActiveController(controller);
	}

	void SetActiveController(SDLController? controller)
	{
		if (controller == activeController) return;

		activeController = controller;

		if (controller != null && gyroStates.TryGetValue(controller, out var gyro))
		{
			gyro.Flush();
			activeControllerGyro = gyro;
		}
		else
		{
			activeControllerGyro = null;
		}
		ActiveControllerChanged?.Invoke(controller);
	}

	private void Sdl_ControllerRemoved(SDLController controller)
	{
		if (activeController == controller)
			SetActiveController(null);

		gyroStates.Remove(controller);
	}

	private void Sdl_ControllerButtonUpdated(SDLController controller, GamepadButton button, bool down)
	{
		SetActiveController(controller);

		// toggle gyro
		if (down && (ControllerButton)button == config.GyroButton && config.GyroButtonMode == GyroButtonMode.Toggle)
		{
			gyroButtonState = !gyroButtonState;
		}
	}

	private void Sdl_ControllerSensorUpdated(SDLController controller, SDL_SensorType sensor, Vector3 data, ulong timestamp)
	{
		if (!config.GyroEnabled) return; // skip for performance
		if (activeController != controller || activeControllerGyro == null) return;

		activeControllerGyro.Input(controller.Gyroscope, controller.Accelerometer, controller.GyroscopeTimestamp);
	}
}
