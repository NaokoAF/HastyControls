using HastyControls.SDL3;
using System.Numerics;
using UnityEngine.InputSystem.EnhancedTouch;

namespace HastyControls.Core;

public unsafe class SDLManager
{
	public SDLVersion Version => version;
	public string? Revision => revision;
	public string? CurrentError => sdl.PtrToStringUTF8(sdl.GetError());

	public event Action<SDLController>? ControllerAdded;
	public event Action<SDLController>? ControllerRemoved;
	public event Action<SDLController, ControllerButton, bool>? ControllerButtonUpdated;
	public event Action<SDLController, SDL_SensorType, Vector3, ulong>? ControllerSensorUpdated;
	public event Action<SDLController, int, int, SDLTouchpadFinger>? ControllerTouchpadUpdated;

	SDL sdl;
	Dictionary<SDL_JoystickID, SDLController> controllers = new();
	SDLVersion version;
	string? revision;

	public SDLManager(SDL sdl)
	{
		this.sdl = sdl;
		version = new SDLVersion(sdl.GetVersion());
		revision = sdl.PtrToStringUTF8(sdl.GetRevision());
	}

	public bool Init()
	{
		if (!sdl.Init(SDL_InitFlags.SDL_INIT_JOYSTICK | SDL_InitFlags.SDL_INIT_GAMEPAD))
			return false;

		sdl.SetGamepadEventsEnabled(true); // enable controller events
		return true;
	}

	public void Quit()
	{
		sdl.Quit();
	}

	public void Poll()
	{
		// must update joysticks for rumble to work (since we don't listen to joystick events)
		sdl.UpdateJoysticks();

		SDL_Event evnt;
		while (sdl.PollEvent(&evnt))
		{
			switch ((SDL_EventType)evnt.type)
			{
				case SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
					OnControllerAdded(evnt.gdevice);
					break;
				case SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
					OnControllerRemoved(evnt.gdevice);
					break;
				case SDL_EventType.SDL_EVENT_GAMEPAD_SENSOR_UPDATE:
					OnControllerSensorUpdate(evnt.gsensor);
					break;
				case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN:
				case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP:
					OnControllerButtonUpdate(evnt.gbutton);
					break;
				case SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
					OnControllerAxisUpdate(evnt.gaxis);
					break;
				case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_DOWN:
				case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_UP:
				case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_MOTION:
					OnControllerTouchpadUpdate(evnt.gtouchpad);
					break;
			}
		}
	}

	void OnControllerAdded(SDL_GamepadDeviceEvent evnt)
	{
		// open and create controller
		SDL_JoystickID id = evnt.which;
		SDL_Gamepad* gamepad = sdl.OpenGamepad(id);
		if (gamepad == null) return; // error

		SDLController controller = new(sdl, id, gamepad);
		if (controller.HasGyro)
			controller.SetGyroEnabled(controller.HasGyro);

		// add to dictionary and raise event
		controllers.Add(id, controller);
		ControllerAdded?.Invoke(controller);
	}

	void OnControllerRemoved(SDL_GamepadDeviceEvent evnt)
	{
		if (!controllers.TryGetValue(evnt.which, out var controller)) return;

		sdl.CloseGamepad(controller.Gamepad); // likely unecessary

		controllers.Remove(evnt.which);
		ControllerRemoved?.Invoke(controller);
	}

	void OnControllerButtonUpdate(SDL_GamepadButtonEvent evnt)
	{
		if (!controllers.TryGetValue(evnt.which, out var controller)) return;
		controller.SetButton(evnt.button, evnt.down);

		ControllerButtonUpdated?.Invoke(controller, (ControllerButton)evnt.button, evnt.down);
	}

	void OnControllerAxisUpdate(SDL_GamepadAxisEvent evnt)
	{
		if (!controllers.TryGetValue(evnt.which, out var controller)) return;

		// properly remap from (-32768, 32767) to (-1, 1)
		float valueF = (evnt.value + 32768f) / 65535f;
		switch ((ControllerAxis)evnt.axis)
		{
			case ControllerAxis.LeftStickX: controller.LeftStick.X = valueF; break;
			case ControllerAxis.LeftStickY: controller.LeftStick.Y = valueF; break;
			case ControllerAxis.RightStickX: controller.RightStick.X = valueF; break;
			case ControllerAxis.RightStickY: controller.RightStick.Y = valueF; break;
			case ControllerAxis.LeftTrigger: controller.LeftTrigger = valueF; break;
			case ControllerAxis.RightTrigger: controller.RightTrigger = valueF; break;
		}
	}

	void OnControllerSensorUpdate(SDL_GamepadSensorEvent evnt)
	{
		if (!controllers.TryGetValue(evnt.which, out var controller)) return;

		SDL_SensorType type = (SDL_SensorType)evnt.sensor;
		ulong timestamp = evnt.sensor_timestamp; // timestamp in nanoseconds
		Vector3 data = *(Vector3*)evnt.data; // convert 3 float pointer to a Vector3
		switch (type)
		{
			case SDL_SensorType.SDL_SENSOR_GYRO:
				controller.Gyroscope = data;
				controller.GyroscopeTimestamp = timestamp;
				break;
			case SDL_SensorType.SDL_SENSOR_ACCEL:
				controller.Accelerometer = data;
				controller.AccelerometerTimestamp = timestamp;
				break;
		}

		ControllerSensorUpdated?.Invoke(controller, type, data, timestamp);
	}

	void OnControllerTouchpadUpdate(SDL_GamepadTouchpadEvent evnt)
	{
		if (!controllers.TryGetValue(evnt.which, out var controller)) return;

		SDLTouchpadFinger finger = new()
		{
			Down = evnt.type != SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_UP,
			Position = new(evnt.x, evnt.y),
			Pressure = evnt.pressure,
			Timestamp = evnt.timestamp,
		};

		// update state
		controller.Touchpads[evnt.touchpad][evnt.finger] = finger;

		ControllerTouchpadUpdated?.Invoke(controller, evnt.touchpad, evnt.finger, finger);
	}
}
