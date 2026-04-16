using System.Collections.Generic;
using Godot;


/*

ff66fa – Borůvková

*/
namespace Flavors
{
    public partial class Flavor : Resource
    {
        public string Description { get; set; }
        public Color FlavorColor { get; set; }
        public List<FlavorEffect> Effects { get; set; } = [];

        public Flavor(string description, Color color)
        {
            Description = description;
            FlavorColor = color;
        }

        public void AddEffect(FlavorEffect effect)
        {
            Effects.Add(effect);
        }

        public void Apply(Node target)
        {
            foreach (FlavorEffect effect in Effects)
            {
                effect.Apply(target);
            }
        }
    }

    public partial class FlavorEffect : Resource
    {
        public virtual void Apply(Node target) { }
    }

    public partial class HealthBoost : FlavorEffect
    {

    }

    public partial class AttackBoost : FlavorEffect
    {

    }
}