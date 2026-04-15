using Godot;

public partial class Hud : Control
{

	public TileMapLayer TileMapLayer;
	private Label EnemiesLeftLabel;
	private bool[,] roomMapIconGrid;

	private Character PlayerParent;
	public TextureProgressBar HealthBar;
	public TextureProgressBar SpecialBar;

	public override void _Ready()
	{
		TileMapLayer = GetNode<TileMapLayer>("MapCenterContainer//TileMapLayer");
		EnemiesLeftLabel = GetNode<Label>("TextureRect//CenterContainer//EnemiesLeftLabel");
		PlayerParent = GetParent<CanvasLayer>().GetParent<Character>();
		HealthBar = GetNode<TextureProgressBar>("TextureRect//HealthBar"); //TODO
		SpecialBar = GetNode<TextureProgressBar>("TextureRect//SpecialBar"); //TODO
	}

	public async void UpdateEnemyCounter()
	{
		EnemiesLeftLabel.Text = $"{GlobalScript.FloorCurrentEnemyCount}/{GlobalScript.FloorTotalEnemyCount}";
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
					TileMapLayer.SetCell(new Vector2I(i, j), 0, new Vector2I(0, 0), 0);
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

	public void OnEnemyDeath()
	{
		GlobalScript.FloorCurrentEnemyCount--;
		UpdateEnemyCounter();	
		if(GlobalScript.FloorCurrentEnemyCount == 0)
		{
			
		}
	}

}
