using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Player : Character
{
	public Vector2 LastDirection { get; private set; } = Vector2.Right;
	[Export] public float ChargeDelay = 5.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector(
			"plr_left", "plr_right",
			"plr_up", "plr_down"
		);

		_moveComponent.DesiredDirection = direction;

		if (direction != Vector2.Zero)
			LastDirection = direction;

		if (Input.IsActionPressed("plr_attack"))
			_moveComponent.StartCharge();

		if (Input.IsActionJustReleased("plr_attack"))
		{
			_moveComponent.StopCharge();
			_moveComponent.Dash(LastDirection);
		}
		
		if (Input.IsKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}
	}


	public void _on_dungeon_generator_finished_generation(Array<bool> godotGrid, int x, int y, int height)
	{
		var grid = godotGrid.ToArray<bool>();
		Hud hudnode = GetNode<Hud>("CanvasLayer//HUD");
        hudnode.GenerateMinimap(grid, x, y, height);
		
		/*
		TextureRect mapTextureRect = GetNode<TextureRect>("MapTextureRect");

		*/
	}

}