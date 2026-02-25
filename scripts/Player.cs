using Godot;
using System;

public static class SpeedEnum
{
	public const float Default = 300.0f;
	public const float Sprint = 400.0f;
	public const float Dash = 800.0f;
	public const float Slowed = 150.0f;
}


public partial class Player : CharacterBody2D
{
	public float Speed = SpeedEnum.Default;

	public Vector2 attackDiretcion = Vector2.Zero;

	AnimatedSprite2D animatedSprite_weaponSlash;
	public override void _Ready()
	{
		animatedSprite_weaponSlash = GetNode<AnimatedSprite2D>("WeaponSlash");
	}


	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if (Speed <= SpeedEnum.Dash && Speed >= SpeedEnum.Default)
		{
			Speed -= 25;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity = direction * Speed;
			attackDiretcion = direction;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		if (Input.IsActionJustReleased("plr_attack"))
		{

			animatedSprite_weaponSlash.Play("slashC");
			Speed = SpeedEnum.Dash;
		}

		if (Input.IsActionPressed("plr_attack"))
		{
			if (Speed > 0)
			{
				Speed -= 10;
			}
			else if (Speed <= 0)
			{
				Speed = 0;
			}

		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void _on_weapon_slash_animation_finished()
	{
		//Console.WriteLine("Slashed!");
		animatedSprite_weaponSlash.Animation = "idle";
	}
}
