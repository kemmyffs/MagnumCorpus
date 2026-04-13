using System.Xml.Serialization;
using Godot;

public partial class Character : CharacterBody2D
{
    [ExportGroup("Health")]
    [Export] public int MaxHealth = 100;
    [ExportGroup("Movement")]
    [Export] public float BaseSpeed = 200f;
    [Export] public int ChargesAmountInFullBar = 5;
    public MoveComponent _moveComponent;
    public HealthComponent _healthComponent;
    public AttackComponent _attackComponent;
    public CollisionShape2D MoveCollisionShape;
	public float Speed => BaseSpeed;
    public bool IsAlive { get; protected set; } = true;
	public Vector2 FacingDirection {get; protected set;}

    public override void _Ready()
    {
        _moveComponent = GetNode<MoveComponent>("MoveComponent");
        _healthComponent = GetNode<HealthComponent>("HealthComponent");
        _attackComponent = GetNode<AttackComponent>("AttackComponent");
        
        MoveCollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public virtual void Die()
    {
        
        QueueFree();
    }

}