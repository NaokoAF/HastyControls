using HastyControls.SDL3;
using System.Numerics;
using static HastyControls.Core.Settings.HastySettings;

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
	Vector2 gyroDelta;
	bool gyroButtonState = true;
	Dictionary<SDLController, GyroState> gyroStates = new();

	public ControllerManager(SDLManager sdl)
	{
		this.sdl = sdl;
		sdl.ControllerAdded += Sdl_ControllerAdded;
		sdl.ControllerRemoved += Sdl_ControllerRemoved;
		sdl.ControllerButtonUpdated += Sdl_ControllerButtonUpdated;
		sdl.ControllerSensorUpdated += Sdl_ControllerSensorUpdated;
	}

	public void PrePoll()
	{
		foreach (var gyro in gyroStates.Values)
		{
			gyro.GyroInput.Begin();
		}
	}

	public void Update(float deltaTime)
	{
		int gyroButton = (int)GetSetting<GyroButtonSetting>().Value;
		int gyroCalibrateButton = (int)GetSetting<GyroCalibrateButtonSetting>().Value;

		// add up gyro on all controllers
		gyroDelta = Vector2.Zero;
		if (activeController != null && activeControllerGyro != null)
		{
			switch (GetSetting<GyroButtonModeSetting>().Value)
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
				activeControllerGyro.Reset();
			}
		}
	}

	private void Sdl_ControllerAdded(SDLController controller)
	{
		if (controller.HasGyro)
		{
			GyroState gyro = new();
			gyro.BiasCalibrationTime = 1f; // calibrate once added
			gyro.BiasCalibrated += bias => GyroBiasCalibrated?.Invoke(controller, bias);
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
			gyro.Reset();
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

	private void Sdl_ControllerButtonUpdated(SDLController controller, ControllerButton button, bool down)
	{
		SetActiveController(controller);

		// toggle gyro
		var gyroButton = GetSetting<GyroButtonSetting>().Value;
		var gyroButtonMode = GetSetting<GyroButtonModeSetting>().Value;
		if (down && button == gyroButton && gyroButtonMode == GyroButtonMode.Toggle)
		{
			gyroButtonState = !gyroButtonState;
		}
	}

	private void Sdl_ControllerSensorUpdated(SDLController controller, SDL_SensorType sensor, Vector3 data, ulong timestamp)
	{
		if (GetSetting<GyroButtonSetting>().Value == 0) return; // skip for performance
		if (activeController != controller || activeControllerGyro == null) return;

		switch (sensor)
		{
			case SDL_SensorType.SDL_SENSOR_GYRO:
				activeControllerGyro.GyroInput.InputGyro(data, timestamp);
				break;
			case SDL_SensorType.SDL_SENSOR_ACCEL:
				activeControllerGyro.GyroInput.InputAccelerometer(data, timestamp);
				break;
		}
	}
}
