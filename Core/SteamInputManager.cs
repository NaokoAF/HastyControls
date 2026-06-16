using System.Numerics;
using Steamworks;

namespace HastyControls.Core;

public class SteamInputManager : IDisposable
{
	// NOTE: MonoMod is being annoying with Spans, so i'm just returning the full array here 
	public SteamInputState[] Controllers => states;
	public int ControllerCount => stateCount;
	public ulong Timestamp => timestamp;
	
	private readonly InputHandle_t[] handles = new InputHandle_t[MaxControllerCount];
	private readonly SteamInputState[] states = new SteamInputState[MaxControllerCount];
	private int stateCount;
	private ulong timestamp;

	private const float GyroScale = (1f / 32768f) * 2000f * (MathF.PI / 180f); // 2000 degrees/s to radians/s
	private const float AccelScale = (1f / 32768f) * 2.0f * 9.80665f; // 2G to m/s2
	
	public const int MaxControllerCount = Constants.STEAM_INPUT_MAX_COUNT;

	public SteamInputManager()
	{
		SteamInput.Init(true);
	}

	public void Update(float deltaTime)
	{
		SteamInput.RunFrame();
		
		timestamp += (ulong)(deltaTime * 1000000000.0); // nanoseconds

		stateCount = SteamInput.GetConnectedControllers(handles);
		for (int i = 0; i < stateCount; i++)
		{
			InputHandle_t handle = handles[i];
			InputMotionData_t motionData = SteamInput.GetMotionData(handle);
			ESteamInputType type = SteamInput.GetInputTypeForHandle(handle);

			Vector3 gyro = new Vector3(motionData.rotVelX, motionData.rotVelZ, motionData.rotVelY) * GyroScale;
			Vector3 accel = new Vector3(motionData.posAccelX, motionData.posAccelZ, -motionData.posAccelY) * AccelScale;
			SteamInputState state = new(handle, type, gyro, accel);
			states[i] = state;
		}
	}

	public void Dispose()
	{
		SteamInput.Shutdown();
	}
}