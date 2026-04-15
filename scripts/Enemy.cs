using Godot;
using System;


public partial class Enemy : Character
{
	private Player Target;
	[Signal] public delegate void JustDiedEventHandler();

	public override void _Ready()
	{
		base._Ready();
		SetTarget();
		Connect("JustDied", new Callable(Target.GetNode<CanvasLayer>("CanvasLayer").GetNode<Hud>("HUD"), "OnEnemyDeath"));
	}
	public override void _PhysicsProcess(double delta)
	{
		if(!IsInstanceValid(Target)) return;

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

    public override void Die()
    {
		EmitSignal(SignalName.JustDied);
        base.Die();
    }
}
