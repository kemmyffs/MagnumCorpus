using Godot;
using System;

public partial class HealthComponent : Node2D
{

	private Character _parent;
	public int MaxHealth;
	public int CurrentHealth {get; set;}
	
	public override void _Ready()
	{
		_parent = GetParent<Character>();
		MaxHealth = _parent.MaxHealth;
		CurrentHealth = MaxHealth;
	}

	public override void _Process(double delta)
	{
	}

	public void Damage(int dmg)
	{
		CurrentHealth -= dmg;
	}

	public void Heal(int amount)
	{
		CurrentHealth = Math.Max(CurrentHealth + amount, MaxHealth);
	}

	public void updateHealthBar()
	{
		//KAŽDÝ CHARACTER BUDE MÍT VLASTNÍ REFERENCI NA JEHO HEALTHBAR
		//HRÁČ HO MÁ V UI, OSTATNÍ POD SEBOU!!!!
		//GENIÁLNÍ KEMMY, GRATULUJU
	}
}
