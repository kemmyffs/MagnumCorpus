using Godot;
using System;


public partial class Enemy : Character
{
	private Player Target;
	private Player playerNode;
	[Export] public int ChaseRadius;
	[Signal] public delegate void JustDiedEventHandler();

	public override void _Ready()
	{
		base._Ready();
		playerNode = GetParent().GetParent().GetParent().GetNode<Player>("Player");
		Connect("JustDied", new Callable(playerNode.GetNode<Hud>("CanvasLayer/HUD"), "OnEnemyDeath"));

		if(HasNode("ChaseComponent"))
		{
			var shape = (CircleShape2D)GetNode<CollisionShape2D>("ChaseComponent/Area2D/CollisionShape2D").Shape;
			shape.Radius = ChaseRadius;
		}

	}
	public override void _PhysicsProcess(double delta)
	{
		if(!IsInstanceValid(Target) || Target == null) return;

		Vector2 direction = (Target.GlobalPosition - GlobalPosition).Normalized();
		
		_moveComponent.DesiredDirection = direction;
	}

	public void SetTarget()
	{
		try
		{
			Target = GetParent().GetParent().GetParent().GetNode<Player>("Player");
		}
		catch (System.NullReferenceException)
		{
			Target =  null;
		}
	}
	public void SetTarget(Player body)
	{
		Target = body;
	}

    public override void Die()
    {
		EmitSignal(SignalName.JustDied);
        base.Die();
    }
}
