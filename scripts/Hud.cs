using System;
using Godot;

[Icon("res://customResources//iconPack/32x32/window_frame_show.png")]
public partial class Hud : Control
{

	public TileMapLayer TileMapLayer;
	private Label EnemiesLeftLabel;
	private bool[,] roomMapIconGrid;

	private Character PlayerParent;
	public TextureProgressBar HealthBar;
	public TextureProgressBar SpecialBar;
	
	public DialogueManager dialogueManager;

	public override void _Ready()
	{
		TileMapLayer = GetNode<TileMapLayer>("MapCenterContainer//TileMapLayer");
		EnemiesLeftLabel = GetNode<Label>("TextureRect//CenterContainer//EnemiesLeftLabel");
		PlayerParent = GetParent<CanvasLayer>().GetParent<Character>();
		HealthBar = GetNode<TextureProgressBar>("TextureRect//HealthBar"); //TODO
		SpecialBar = GetNode<TextureProgressBar>("TextureRect//SpecialBar"); //TODO

		Console.WriteLine(GetTree().CurrentScene.Name);
		GD.Print(GetTree().CurrentScene.Name);
		if(GetTree().CurrentScene.Name == "TutorialMap")
		{
			GetNode<DialogueManager>("DialogManager").Visible = true;
			dialogueManager = GetNode<DialogueManager>("DialogManager");
			dialogueManager.init();
			dialogueManager.DisplayDialogue();
		} else
		{
			GetNode<DialogueManager>("DialogManager").Visible = false;
			GetNode<DialogueManager>("DialogManager").QueueFree();
		}
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
