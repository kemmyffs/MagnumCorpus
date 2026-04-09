using Godot;
using System;


public partial class Enemy : Character
{
	private Character Target;

    public override void _Ready()
    {
        base._Ready();
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
			Target = GetParent().GetParent().GetParent().GetNode<Character>("Player");
			Console.WriteLine($"Mam hrace! Tady je:{Target}");
		}
		catch (System.NullReferenceException)
		{
			Console.WriteLine("Neni hrac k nalezeni wtf");
		}
	}
}
