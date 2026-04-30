using System;
using Godot;
[Icon("res://customResources//iconPack/32x32/arrow_out.png")]
public partial class MoveComponent : Component
{
	private CollisionShape2D MoveCollisionShape;
	public float BaseSpeed;
	[Export] public float DashForce = 200;
	[Export] public float DashDecay = 750; // higher -> stops faster
	[Export] public float Acceleration = 2000f;
	[Export] public float Friction = 2000f;
	[Export] public float ChargeFriction = 300f;

	private Vector2 _dashVelocity = Vector2.Zero;

	private bool _isCharging = false;
	private bool _isDashing = false;

	public Vector2 DesiredDirection { get; set; } = Vector2.Zero;

	public override void _Ready()
	{
		Parent = GetParent<Character>();
		BaseSpeed = Parent.BaseSpeed;
		MoveCollisionShape = Parent.GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		float d = (float)delta;
		Vector2 velocity = Parent.Velocity;

		_isDashing = _dashVelocity.Length() > 0;
		//Charge
		//Transitions into either a full stop(not moving) or the default moving speed (if moving)
		if (_isCharging)
		{
			velocity = velocity.MoveToward(
				Vector2.Zero,
				ChargeFriction * d
			);
		}
		else
		{
			// Normal movement
			if (DesiredDirection != Vector2.Zero)
			{
				Vector2 target = DesiredDirection * BaseSpeed;
				velocity = velocity.MoveToward(
					target,
					Acceleration * d
				);
			}
			else
			{
				velocity = velocity.MoveToward(
					Vector2.Zero,
					Friction * d
				);
			}
		}

		if (_dashVelocity.Length() > 0)
		{
			velocity += _dashVelocity;

			_dashVelocity = _dashVelocity.MoveToward(
				Vector2.Zero,
				DashDecay * d
			);
		}

		Parent.SetCollisionMaskValue(1, !_isDashing);
		Parent.SetCollisionMaskValue(2, _isDashing);
		Parent.SetCollisionLayerValue(1, !_isDashing);
		Parent.SetCollisionLayerValue(2, _isDashing);


		//Smoothing transition between a dash move and a regular move
		//so the player doesn't need to wait for the dash to end before he can move (which caused stops after dashes)
		Vector2 combined = velocity + _dashVelocity;
		if (combined.Length() > BaseSpeed + _dashVelocity.Length())
			combined = combined.Normalized() * (BaseSpeed + _dashVelocity.Length());
		velocity = combined;

		Parent.Velocity = velocity;

		Parent.MoveAndSlide();
	}

	public void Dash(Vector2 direction)
	{
		if (direction == Vector2.Zero)
			return;
		if (Parent._healthComponent.hasEnoughSpecial(Parent.ChargesAmountInFullBar))
		{
			Parent._healthComponent.SubtractSpecialBarValue(Parent.ChargesAmountInFullBar);
			_dashVelocity = direction.Normalized() * DashForce;
		}

	}

	public void StartCharge()
	{
		_isCharging = true;
	}

	public void StopCharge()
	{
		_isCharging = false;
	}
}