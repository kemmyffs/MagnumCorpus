using Godot;
using System;
using System.Diagnostics;

[Icon("res://customResources//iconPack/32x32/heart.png")]

public partial class HealthComponent : Node2D
{
	private Character _parent;
	private TextureProgressBar HealthBar;
	private TextureProgressBar SpecialBar;
	private Area2D Hurtbox;
	[Export] public int MaxHealth;
	public int CurrentHealth { get; set; }
	[Export] public float SpecialBarRechargeTime = 10.0f;
	public double SpecialBarMaxValue = 100;


	public override void _Ready()
	{
		_parent = GetParent<Character>();
		Hurtbox = GetNode<Area2D>("Hurtbox");
		Console.WriteLine($"RechargeTime: {SpecialBarRechargeTime}");

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
		HealthBar.Value = CurrentHealth;

		SpecialBar.MaxValue = SpecialBarMaxValue;
		SpecialBar.MinValue = 0;
		SpecialBar.Step = 0.00001;
		SpecialBar.Value = SpecialBarMaxValue;
	}

	public override void _Process(double delta)
	{
		if (SpecialBar.Value < SpecialBarMaxValue)
		{
			double amountToAdd = (SpecialBarMaxValue / SpecialBarRechargeTime) * delta;

			// 2. Apply and Clamp
			SpecialBar.Value = Mathf.Clamp(SpecialBar.Value + amountToAdd, 0, SpecialBarMaxValue);
		}
	}


	public void Damage(int dmg)
	{
		CurrentHealth -= dmg;
		if (CurrentHealth <= 0) _parent.Die();
		HealthBar.Value = CurrentHealth;
	}

	public void Heal(int amount)
	{
		CurrentHealth = Math.Max(CurrentHealth + amount, MaxHealth);
		HealthBar.Value = CurrentHealth;
	}
	public void AddSpecialBarValue(double delta)
	{
		SpecialBar.Value = Math.Max(SpecialBar.Value + 1 / delta, SpecialBarMaxValue);
	}
	public void SubtractSpecialBarValue(double value)
	{
		double realValue = SpecialBar.MaxValue / value;
		SpecialBar.Value = Math.Max(SpecialBar.Value - realValue, 0);
	}

	private void OnAreaEntered(Area2D area)
	{

		if (area.Name == "Hitbox")
		{
			if (area.GetParent().GetParent<Character>() == this.GetParent<Character>()) return;
			Character attacker = area.GetParent().GetParent<Character>();
			Damage(attacker._attackComponent.Damage);
		}
	}

	internal bool hasEnoughSpecial(float singleActionCost)
	{
		return SpecialBar.Value > SpecialBar.MaxValue / singleActionCost;
	}

}
