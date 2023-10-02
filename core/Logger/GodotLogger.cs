using Godot;
using MathsMurderSpike.core.Logger;

public partial class GodotLogger : Node
{
	private static LoggingLevel _loggingLevel = LoggingLevel.Debug;
	
	public void SetLoggingLevel(LoggingLevel level)
	{
		_loggingLevel = level;
	}

	public static void LogDebug(string message)
	{
		if (_loggingLevel <= LoggingLevel.Debug)
			LogMessage(message, LoggingLevel.Debug);
	}

	public static void LogInfo(string message)
	{
		if (_loggingLevel <= LoggingLevel.Info)
			LogMessage(message, LoggingLevel.Info);
	}

	public static void LogWarning(string message)
	{
		if (_loggingLevel <= LoggingLevel.Warning)
			LogMessage(message, LoggingLevel.Warning);
	}

	public static void LogError(string message)
	{
		if (_loggingLevel <= LoggingLevel.Error)
			LogMessage(message, LoggingLevel.Error);
	}

	private static void LogMessage(string message, LoggingLevel level)
	{
		switch (level)
		{
			case LoggingLevel.Debug:
			case LoggingLevel.Info:
				GD.Print(message);
				break;
			case LoggingLevel.Warning:
				GD.PushWarning(message);
				break;
			case LoggingLevel.Error:
				GD.PushError(message);
				break;
		}
	}
}
