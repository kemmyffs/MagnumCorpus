using Godot;

public partial class Character : CharacterBody2D
{
    [Export] public float BaseSpeed = 200f;
    [Export] public int MaxHealth = 100;
    public MoveComponent _moveComponent;
    public HealthComponent _healthComponent;
    public CollisionShape2D MoveCollisionShape;

	public float Speed => BaseSpeed;

    public bool IsAlive { get; protected set; } = true;

	public Vector2 FacingDirection {get; protected set;}

    public override void _Ready()
    {
        _moveComponent = GetNode<MoveComponent>("MoveComponent");
        MoveCollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _healthComponent = GetNode<HealthComponent>("HealthComponent");
    }

}