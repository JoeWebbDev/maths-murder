using Godot;

public partial class GameDataManager : Node
{
	[Export] private PlayerData _playerDataResource;
	[Export] private EnemyDataCollection _enemyDataCollection;
	private PlayerData _playerDataInstance;

	public FighterData GetPlayerFighterData()
	{
		return _playerDataInstance.FighterData;
	}

	public override void _Ready()
	{
		_playerDataInstance ??= _playerDataResource.Duplicate(true) as PlayerData;
	}

	public void ResetPlayerData()
	{
		_playerDataInstance = _playerDataResource.Duplicate(true) as PlayerData;
		GodotLogger.LogDebug($"Resetting player data. Player data: {_playerDataInstance}");
	}

	public void IncreasePlayerExperience(int amount)
	{
		_playerDataInstance.ExperiencePoints += amount;
	}
	
	public void IncreasePlayerExperienceFromCurrentOpponent()
	{
		_playerDataInstance.ExperiencePoints += _playerDataInstance.CurrentOpponent.ExperiencePointsValue;
		GodotLogger.LogDebug($"Player gained {_playerDataInstance.CurrentOpponent.ExperiencePointsValue} experience points");
		GodotLogger.LogDebug($"Player EXP: {_playerDataInstance.ExperiencePoints}");
	}

	public FighterData GetCurrentOpponentData()
	{
		return _playerDataInstance.CurrentOpponent;
	}

	public FighterData GetRandomOpponentData()
	{
		_playerDataInstance.CurrentOpponent = _enemyDataCollection.GetRandomOpponent();
		return _playerDataInstance.CurrentOpponent;
	}
}
