using HastyControls.SDL3;
using System.Numerics;

namespace HastyControls.Core;

public unsafe class ControllerManager
{
	public Vector2 GyroDelta => gyroDelta;

	public GyroButtonMode GyroButtonMode { get; set; }
	public bool GyroButtonDown { get; set; }
	public bool GyroCalibrateButtonDown { get; set; }
	public bool GyroPaused { get; set; }

	public event Action<SDLController, Vector3>? GyroBiasCalibrated;

	SDLManager sdl;
	Vector2 gyroDelta;
	bool gyroButtonState = true;
	bool prevGyroButtonDown;
	Dictionary<SDLController, GyroState> gyroStates = new();

	public ControllerManager(SDLManager sdl)
	{
		this.sdl = sdl;
		sdl.ControllerAdded += Sdl_ControllerAdded;
		sdl.ControllerRemoved += Sdl_ControllerRemoved;
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
		switch (GyroButtonMode)
		{
			case GyroButtonMode.Off:
				gyroButtonState = !GyroButtonDown;
				break;
			case GyroButtonMode.On:
				gyroButtonState = GyroButtonDown;
				break;
			case GyroButtonMode.Toggle:
				if (GyroButtonDown && !prevGyroButtonDown)
				{
					gyroButtonState = !gyroButtonState;
				}
				prevGyroButtonDown = GyroButtonDown;
				break;
		}

		// add up gyro on all controllers
		gyroDelta = Vector2.Zero;
		foreach (var gyro in gyroStates.Values)
		{
			if (GyroCalibrateButtonDown)
				gyro.BiasCalibrationTime = 0.1f;

			if (gyroButtonState && !GyroPaused)
			{
				gyroDelta += gyro.Update(deltaTime);
			}
			else
			{
				gyro.Reset();
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
	}

	private void Sdl_ControllerRemoved(SDLController controller)
	{
		gyroStates.Remove(controller);
	}

	private void Sdl_ControllerSensorUpdated(SDLController controller, SDL_SensorType sensor, Vector3 data, ulong timestamp)
	{
		if (!gyroStates.TryGetValue(controller, out var gyro)) return;

		switch (sensor)
		{
			case SDL_SensorType.SDL_SENSOR_GYRO:
				gyro.GyroInput.AddGyroSample(data, timestamp);
				break;
			case SDL_SensorType.SDL_SENSOR_ACCEL:
				gyro.GyroInput.AddAccelerometerSample(data, timestamp);
				break;
		}
	}
}
