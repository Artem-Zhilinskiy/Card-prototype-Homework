using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private LinkedList<BaseEffect> _effects = new LinkedList<BaseEffect>();

    [SerializeField]
    private int _defaultHealth = 1;
    [SerializeField]
    private int _defaultDamage = 0;

    public int CurrentHealth { get; set; }
    public int CurrentDamage { get; set; }

    public void AddEffect(BaseEffect effect)
    {
        _effects.AddLast(effect);
        effect.SetEffect(this);
	}
    
    public bool TryToRemoveEffect(BaseEffect effect)
    {
        if (!_effects.Contains(effect)) return false;
        _effects.Remove(effect);

        return effect.TryToRemoveEffect(this);
	}

	private void Awake()
	{
        CurrentHealth = _defaultHealth;
        CurrentDamage = _defaultDamage;
	}
}

public abstract class BaseEffect
{
    public string Name { get; }
    public Card Parent { get; }
    public bool Permanent { get; }

    public BaseEffect(Card parent, bool permanent, string name = "")
    {
        Parent = parent; Permanent = permanent; Name = name;
	}

    public abstract void SetEffect(Card target);
    public abstract bool TryToRemoveEffect(Card target);
}

public class StatsEffect : BaseEffect
{
    public int Damage { get; }
    public int Health { get; }
    public StatsEffect(int damage, int health, Card parent, bool permanent, string name = "") : base(parent, permanent, name)
    {
        Damage = damage; Health = health;
	}

	public override void SetEffect(Card target)
	{
        target.CurrentHealth += Health;
        target.CurrentDamage += Damage;
	}

	public override bool TryToRemoveEffect(Card target)
	{
        if (Permanent) return false;

        target.CurrentHealth -= Health;
        target.CurrentDamage -= Damage;

        return true;
	}
}

