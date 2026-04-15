using Godot;
using System;

//0,0 až 23,23
//TODO neni dobrej napad si pocet tilů roomky (bez zdí) automaticky vypocitavat? Mozna?
//doors -1, 10-13

// ----**** TO-DO ****----
//	 
//	*	Předělat generaci mostů a dveří s novými tiles
//	*
//
// ----***************----
public partial class RoomPrefab : Node2D
{

	public TileMapLayer TileMapBase;
	public TileMapLayer TileMapObjects;
	public int FloorSize = 24;


	public override void _Ready()
	{
		TileMapBase = GetNode<TileMapLayer>("TileMapLayer_Base");
	}

	public void RandomizeFloor()
	{
		Random rng = new Random();
		for (int x = 0; x < FloorSize; x++)
		{
			for (int y = 0; y < FloorSize; y++)
			{
				Vector2I tile = Vector2I.Zero;
				tile.X = 3;
				if(rng.NextDouble() < 0.8)
				{
					tile.Y = 0;
				} else
				{
					tile.Y = rng.Next(1,3);
				}

				TileMapBase.SetCell(new Vector2I(x, y), 0, tile, 0);
			}
		}
	}

	public void GenerateRoomObjects()
	{
		//TODO
	}

	public void GenerateEnemies(int count)
	{
		for (int i = 0; i < count; i++)
		{
			SpawnEnemy();
		}
	}

	public void SpawnEnemy()
	{
		Random rng = new Random();
		PackedScene enemyScene = GD.Load<PackedScene>("objects/enemies/testEnemy.tscn");
		Enemy enemyInstance = (Enemy) enemyScene.Instantiate();
		enemyInstance.GlobalPosition = new Vector2(rng.Next(32,361), rng.Next(32,361));
		AddChild(enemyInstance);
		enemyInstance.SetTarget();
	}

	public void SpawnEnemy(GlobalScript.EnemyTypes enemyType)
	{
		
	}

	
	public void SetDoor(Vector2 side, bool open)
	{
		if(open) return;

		int startX = 0, startY = 0, doorWidth = 0, doorHeight = 0;
		Vector2I wallTile = Vector2I.One;
		if (side == Vector2.Up)
		{
			startY = -1;
			startX = 9;
			doorWidth = 6;
			doorHeight = 1;
			wallTile.X = 2;
		}
		else if (side == Vector2.Left)
		{
			startY = 9;
			startX = -1;
			doorWidth = 1;
			doorHeight = 6;
		} else if(side == Vector2.Down)
		{
			startY = 24;
			startX = 9;
			doorWidth = 6;
			doorHeight = 1;
			wallTile.X = 2;
		} else if(side == Vector2.Right)
		{
			startY = 9;
			startX = 24;
			doorWidth = 1;
			doorHeight = 6;
		}

		for (int x = 0; x < doorWidth; x++)
		{
			for (int y = 0; y < doorHeight; y++)
			{
				TileMapBase.SetCell(new Vector2I(startX + x, startY + y), 0, wallTile, 0);
			}
		}
	}

	public void GenerateBridges(bool rightBridge, bool bottomBridge)
	{
		//mosty jsou předem udělaný v každém roomPrefabu –> když přijde FALSE, SCHOVÁVÁM mosty a jen zavřu dveře
		const int startX = 25;
		const int lastX = 30;
		const int startY = 9;
		const int lastY = 15;
		if (!rightBridge)
		{
			//SCHOVAT pravy most a ZAVRIT DVERE
			EraseTileArea(startX, startY, lastX, lastY);
			SetDoor(Vector2.Right, true);
		}

		if (!bottomBridge)
		{
			//SCHOVAT levy most a ZAVRIT DVERE
			EraseTileArea(startY, startX, lastY, lastX);
			SetDoor(Vector2.Up, true);
		}
	}

	/// <summary>
	/// Erases tiles in a rectangular area. Parameters are the coordinates of the Topleft and bottomright tiles. 
	/// </summary>
	/// <param name="x0">Top-left tile's X coordinate</param>
	/// <param name="y0">Top-left tile's Y coordinate</param>
	/// <param name="x1">Bottom-right tile's X coordinate</param>
	/// <param name="y1">Bottom-right tile's Y coordinate</param>
	private void EraseTileArea(int x0, int y0, int x1, int y1)
	{
		for (int x = x0; x <= x1; x++)
			{
				for (int y = y0; y <= y1; y++)
				{
					TileMapBase.EraseCell(new Vector2I(x,y));
				}
			}
	}


}
