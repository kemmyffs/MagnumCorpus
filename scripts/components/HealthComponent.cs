using Godot;
using System;

public partial class HealthComponent : Node2D
{
	private Character _parent;
	private TextureProgressBar HealthBar;
	private TextureProgressBar SpecialBar;
	private Area2D Hurtbox;
	[Export] public int MaxHealth;
	public int CurrentHealth { get; set; }
	[Export] public int SpecialBarRechargeTime { get; set; }
	public double SpecialBarCurrentValue {get; set;}



	public override void _Ready()
	{
		_parent = GetParent<Character>();
		MaxHealth = _parent.MaxHealth;
		Hurtbox = GetNode<Area2D>("Hurtbox");

		Hurtbox.AreaEntered += OnAreaEntered;

		if (_parent.Name == "Player")
		{
			
			HealthBar = _parent.GetNode<TextureProgressBar>("CanvasLayer//HUD//TextureRect//HealthBar");
			SpecialBar = _parent.GetNode<TextureProgressBar>("CanvasLayer//HUD//TextureRect//SpecialBar");

			GetNode<TextureProgressBar>("HealthBar").Visible = false;
			GetNode<TextureProgressBar>("SpecialBar").Visible = false;
			GetNode<TextureRect>("Background").Visible = false;

		}
		else
		{
			HealthBar = GetNode<TextureProgressBar>("HealthBar");
			SpecialBar = GetNode<TextureProgressBar>("SpecialBar");
		}

		HealthBar.MaxValue = MaxHealth;
		HealthBar.MinValue = 0;
		CurrentHealth = MaxHealth;
		UpdateHealthBar();

	}

	public override void _Process(double delta)
	{
		SpecialBarCurrentValue += delta;
    	SpecialBarCurrentValue = Math.Min(SpecialBarCurrentValue, SpecialBarRechargeTime);
		//UpdateSpecialBar();
		//UpdateHealthBar();
	}

	public void Damage(int dmg)
	{
		CurrentHealth -= dmg;
		if(CurrentHealth <= 0) _parent.Die();
		UpdateHealthBar();
	}

	public void Heal(int amount)
	{
		CurrentHealth = Math.Max(CurrentHealth + amount, MaxHealth);
		UpdateHealthBar();
	}
	public void UpdateHealthBar()
	{
		HealthBar.Value = CurrentHealth;
	}

	public void UpdateSpecialBar()
	{
		//SpecialBar.Value = SpecialBarCurrentValue;
	}

	private void OnAreaEntered(Area2D area)
	{
		
		if(area.Name == "Hitbox")
		{
			if(area.GetParent().GetParent<Character>() == this.GetParent<Character>()) return;
			Character attacker = area.GetParent().GetParent<Character>();
			Damage(attacker._attackComponent.Damage);
			
			
		}
		//
		// Nevypínat monitorable v AttackComponent, ale koukat jestli "útočí"
		//
		
	}
	
}
