using IniParser;

namespace HastyControls.Configuration;

public class IniConfigFile
{
	public IniData IniData => ini;

	IniData ini;

	public IniConfigFile(IniData ini)
	{
		this.ini = ini;
	}

	public IniConfigCategory CreateCategory(string name, string? description = null)
	{
		return new IniConfigCategory(ini, name, description);
	}
}
