using Godot;
using System;
using System.Threading.Tasks;

[Icon("res://customResources//iconPack/32x32/sword.png")]

public partial class AttackComponent : Component
{

	[Export] public GlobalScript.AttackTypes AttackType { get; set; }
	[Export] public int AttackOffset = 0;
	[Export] public int Damage = 1;

	public bool CanAttack = true;
	private AnimatedSprite2D AttackAnimatedSprite;
	private Area2D Hitbox;
	private CollisionShape2D HitboxShape;
	private Action<double> _attackProcess = null;

	public override void _Ready()
	{
		base._Ready();

		ResolveSetupByAttackType();
		// Ano, já vím. Ano, tak to má být. Z tohohle ready potřebuju odejít,
		// aby se dodělaly jiné ready (například Parentova) a tím se až může
		// dokončit ta funkce ResolveSetupByAttackType, protože přesně na to čeká

	}

	private async Task ResolveSetupByAttackType()
	{
		Hitbox = GetNode<Area2D>("Hitbox");
		HitboxShape = Hitbox.GetNode<CollisionShape2D>("CollisionShape2D");

		if (AttackType == GlobalScript.AttackTypes.Touch)
		{
			HitboxShape.Disabled = false; //vždycky aktivní
			_attackProcess = ProcessConsistentAttacks;
			//ReshapeHitbox();
		}
		else
		{
			AttackAnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			HitboxShape.Disabled = true;
			_attackProcess = ProcessActivationAttacks;
		}
	}

	private async Task ReshapeHitbox()
	{
		await ToSignal(Parent, Node.SignalName.Ready);
		HitboxShape.Shape = ShapeManipulator.CopyExpandedShape(Parent.MoveCollisionShape.Shape);
	}

	public override void _Process(double delta)
	{
		_attackProcess?.Invoke(delta);
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

	private void ProcessActivationAttacks(double delta)
	{
		if (!AttackAnimatedSprite.IsPlaying())
		{
			AttackAnimatedSprite.Play("idle");
			CanAttack = true;
		}
	}

	private void ProcessConsistentAttacks(double delta)
	{

	}
}