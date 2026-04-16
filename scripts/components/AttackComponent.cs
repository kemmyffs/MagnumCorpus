using Godot;
using System;
using System.Threading.Tasks;

[Icon("res://customResources//iconPack/32x32/sword.png")]

public partial class AttackComponent : Node2D
{
	[Export] public int AttackOffset;
	[Export] public int Damage;
	public enum AttackType
	{

	};

	public bool CanAttack = true;

	private Character _parent;
	private AnimatedSprite2D AttackAnimatedSprite;
	private Area2D Hitbox;
	private CollisionShape2D HitboxShape;


	public override void _Ready()
	{
		_parent = GetParent<Character>();
		AttackAnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Hitbox = GetNode<Area2D>("Hitbox");
		HitboxShape = Hitbox.GetNode<CollisionShape2D>("CollisionShape2D");
		HitboxShape.Disabled = true;
	}


	public override void _Process(double delta)
	{
		if (!AttackAnimatedSprite.IsPlaying())
		{
			AttackAnimatedSprite.Play("idle");
			CanAttack = true;
		}
	}

	public async void Attack(Vector2 direction)
	{
		CanAttack = false;
		Vector2 attackDir = direction.Normalized();

		if (attackDir == Vector2.Up)
		{
			this.RotationDegrees = -90;
		}
		else if (attackDir == Vector2.Down)
		{
			this.RotationDegrees = 90;
		}
		else if (attackDir == Vector2.Left)
		{
			this.RotationDegrees = 180;
		}
		else if (attackDir == Vector2.Right)
		{
			this.RotationDegrees = 0;
		}

		AttackAnimatedSprite.Play("plr_slash");
		
		HitboxShape.Disabled = false;
		await ToSignal(AttackAnimatedSprite, AnimatedSprite2D.SignalName.AnimationFinished);
		HitboxShape.Disabled = true;
		CanAttack = true;
	}
}