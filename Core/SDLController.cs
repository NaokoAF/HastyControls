using HastyControls.SDL3;
using System.Numerics;

namespace HastyControls.Core;

public unsafe class SDLController
{
	public SDL_JoystickID Id { get; }
	public SDL_Gamepad* Gamepad { get; }
	public SDL_Joystick* Joystick { get; }

	public string? Name { get; }
	public bool HasGyro { get; }

	// state
	public uint Buttons;
	public Vector2 LeftStick;
	public Vector2 RightStick;
	public float LeftTrigger;
	public float RightTrigger;
	public Vector3 Gyroscope;
	public Vector3 Accelerometer;
	public ulong GyroscopeTimestamp;
	public ulong AccelerometerTimestamp;

	SDL sdl;

	public SDLController(SDL sdl, SDL_JoystickID id, SDL_Gamepad* gamepad)
	{
		this.sdl = sdl;
		Id = id;
		Gamepad = gamepad;

		Name = sdl.PtrToStringUTF8(sdl.GetGamepadName(Gamepad));
		HasGyro = sdl.GamepadHasSensor(Gamepad, SDL_SensorType.SDL_SENSOR_GYRO);
		Joystick = sdl.GetGamepadJoystick(gamepad);
	}

	public void SetGyroEnabled(bool enabled)
	{
		sdl.SetGamepadSensorEnabled(Gamepad, SDL_SensorType.SDL_SENSOR_GYRO, enabled);
		sdl.SetGamepadSensorEnabled(Gamepad, SDL_SensorType.SDL_SENSOR_ACCEL, enabled);
	}

	// user friendly values. frequency ranges between 0 and 1, and duration in seconds
	public void Rumble(float lowFrequency, float highFrequency, float duration)
	{
		sdl.RumbleGamepad(Gamepad,
			(ushort)(Math.Clamp(lowFrequency, 0f, 1f) * 65535f),
			(ushort)(Math.Clamp(highFrequency, 0f, 1f) * 65535f),
			(uint)(Math.Clamp(duration, 0f, 4294967.295f) * 1000f)
		);
	}

	public void SetButton(int button, bool down)
	{
		if (button < 0) return;

		// bit fuckery to assign a bit (technically unsafe but we're the only callers of this function)
		// 1. reinterpret bool as a byte, meaning it will be 1 when true and 0 when false
		// 2. clear the bit so we can override it
		// 3. or with the new bit
		uint downBit = *(byte*)&down;
		Buttons = Buttons & ~(1u << button) | (downBit << button);
	}

	public bool GetButton(int button)
	{
		if (button < 0) return false;

		// bit fuckery to reinterpret the bit as a boolean (should be safe)
		byte flag = (byte)(Buttons >> button & 1);
		return *(bool*)&flag;
	}
}
