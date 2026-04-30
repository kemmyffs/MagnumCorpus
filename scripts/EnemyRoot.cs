using Godot;
using System;

public partial class EnemyRoot : Node2D
{
	Random Rng = new Random();
	DungeonGenerator dungeonGenerator;

	public override void _Ready()
	{
		base._Ready();
		dungeonGenerator = GetParent<DungeonGenerator>();
	}


	//enemyRoot.GenerateEnemies(TempEnemyCount, RoomToGlobalCoords(pos, RandomRoomCoordinates()));
	public void GenerateEnemies(int count, Vector2 roomPosition) // Pass room index, not final coords
	{
		for (int i = 0; i < count; i++)
		{
			// Generate a new random spot for EVERY enemy
			Vector2 localCoords = dungeonGenerator.RandomRoomCoordinates();
			Vector2 finalGlobalPos = dungeonGenerator.RoomToGlobalCoords(roomPosition, localCoords);

			SpawnEnemy(GetRandomEnemyType(), finalGlobalPos);
		}
	}
	/*public void GenerateEnemies(int count, GlobalScript.EnemyTypes type)
	{
		//TODO
	}
	*/

	private void SpawnEnemy(GlobalScript.EnemyTypes enemyType, Vector2 globalPosition)
	{
		string path = $"res://objects/enemies/{enemyType}.tscn";
		PackedScene enemyScene = GD.Load<PackedScene>(path);

		if (enemyScene == null)
		{
			GD.PushError($"Failed to load enemy scene at: {path}");
			return;
		}
		Enemy enemyInstance = enemyScene.Instantiate<Enemy>();

		AddChild(enemyInstance);
		
		enemyInstance.GlobalPosition = globalPosition;
	}

	private GlobalScript.EnemyTypes GetRandomEnemyType()
	{
		Array values = Enum.GetValues(typeof(GlobalScript.EnemyTypes));
		Random rand = new Random();
		return (GlobalScript.EnemyTypes)values.GetValue(rand.Next(values.Length));
	}

}
