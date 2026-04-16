using Godot;
using System;
using System.Reflection;

public partial class TrainingDummy : Character
{
    bool falseDied = false;
    [Signal] public delegate void OnDeathEventHandler();
    [Signal] public delegate void OnFalseDeathEventHandler();
    public override void Die()
    {
        if(!falseDied)
        {
            EmitSignal("OnFalseDeath");
            _healthComponent.Heal(5);
            falseDied = true;
        } else
        {
            EmitSignal("OnDeath");
            base.Die();
        }
        
    }

}
