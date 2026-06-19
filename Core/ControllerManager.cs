using System.Diagnostics;
using HastyControls.SDL3;
using System.Numerics;
using Steamworks;

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

	private static readonly Dictionary<(ushort, ushort), GyroOrientation> DefaultOrientationMap = new()
	{
		{ (0x28de, 0x1205), GyroOrientation.Deck }, // Steam Deck
		{ (0x0b05, 0x1abe), GyroOrientation.Ally }, // ROG Ally
		{ (0x0b05, 0x1b4c), GyroOrientation.Ally }, // ROG Ally X
	};

	public ControllerManager(SDLManager sdl, SteamInputManager steamInput)
	{
		this.sdl = sdl;
		this.steamInput = steamInput;
		sdl.ControllerAdded += Sdl_ControllerAdded;
		sdl.ControllerRemoved += Sdl_ControllerRemoved;
		sdl.ControllerSensorUpdated += Sdl_ControllerSensorUpdated;
		sdl.ControllerButtonUpdated += Sdl_ControllerButtonUpdated;
		sdl.ControllerAxisUpdated += Sdl_ControllerAxisUpdated;
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
		foreach ((SDLController controller, GyroState gyro) in gyroStates)
		{
			// add steam input gyro
			if (steamInput.TryGetState(controller, out var steamGyro, out var steamAccel))
			{
				gyro.GyroInput.AddGyroSample(steamGyro, steamInput.Timestamp);
				gyro.GyroInput.AddAccelerometerSample(steamAccel, steamInput.Timestamp);
			}
			else
			{
				if (GyroCalibrateButtonDown)
					gyro.BiasCalibrationTime = 0.1f;
			}

			gyroDelta += gyro.Update(gyroActive, deltaTime);
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
		bool isSteamInput = controller.SteamHandle != 0;
		if (controller.HasGyro || isSteamInput)
		{
			GyroState gyro = new();

			// calibrate once added. ignore steam input, as it handles calibration itself
			if (!isSteamInput)
			{
				gyro.BiasCalibrationTime = 1f;
				gyro.BiasCalibrated += bias => GyroBiasCalibrated?.Invoke(controller, bias);
			}

			if (DefaultOrientationMap.TryGetValue((controller.VendorId, controller.ProductId), out var orientation))
			{
				gyro.DefaultOrientation = orientation;
			}

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