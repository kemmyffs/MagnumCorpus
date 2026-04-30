using Godot;
using System;

public partial class EnemyCounterProgress : ColorRect
{

	public float MaxValue = 1f;
	public float Additive;
	private ShaderMaterial smat;
	public override void _Ready()
	{
		smat = (ShaderMaterial)Material;
	}
	public override void _Process(double delta)
	{
		if(Additive > 0)
		{
			smat.SetShaderParameter("fV", (float)smat.GetShaderParameter("fV")+0.001f);
			Additive -= 0.001f;
		}
	}

	public void GraduallyIncrementEnemyProgressBar(float fillPercentage)
	{
		Additive = fillPercentage - (float)smat.GetShaderParameter("fV");
	}
}
