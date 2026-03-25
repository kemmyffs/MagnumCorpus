using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Player : Character
{
	private MoveComponent _move;
	public Vector2 LastDirection { get; private set; } = Vector2.Right;
	private CollisionShape2D MoveCollisionShape;
	[Export] public float ChargeDelay = 5.0f;

	public override void _Ready()
	{
		_move = GetNode<MoveComponent>("MoveComponent");
		MoveCollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector(
			"plr_left", "plr_right",
			"plr_up", "plr_down"
		);

		_move.DesiredDirection = direction;

		if (direction != Vector2.Zero)
			LastDirection = direction;

		if (Input.IsActionPressed("plr_attack"))
			_move.StartCharge();

		if (Input.IsActionJustReleased("plr_attack"))
		{
			_move.StopCharge();
			_move.Dash(LastDirection);
		}
		
		if (Input.IsKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}
	}


	public void _on_dungeon_generator_finished_generation(Array<bool> godotGrid, int x, int y, int height)
	{
		var grid = godotGrid.ToArray<bool>();
		Console.WriteLine("HRAC MA NEJAKY SRANDY S MINIMAPOU");
		Hud hudnode = GetNode<Hud>("CanvasLayer//HUD");
        hudnode.GenerateMinimap(grid, x, y, height);
		
		/*
		TextureRect mapTextureRect = GetNode<TextureRect>("MapTextureRect");

		*/
	}

}