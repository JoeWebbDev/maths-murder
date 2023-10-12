using Godot;

public partial class GameDataManager : Node
{
	[Export] private PlayerData _playerDataResource;
	[Export] private EnemyDataCollection _enemyDataCollection;
	private PlayerData _playerDataInstance;
	private NotificationSystem _notificationSystem;

	public FighterData GetPlayerFighterData()
	{
		return _playerDataInstance.FighterData;
	}

	public PlayerData GetPlayerData()
	{
		return _playerDataInstance;
	}

	public int GetPlayerExperiencePoints()
	{
		return _playerDataInstance.ExperiencePoints;
	}

	public override void _Ready()
	{
		_playerDataInstance ??= _playerDataResource.Duplicate(true) as PlayerData;
		_notificationSystem = GetNode<NotificationSystem>("/root/NotificationSystem");
		_notificationSystem.DamageDealt += (fighter, amount) =>
		{
			if (fighter.PlayerNumber == 1) _playerDataInstance.DamageDealt += amount;
		};
		_notificationSystem.DamageTaken += (fighter, amount) =>
		{
			if (fighter.PlayerNumber == 1) _playerDataInstance.DamageTaken += amount;
		};
	}

	public void ResetPlayerData()
	{
		_playerDataInstance = _playerDataResource.Duplicate(true) as PlayerData;
		GodotLogger.LogDebug($"Resetting player data. Player data: {_playerDataInstance}");
	}

	public void IncreasePlayerExperience(int amount)
	{
		_playerDataInstance.ExperiencePoints += amount;
		_playerDataInstance.TotalExpGained += amount;
	}

	public void DecreasePlayerExperience(int amount)
	{
		_playerDataInstance.ExperiencePoints -= amount;
	}
	
	public void IncreasePlayerExperienceFromCurrentOpponent()
	{
		IncreasePlayerExperience(_playerDataInstance.CurrentOpponent.ExperiencePointsValue);
		GodotLogger.LogDebug($"Player gained {_playerDataInstance.CurrentOpponent.ExperiencePointsValue} experience points");
		GodotLogger.LogDebug($"Player EXP: {_playerDataInstance.ExperiencePoints}");
	}

	public FighterData GetCurrentOpponentData()
	{
		return _playerDataInstance.CurrentOpponent;
	}

	public void SetCurrentOpponentData(FighterData opponent)
	{
		_playerDataInstance.CurrentOpponent = opponent;
	}

	public FighterData GetRandomOpponentData(bool setAsCurrentOpponent = true)
	{
		var opponent = _enemyDataCollection.GetRandomOpponent();
		if (setAsCurrentOpponent) _playerDataInstance.CurrentOpponent = opponent;
		return opponent;
	}

	public void RegisterDefeatedOpponent()
	{
		IncreasePlayerExperienceFromCurrentOpponent();
		_playerDataInstance.DefeatedOpponents.Add(_playerDataInstance.CurrentOpponent);
		_playerDataInstance.CurrentOpponent = null;
	}
}
