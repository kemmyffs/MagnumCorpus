/*
using Godot;
using System;

public partial class DungeonGenerator : Node2D
{
	[Export] Vector2I WorldSize;
	[Export] int NumberOfRooms;
	[Export] public PackedScene RoomPrefab;
	[Export] private int RoomOffset;

	[Export] private int StartOverChance;
	[Export] private int StartOverIncrease;
	[Export] private int MaxStartOverChance;
	private Node2D MapRoot;

	private Room[,] RoomGrid;

	public override void _Ready()
	{

		MapRoot = GetNode<Node2D>("MapRoot");
		RoomGrid = new Room[WorldSize.X, WorldSize.Y];

		GenerateDungeon();
	}


	public void GenerateDungeon()
	{
		int usedRooms = 0;
		var rng = new Random();
		Vector2I currentPosition;
		Vector2I randomDirection = Vector2I.Zero;

		Vector2I centerPoint = new(WorldSize.X / 2, WorldSize.Y / 2);

		Room firstRoom = RoomPrefab.Instantiate<Room>();
		RoomGrid[centerPoint.X, centerPoint.Y] = firstRoom;
		MapRoot.AddChild(firstRoom);
		firstRoom.Position = new Vector2(centerPoint.X * 40, centerPoint.Y * 40);
		firstRoom.SetMapIconColor(firstRoom.EnterColor);
		usedRooms++;

		currentPosition = centerPoint;
		//
		while (usedRooms < NumberOfRooms)
		{
			if (rng.Next(0, 101) < StartOverChance)
			{
				currentPosition = centerPoint;
				StartOverChance = 0;

			}
			else
			{
				StartOverChance = Math.Min(StartOverChance + StartOverIncrease, MaxStartOverChance);
			}
			randomDirection.X = rng.Next(-1, 2);
			randomDirection.Y = (randomDirection.X == 0) ? rng.Next(-1, 2) : 0;

			// X bounds
			if (currentPosition.X + randomDirection.X >= WorldSize.X)
			{
				randomDirection.X = -1;
			}
			else if (currentPosition.X + randomDirection.X < 0)
			{
				randomDirection.X = 1;
			}

			// Y bounds
			if (currentPosition.Y + randomDirection.Y >= WorldSize.Y)
			{
				randomDirection.Y = -1;
			}
			else if (currentPosition.Y + randomDirection.Y < 0)
			{
				randomDirection.Y = 1;
			}

			currentPosition += randomDirection;

			if (RoomGrid[currentPosition.X, currentPosition.Y] != null) continue;

			Room newRoom = RoomPrefab.Instantiate<Room>();
			RoomGrid[currentPosition.X, currentPosition.Y] = newRoom;
			MapRoot.AddChild(newRoom);
			newRoom.Position = new Vector2(currentPosition.X * 40, currentPosition.Y * 40);
			newRoom.SetMapIconColor(newRoom.NormalColor);
			usedRooms++;
		}

		GenerateBridges();

	}

	private void GenerateBridges()
	{
		for (int x = 0; x < WorldSize.X-1; x++)
		{
			for (int y = 0; y < WorldSize.Y-1; y++)
			{
				var currentRoom = RoomGrid[x,y];
				if (currentRoom == null) {
					continue;
				}
				//right
				if(RoomGrid[x+1,y] != null)
				{
					currentRoom.showBridge(true);
					Console.WriteLine(currentRoom);
				}

				//bottom
				if(RoomGrid[x,y+1] != null)
				{
					RoomGrid[x,y].showBridge(false);
				}
			}
		}
	}
}

*/


//
// ***### ISAAC-LIKE GENERATION ###***
//






