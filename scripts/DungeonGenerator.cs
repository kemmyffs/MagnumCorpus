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
    [Export] public int roomTileCount;
    [Export] public int tileSizePx;
    [Export] private int RoomOffset;
    private Vector2I CenterRoomCoords;

    [Signal] public delegate void FinishedGenerationEventHandler(Array<bool> grid, int x, int y, int height);

    private Node2D MapRoot;
    private RoomMapIcon[,] RoomGrid_Icons;
    private RoomPrefab[,] RoomGrid_Prefabs;
    private Character PlayerNode;

    private Random rng = new Random();

    public override void _Ready()
    {
        MapRoot = GetNode<Node2D>("MapRoot");
        RoomGrid_Icons = new RoomMapIcon[WorldSize.X, WorldSize.Y];
        RoomGrid_Prefabs = new RoomPrefab[WorldSize.X, WorldSize.Y];
        PlayerNode = GetNode<Character>("Player");
        CenterRoomCoords = new Vector2I(WorldSize.X / 2, WorldSize.Y / 2);

        GenerateDungeonMap();
        EmitFlattenedGrid(RoomGrid_Icons);
        setFloorColor(new Color(0.0f, 1.419f, 0.783f));


    }

    public void GenerateDungeonMap()
    {
        List<Vector2I> placedRooms = new();
        List<Vector2I> frontier = new();

        PlaceRoom(CenterRoomCoords);
        placedRooms.Add(CenterRoomCoords);
        Vector2 roomV2 = placedRooms.First();
        frontier.Add(CenterRoomCoords);

        

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

                // Prevent blobs (IMPORTANT)
                if (CountNeighbors(newPos) > 1) continue;

                PlaceRoom(newPos);

                placedRooms.Add(newPos);
                frontier.Add(newPos);

                placed = true;
                break;
            }

            // If this room can't expand anymore, remove it
            if (!placed)
                frontier.Remove(baseRoom);
        }
        int pos = (CenterRoomCoords.X * tileSizePx * roomTileCount) + ((tileSizePx * roomTileCount)/2);
        PlayerNode.GlobalPosition = new Vector2(pos, pos);
        GenerateBridges();


    }

    private void PlaceRoom(Vector2I pos)
    {
        //ICONS
        RoomMapIcon newRoom = RoomMapIconPrefab.Instantiate<RoomMapIcon>();
        RoomGrid_Icons[pos.X, pos.Y] = newRoom;
        /* newRoom.Position = new Vector2(pos.X * (tileSizePx), pos.Y * tileSizePx);

        if (isStart)
            newRoom.SetMapIconColor(newRoom.EnterColor);
        else
            newRoom.SetMapIconColor(newRoom.NormalColor);  */

        //PREFABS
        RoomPrefab newRoomPrefab = RoomPrefab.Instantiate<RoomPrefab>();

        RoomGrid_Prefabs[pos.X, pos.Y] = newRoomPrefab;
        int TrueRoomSize = roomTileCount * tileSizePx;
        newRoomPrefab.Position = new Vector2(pos.X * (RoomOffset + TrueRoomSize), pos.Y * (RoomOffset + TrueRoomSize));
        MapRoot.AddChild(newRoomPrefab);
        newRoomPrefab.RandomizeFloor();
        newRoomPrefab.GenerateEnemies(1);
        //newRoomPrefab.SetDoor(Vector2.Up, false);
        //newRoomPrefab.SetDoor(Vector2.Left, false);

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
                var currentRoom = RoomGrid_Prefabs[x, y];
                if (currentRoom == null) continue;
                bool[] bridges_TopDownLeftRight = new bool[4];

                // Right
                if (x + 1 < WorldSize.X && RoomGrid_Prefabs[x + 1, y] != null) bridges_TopDownLeftRight[3] = true;
                // Left
                if (x - 1 >= 0 && RoomGrid_Prefabs[x - 1, y] != null) bridges_TopDownLeftRight[2] = true;
                // Down
                if (y + 1 < WorldSize.Y && RoomGrid_Prefabs[x, y + 1] != null) bridges_TopDownLeftRight[1] = true;
                // Up
                if (y - 1 >= 0 && RoomGrid_Prefabs[x, y - 1] != null) bridges_TopDownLeftRight[0] = true;


                currentRoom.GenerateBridges(bridges_TopDownLeftRight[3], bridges_TopDownLeftRight[1]);

                currentRoom.SetDoor(Vector2.Up, bridges_TopDownLeftRight[0]);
                currentRoom.SetDoor(Vector2.Down, bridges_TopDownLeftRight[1]);
                currentRoom.SetDoor(Vector2.Left, bridges_TopDownLeftRight[2]);
                currentRoom.SetDoor(Vector2.Right, bridges_TopDownLeftRight[3]);
            }
        }
        /*
        //icons
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
        */

        //
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
                if (grid[x, y] == null)
                {
                    result[x * height + y] = true;
                }
                else
                {
                    result[x * height + y] = false;
                }
            }
        }
        var godotResult = new Array<bool>(result);
        EmitSignal(SignalName.FinishedGeneration, godotResult, x, y, height);

    }

    private void setFloorColor(Color desiredColor)
    {
        //MapRoot.Modulate = desiredColor;
        foreach (RoomPrefab room in MapRoot.GetChildren())
        {
            room.TileMapBase.Modulate= desiredColor;
        }
    }
    
}
