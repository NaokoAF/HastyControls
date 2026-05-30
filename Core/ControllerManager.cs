using System.Diagnostics;
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
	public SDLController? ActiveController => activeController;

	public event Action<SDLController, Vector3>? GyroBiasCalibrated;

	private readonly SDLManager sdl;
	private readonly SteamInputManager steamInput;
	private Vector2 gyroDelta;
	private bool gyroButtonState = true;
	private bool prevGyroButtonDown;
	private SDLController? activeController;
	private readonly Dictionary<SDLController, GyroState> gyroStates = new();
	private readonly GyroState[] steamGyroStates = new GyroState[SteamInputManager.MaxControllerCount];

	public ControllerManager(SDLManager sdl, SteamInputManager steamInput)
	{
		this.sdl = sdl;
		this.steamInput = steamInput;
		sdl.ControllerAdded += Sdl_ControllerAdded;
		sdl.ControllerRemoved += Sdl_ControllerRemoved;
		sdl.ControllerSensorUpdated += Sdl_ControllerSensorUpdated;
		sdl.ControllerButtonUpdated += Sdl_ControllerButtonUpdated;
		sdl.ControllerAxisUpdated += Sdl_ControllerAxisUpdated;

		for (int i = 0; i < steamGyroStates.Length; i++)
			steamGyroStates[i] = new();
	}

	public void PrePoll()
	{
		foreach (var gyro in gyroStates.Values)
		{
			gyro.GyroInput.Begin();
		}
		
		foreach (var gyro in steamGyroStates)
		{
			gyro.GyroInput.Begin();
		}
	}

	public void Update(float deltaTime)
	{
		switch (GyroButtonMode)
		{
			case GyroButtonMode.Off:
			case GyroButtonMode.RecenterAndOff:
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
		bool gyroActive = gyroButtonState && !GyroPaused;
		foreach (var gyro in gyroStates.Values)
		{
			if (GyroCalibrateButtonDown)
				gyro.BiasCalibrationTime = 0.1f;

			gyroDelta += gyro.Update(gyroActive, deltaTime);
		}

		// process and add up steam input gyro
		for (int i = 0; i < steamInput.ControllerCount; i++)
		{
			SteamInputState controller = steamInput.Controllers[i];
			GyroState gyroState = steamGyroStates[i];
			gyroState.GyroInput.AddGyroSample(controller.Gyro, steamInput.Timestamp);
			gyroState.GyroInput.AddAccelerometerSample(controller.Accel, steamInput.Timestamp);
			
			gyroDelta += gyroState.Update(gyroActive, deltaTime);
		}
	}

	public void ForceEnableGyro()
	{
		foreach (var controller in gyroStates.Keys)
		{
			// SDL caches the previous sensor enabled value
			// set to false first to force it to be enabled
			controller.SetGyroEnabled(false);
			controller.SetGyroEnabled(true);
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
			activeController = controller;
	}

	private void Sdl_ControllerRemoved(SDLController controller)
	{
		gyroStates.Remove(controller);

		if (activeController == controller)
			activeController = null;
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

	private void Sdl_ControllerButtonUpdated(SDLController controller, ControllerButton button, bool down)
	{
		activeController = controller;
	}

	private void Sdl_ControllerAxisUpdated(SDLController controller, ControllerAxis axis, float value)
	{
		if (MathF.Abs(value) > 0.2f)
		{
			activeController = controller;
		}
	}
}
