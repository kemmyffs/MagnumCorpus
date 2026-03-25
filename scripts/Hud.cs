using Godot;
using Godot.Collections;
using System;

public partial class Hud : Control
{

	public TileMapLayer tileMapLayer;
	private bool[,] roomMapIconGrid;

	public override void _Ready()
	{
		tileMapLayer = GetNode<TileMapLayer>("MapCenterContainer//TileMapLayer");
	}

	public void GenerateMinimap(bool[] grid, int x, int y, int height)
	{
		roomMapIconGrid = Reconstruct(grid, x, y, height);

		for (int i = 0; i < x; i++)
		{
			for (int j = 0; j < y; j++)
			{
				if (!roomMapIconGrid[i, j])
				{
					tileMapLayer.SetCell(new Vector2I(i, j), 0, new Vector2I(0, 0), 0);
            		Console.WriteLine($"Set tile at {i}, {j}");
				}
			}
		}
	}

	bool[,] Reconstruct(bool[] grid, int x, int y, int height)
	{
		int width = x;
		int h = height;

		var reconstructed = new bool[width, h];

		for (int ix = 0; ix < width; ix++)
		{
			for (int iy = 0; iy < h; iy++)
			{
				reconstructed[ix, iy] = grid[ix * h + iy];
			}
		}

		return reconstructed;
	}


}
