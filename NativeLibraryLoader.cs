using System.Runtime.InteropServices;

namespace HastyControls;

public static class NativeLibraryLoader
{
	private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

	public static nint Open(string path)
	{
		nint pointer;
		if (IsWindows) pointer = LoadLibrary(path);
		else pointer = dlopen(path, RTLD_LAZY);

		return pointer != 0 ? pointer : throw new Exception($"Failed to load library: {GetError()}");
	}

	public static nint GetSymbol(nint handle, string symbol)
	{
		nint pointer;
		if (IsWindows) pointer = GetProcAddress(handle, symbol);
		else pointer = dlsym(handle, symbol);
		
		return pointer != 0 ? pointer : throw new Exception($"Failed to get symbol: {GetError()}");
	}

	public static void Close(nint handle)
	{
		if (IsWindows) FreeLibrary(handle);
		else dlclose(handle);
	}

	private static string GetError()
	{
		if (IsWindows) return Marshal.GetLastWin32Error().ToString();
		else return dlerror();
	}

	private const int RTLD_LAZY = 0x00001;
	private const int RTLD_NOW = 0x00002;
	private const int RTLD_BINDING_MASK = 0x00003;
	private const int RTLD_NOLOAD = 0x00004;
	private const int RTLD_DEEPBIND = 0x00008;
	private const int RTLD_GLOBAL = 0x00100;
	private const int RTLD_LOCAL = 0x00000;
	private const int RTLD_NODELETE = 0x01000;
	
	[DllImport("kernel32.dll", EntryPoint = "LoadLibrary", SetLastError = true)]
	private static extern nint LoadLibrary(string lib);

	[DllImport("kernel32.dll", EntryPoint = "FreeLibrary", SetLastError = true)]
	private static extern void FreeLibrary(nint module);

	[DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = true)]
	private static extern nint GetProcAddress(nint module, string proc);

	[DllImport("libdl.so.2", EntryPoint = "dlopen")]
	private static extern nint dlopen(string file, int flags);

	[DllImport("libdl.so.2", EntryPoint = "dlsym")]
	private static extern nint dlsym(nint handle, string symbol);

	[DllImport("libdl.so.2", EntryPoint = "dlclose")]
	private static extern int dlclose(nint handle);

	[DllImport("libdl.so.2", EntryPoint = "dlerror")]
	private static extern string dlerror();
}