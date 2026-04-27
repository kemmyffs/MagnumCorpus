using System;
using Godot;

public partial class PlayerSprite : Node2D
{
	private float Intensity = 0f;

	[Export] private float Target = 0.5f;
	[Export] private float TintSpeed = 0.5f; // 0.5 per second → reaches 0.5 in 1s
	private float blip_strength = 0.0f;

	private ShaderMaterial material;

	public override void _Ready()
	{
		material = (ShaderMaterial) Material;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("plr_charge"))
		{
			Intensity += TintSpeed * (float)delta;
		}
		else
		{
			Intensity -= TintSpeed * (float)delta * 4;
			blip_strength = 0;
		}

		Intensity = Mathf.Clamp(Intensity, 0f, Target);

		if(Intensity >= 0.5f)
		{
			blip_strength = 1;
		} else
		{
			blip_strength = 0;
		}

		material.SetShaderParameter("intensity", Intensity);
		material.SetShaderParameter("blip_strength", blip_strength);
	}
}