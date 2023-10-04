using Godot;

public partial class GameDataManager : Node
{
	[Export] private PlayerData _playerData;
	[Export] private EnemyDataCollection _enemyDataCollection;

	public FighterData GetPlayerFighterData()
	{
		return _playerData.FighterData;
	}

	public void IncreasePlayerExperience(int amount)
	{
		_playerData.ExperiencePoints += amount;
	}
	
	public void IncreasePlayerExperienceFromCurrentOpponent()
	{
		_playerData.ExperiencePoints += _playerData.CurrentOpponent.ExperiencePointsValue;
	}

	public FighterData GetCurrentOpponentData()
	{
		return _playerData.CurrentOpponent;
	}

	public FighterData GetRandomOpponentData()
	{
		_playerData.CurrentOpponent = _enemyDataCollection.GetRandomOpponent();
		return _playerData.CurrentOpponent;
	}
}
