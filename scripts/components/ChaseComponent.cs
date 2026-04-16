using Godot;
using System;
[Icon("res://customResources//iconPack/32x32/location_character.png")]

public partial class ChaseComponent : Node2D
{
	public Enemy Parent;
	public override void _Ready()
	{
		Parent = GetParent<Enemy>();
	}

	public void Area2DBodyEntered(Node2D body)
	{
		if(body.Name.Equals("Player"))
		{
			Parent.SetTarget((Player) body);
			GD.Print("Targeting the player!");
		} else
		{
			Parent.SetTarget(null);
		}
	}

	public void Area2DBodyExited(Node2D body)
	{
		if(body.Name.Equals("Player"))
		{
			Parent.SetTarget(null);
			GD.Print("Lost track of player :(");
		}
	}
}
