using Godot;
using System;

public partial class HealthComponent : Node2D
{

	private Character _parent;
	private TextureProgressBar HealthBar;
	private TextureProgressBar SpecialBar;
	public int MaxHealth;
	public int CurrentHealth { get; set; }


	public override void _Ready()
	{
		_parent = GetParent<Character>();
		MaxHealth = _parent.MaxHealth;
		if (_parent.Name == "Player")
		{
			Player tmp = (Player) _parent;
			HealthBar = tmp.GetHealthBar();
			SpecialBar = tmp.GetSpecialbar();
			GetNode<TextureProgressBar>("HealthBar").Visible = false;
			GetNode<TextureProgressBar>("SpecialBar").Visible = false;
			GetNode<TextureRect>("Background").Visible = false;
		} else
		{
			HealthBar = GetNode<TextureProgressBar>("HealthBar");
			SpecialBar = GetNode<TextureProgressBar>("SpecialBar");
		}
		CurrentHealth = MaxHealth;
		HealthBar.MaxValue = MaxHealth;
		HealthBar.MinValue = 0;

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
