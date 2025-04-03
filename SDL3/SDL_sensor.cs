using System.Runtime.InteropServices;

namespace HastyControls.SDL3;

public enum SDL_SensorID : uint;

public partial struct SDL_Sensor
{
}

public enum SDL_SensorType
{
	SDL_SENSOR_INVALID = -1,
	SDL_SENSOR_UNKNOWN,
	SDL_SENSOR_ACCEL,
	SDL_SENSOR_GYRO,
	SDL_SENSOR_ACCEL_L,
	SDL_SENSOR_GYRO_L,
	SDL_SENSOR_ACCEL_R,
	SDL_SENSOR_GYRO_R,
}

public unsafe partial class SDL
{
	public delegate SDL_SensorID* SDL_GetSensors(int* count);
	public delegate byte* SDL_GetSensorNameForID(SDL_SensorID instance_id);
	public delegate SDL_Sensor* SDL_OpenSensor(SDL_SensorID instance_id);
	public delegate SDL_Sensor* SDL_GetSensorFromID(SDL_SensorID instance_id);
	public delegate byte* SDL_GetSensorName(SDL_Sensor* sensor);
	public delegate SDL_SensorType SDL_GetSensorType(SDL_Sensor* sensor);
	public delegate void SDL_CloseSensor(SDL_Sensor* sensor);
	public delegate void SDL_UpdateSensors();
	//public delegate SDL_PropertiesID SDL_GetSensorProperties(SDL_Sensor* sensor);

	public SDL_GetSensors GetSensors;
	public SDL_GetSensorNameForID GetSensorNameForID;
	public SDL_OpenSensor OpenSensor;
	public SDL_GetSensorFromID GetSensorFromID;
	public SDL_GetSensorName GetSensorName;
	public SDL_GetSensorType GetSensorType;
	public SDL_CloseSensor CloseSensor;
	public SDL_UpdateSensors UpdateSensors;
	//public SDL_GetSensorProperties GetSensorProperties;

}