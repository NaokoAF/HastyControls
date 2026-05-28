using System.Numerics;
using Steamworks;

namespace HastyControls.Core;

public record struct SteamInputState(InputHandle_t Handle, Vector3 Gyro, Vector3 Accel);