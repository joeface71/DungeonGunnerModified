using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    private Player player;

    [HideInInspector] public Enemy enemy;
    [HideInInspector] public bool isDamageable = true;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        CallHealthEvent(0);

        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
    }

    public void TakeDamage(int damageAmount)
    {
        bool isRolling = false;

        if (player != null)
        {
            isRolling = player.playerControl.isPlayerRolling;
        }

        if (isDamageable && !isRolling)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);
        }

        if (isDamageable && isRolling)
        {
            Debug.Log("Dodged");
        }
    }

    private void CallHealthEvent(int damageAmount)
    {
        healthEvent.CallHealthChangedEvent(((float)currentHealth / (float)startingHealth), currentHealth, damageAmount);
    }

    public void SetStaringHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    public int GetStartingHealth()
    {
        return startingHealth;
    }
}