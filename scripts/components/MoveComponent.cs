using System;
using Godot;

public partial class MoveComponent : Node
{
	private Character _parent;
	private CollisionShape2D MoveCollisionShape;

	[Export] public float BaseSpeed = 150;
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
		_parent = GetParent<Character>();
		BaseSpeed = _parent.BaseSpeed;
		MoveCollisionShape = _parent.GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public override void _PhysicsProcess(double delta)
	{

		float d = (float)delta;
		Vector2 velocity = _parent.Velocity;

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

		if (_isDashing)
		{
			_parent.SetCollisionMaskValue(1, false);
			_parent.SetCollisionMaskValue(2, true);
			_parent.SetCollisionLayerValue(1, false);
			_parent.SetCollisionLayerValue(2, true);
		}
		else
		{
			_parent.SetCollisionMaskValue(1, true);
			_parent.SetCollisionMaskValue(2, false);
			_parent.SetCollisionLayerValue(1, true);
			_parent.SetCollisionLayerValue(2, false);
		}

		//Smoothing transition between a dash move and a regular move
		//so the player doesn't need to wait for the dash to end before he can move (which caused stops after dashes)
		Vector2 combined = velocity + _dashVelocity;
		if (combined.Length() > BaseSpeed + _dashVelocity.Length())
			combined = combined.Normalized() * (BaseSpeed + _dashVelocity.Length());
		velocity = combined;
		//

		_parent.Velocity = velocity;

		_parent.MoveAndSlide();
	}

	public void Dash(Vector2 direction)
	{
		if (direction == Vector2.Zero)
			return;

		_dashVelocity = direction.Normalized() * DashForce;
	}

	public void StartCharge()
	{
		_isCharging = true;
	}

	public void StopCharge()
	{
		_isCharging = false;
	}

	public void _DashCollisionLayer()
	{
		
	}
}