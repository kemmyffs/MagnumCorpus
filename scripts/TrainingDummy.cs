using Godot;
using System;

public partial class TrainingDummy : Character
{
    [Signal] public delegate void OnDeathEventHandler();
    public override void Die()
    {
        EmitSignal("OnDeath");
        base.Die();
    }

}
