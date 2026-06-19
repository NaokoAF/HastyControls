using System.Numerics;
using Steamworks;

namespace HastyControls.Core;

public class SteamInputManager : IDisposable
{
	public ulong Timestamp => timestamp;

	private ulong timestamp;

	private const float GyroScale = 2000f * (MathF.PI / 180f) / 32768f; // 2000 degrees/s to radians/s
	private const float AccelScale = 2.0f * 9.80665f / 32768f; // 2G to m/s2
	
	public SteamInputManager()
	{
		SteamInput.Init(true);
	}

	public void Update(float deltaTime)
	{
		SteamInput.RunFrame();
		
		timestamp += (ulong)(deltaTime * 1000000000.0); // nanoseconds
	}

	public bool TryGetState(SDLController controller, out Vector3 gyro, out Vector3 accel)
	{
		if (controller.SteamHandle == 0)
		{
			gyro = default;
			accel = default;
			return false;
		}
		
		InputHandle_t handle = new(controller.SteamHandle);
		InputMotionData_t motionData = SteamInput.GetMotionData(handle);

		gyro = new Vector3(motionData.rotVelX, motionData.rotVelZ, motionData.rotVelY) * GyroScale;
		accel = new Vector3(motionData.posAccelX, motionData.posAccelZ, -motionData.posAccelY) * AccelScale;
		return true;
	}
	
	public void Dispose()
	{
		SteamInput.Shutdown();
	}
}