using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

public partial class Hud : Control
{

	public TileMapLayer tileMapLayer;
	private Label HPlabel;
	private Label EnemiesLeftLabel;
	private bool[,] roomMapIconGrid;

	private Character PlayerParent;

	public override void _Ready()
	{
		tileMapLayer = GetNode<TileMapLayer>("MapCenterContainer//TileMapLayer");
		HPlabel = GetNode<Label>("HPlabel");
		EnemiesLeftLabel = GetNode<Label>("TextureRect//CenterContainer//EnemiesLeftLabel");
		PlayerParent = GetParent<CanvasLayer>().GetParent<Character>();		

		updateHUD();
	}


	public async void updateHUD()
	{
		await ToSignal(PlayerParent, SignalName.Ready);
		Console.WriteLine(PlayerParent._healthComponent.CurrentHealth);
		HPlabel.Text = PlayerParent._healthComponent.CurrentHealth.ToString();
		

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
