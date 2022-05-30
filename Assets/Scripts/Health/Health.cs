using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Space(10)]
    [Header("References")]

    [Tooltip("Populate with the HealthBar component on the HealthBar gameobject")]
    [SerializeField] private HealthBar healthBar;

    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    private Player player;
    private Coroutine immunityCoroutine;
    private bool isImmuneAfterHit;
    private float immunityTime = 0f;
    private SpriteRenderer spriteRenderer = null;
    private const float spriteFlashInterval = 0.2f;
    private WaitForSeconds WaitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);

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

        if (player != null)
        {
            if (player.playerDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = player.playerDetails.hitImmunityTime;
                spriteRenderer = player.spriteRenderer;
            }
        }
        else if (enemy != null)
        {
            if (enemy.enemyDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = enemy.enemyDetails.hitImmunityTime;
                spriteRenderer = enemy.spriteRendererArray[0];
            }
        }

        if (enemy != null && enemy.enemyDetails.isHealthBarDisplayed == true && healthBar != null)
        {
            healthBar.EnableHealthBar();
        }
        else if (healthBar != null)
        {
            healthBar.DisableHealthBar();
        }
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

            PostHitImmunity();
        }

        if (healthBar != null)
        {
            healthBar.SetHealthBarValue((float)currentHealth / (float)startingHealth);
        }
    }

    /// <summary>
    /// Indicate a hit and give brief post hit immunity
    /// </summary>
    private void PostHitImmunity()
    {
        if (gameObject.activeSelf == false)
        {
            return;
        }

        if (isImmuneAfterHit)
        {
            if (immunityCoroutine != null)
            {
                StopCoroutine(immunityCoroutine);
            }

            immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immunityTime, spriteRenderer));
        }
    }

    /// <summary>
    /// Coroutine to indicate hit and give brief immunity
    /// </summary>
    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);

        isDamageable = false;

        while (iterations > 0)
        {
            spriteRenderer.color = Color.red;

            yield return WaitForSecondsSpriteFlashInterval;

            spriteRenderer.color = Color.white;

            yield return WaitForSecondsSpriteFlashInterval;

            iterations--;

            yield return null;
        }

        isDamageable = true;
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