using Godot;
using Godot.NativeInterop;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DungeonGenerator : Node2D
{
    [Export] Vector2I WorldSize;
    [Export] int NumberOfRooms;
    [Export] public PackedScene RoomMapIconPrefab;
    [Export] public PackedScene RoomPrefab;
    [Export] public int roomTileSize = 16;
    [Export] private int RoomOffset = 16 * 16;


    [Signal] public delegate void FinishedGenerationEventHandler(Array<bool> grid, int x, int y, int height);

    //[Signal] public delegate void FinishedGenerationEventHandler(RoomPrefab[,] a);

    private Node2D MapRoot;
    private RoomMapIcon[,] RoomGrid_Icons;
    private RoomPrefab[,] RoomGrid_Prefabs;

    private Random rng = new Random();

    public override void _Ready()
    {
        MapRoot = GetNode<Node2D>("MapRoot");
        RoomGrid_Icons = new RoomMapIcon[WorldSize.X, WorldSize.Y];
        RoomGrid_Prefabs = new RoomPrefab[WorldSize.X, WorldSize.Y];

        GenerateDungeon();
        EmitFlattenedGrid(RoomGrid_Icons);

    }

    public void GenerateDungeon()
    {
        List<Vector2I> placedRooms = new();
        List<Vector2I> frontier = new();

        Vector2I center = new(WorldSize.X / 2, WorldSize.Y / 2);

        PlaceRoom(center, true);
        placedRooms.Add(center);
        frontier.Add(center);

        while (placedRooms.Count < NumberOfRooms && frontier.Count > 0)
        {
            // Pick random frontier room
            Vector2I baseRoom = frontier[rng.Next(frontier.Count)];

            List<Vector2I> directions = new()
            {
                Vector2I.Up,
                Vector2I.Down,
                Vector2I.Left,
                Vector2I.Right
            };

            // Shuffle directions
            directions = directions.OrderBy(x => rng.Next()).ToList();

            bool placed = false;

            foreach (var dir in directions)
            {
                Vector2I newPos = baseRoom + dir;

                if (!IsInside(newPos)) continue;
                if (RoomGrid_Icons[newPos.X, newPos.Y] != null) continue;

                // 🔥 Prevent blobs (IMPORTANT)
                if (CountNeighbors(newPos) > 1) continue;

                PlaceRoom(newPos, false);

                placedRooms.Add(newPos);
                frontier.Add(newPos);

                placed = true;
                break;
            }

            // If this room can't expand anymore, remove it
            if (!placed)
                frontier.Remove(baseRoom);
        }

        GenerateBridges();
    }

    private void PlaceRoom(Vector2I pos, bool isStart)
    {
        //ICONS
        RoomMapIcon newRoom = RoomMapIconPrefab.Instantiate<RoomMapIcon>();
        RoomGrid_Icons[pos.X, pos.Y] = newRoom;
        newRoom.Position = new Vector2(pos.X * RoomOffset, pos.Y * RoomOffset);

        if (isStart)
            newRoom.SetMapIconColor(newRoom.EnterColor);
        else
            newRoom.SetMapIconColor(newRoom.NormalColor);

        //PREFABS
        RoomPrefab newRoomPrefab = RoomPrefab.Instantiate<RoomPrefab>();
        RoomGrid_Prefabs[pos.X, pos.Y] = newRoomPrefab;
        newRoomPrefab.Position = new Vector2(pos.X * RoomOffset, pos.Y * RoomOffset);
        MapRoot.AddChild(newRoomPrefab);


    }

    private bool IsInside(Vector2I pos)
    {
        return pos.X >= 0 && pos.X < WorldSize.X &&
               pos.Y >= 0 && pos.Y < WorldSize.Y;
    }

    private int CountNeighbors(Vector2I pos)
    {
        int count = 0;

        Vector2I[] directions = new Vector2I[]
        {
            Vector2I.Up,
            Vector2I.Down,
            Vector2I.Left,
            Vector2I.Right
        };

        foreach (var dir in directions)
        {
            Vector2I check = pos + dir;

            if (IsInside(check) && RoomGrid_Icons[check.X, check.Y] != null)
                count++;
        }

        return count;
    }

    private void GenerateBridges()
    {
        for (int x = 0; x < WorldSize.X; x++)
        {
            for (int y = 0; y < WorldSize.Y; y++)
            {
                var currentRoom = RoomGrid_Icons[x, y];
                if (currentRoom == null) continue;

                // Right
                if (x + 1 < WorldSize.X && RoomGrid_Icons[x + 1, y] != null)
                {
                    currentRoom.showBridge(true);
                }

                // Down
                if (y + 1 < WorldSize.Y && RoomGrid_Icons[x, y + 1] != null)
                {
                    currentRoom.showBridge(false);
                }
            }
        }
    }


    void EmitFlattenedGrid(RoomMapIcon[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        var result = new bool[width * height];
        int x = 0;
        int y = 0;
        for (x = 0; x < width; x++)
        {
            for (y = 0; y < height; y++)
            {
                if (grid[x,y] == null)
                {
                    result[x * height + y] = true;
                } else
                {
                    result[x * height + y] = false;
                }
            }
        }
        var godotResult = new Array<bool>(result);
        EmitSignal(SignalName.FinishedGeneration, godotResult, x, y, height);

    }

}
