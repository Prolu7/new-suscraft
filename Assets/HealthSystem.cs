using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] int maxHealth = 20;
    int _health;
    int health
    {
        set { _health = Mathf.Clamp(value, 0, maxHealth); }
        get { return _health; }
    }
    
    public delegate void DamageEventHandler(object sender, DamageEventArgs e);
    public event DamageEventHandler DamageEvent;

    public delegate void DeathEventHandler(object sender, DeathEventArgs e);
    public event DeathEventHandler DeathEvent;

    private void Awake()
    {
        health = maxHealth;
    }
    public void Damage(int damage, GameObject instigator)
    {
        health -= damage;
        DamageEvent?.Invoke(this, new DamageEventArgs(damage, instigator));
        if (!IsAlive())
            DeathEvent?.Invoke(this, new DeathEventArgs(instigator));
    }
    public bool IsAlive() 
    { 
        return health > 0; 
    }
}

public class DamageEventArgs
{
    public DamageEventArgs(int damage, GameObject instigator) 
                                    { Damage = damage; Instigator = instigator; }
    public int Damage { get; }
    public GameObject Instigator { get; }
}
public class DeathEventArgs
{
    public DeathEventArgs(GameObject instigator) 
                                    { Instigator = instigator; }
    public GameObject Instigator { get; }
}