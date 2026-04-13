using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	//Connectnout v Godotu a potom udělat public void funkci se stejným jménem
	public void _on_button_button_up()
	{
		Console.WriteLine("Zmack");
		GetTree().ChangeSceneToFile("res://objects/rooms/dungeon_generator.tscn");
	}
}
