using Godot;

public partial class Enemy : Character
{
	private Player playerNode;
	[Export] public int ChaseRadius;
	[Signal] public delegate void JustDiedEventHandler();
	private DirectionProvidingComponent DirectionProvider;

	public override void _Ready()
	{
		base._Ready();
		playerNode = GetParent().GetParent().GetNode<Player>("%Player");
		Connect("JustDied", new Callable(playerNode.GetNode<Hud>("CanvasLayer/HUD"), "OnEnemyDeath"));

		if (HasNode("ChaseComponent"))
		{
			var shape = (CircleShape2D)GetNode<CollisionShape2D>("ChaseComponent/Area2D/CollisionShape2D").Shape;
			shape.Radius = ChaseRadius;
			DirectionProvider = GetNode<DirectionProvidingComponent>("ChaseComponent");
		}

		//set different node responsible for movement


	}
	public override void _PhysicsProcess(double delta)
	{
		if (DirectionProvider != null)
		{
			Vector2 direction = DirectionProvider.ProvideDirection();
			_moveComponent.DesiredDirection = direction;
		}
	}

	public override void Die()
	{
		EmitSignal(SignalName.JustDied);
		base.Die();
	}
}
