using UnityEngine;
using UnityEngine.Events;

public class PlayerCombatSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Mana Settings")]
    public int maxMana = 50;
    public int currentMana;

    [Header("Combat Settings")]
    public int attackDamage = 10;
    public float attackRange = 1.0f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayers; // layers that contain enemyes

    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged;
    public UnityEvent<int, int> onManaChanged;
    public UnityEvent onPlayerDeath;


    private float lastAttackTime;
    private Animator animator;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
   private void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;

        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onManaChanged?.Invoke(currentMana, maxMana);

    }

    [ContextMenu("Attack")]
    public void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        animator?.SetTrigger("Attack");
    }


    // [ContextMenu("TakeDamage")]  hide from inspector
    public void TakeDamage(int damage)
    {
        
        currentHealth -= damage;

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log("TakeDamage");

        if(animator)
        {
            Debug.Log("Hurt");
            animator.SetTrigger("Hurt");
        }

        if(currentHealth <= 0)
        {
            Die();
        }

    }


    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }


    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);

        onManaChanged?.Invoke(currentMana, maxMana);
    }



    public bool UseMana(int amount)
    {
        if(currentMana >= amount)
        {

            currentMana -= amount;
            onManaChanged?.Invoke(currentMana, maxMana);
            return true;
        }

        return false;
    }



    private void Die()
    {
        animator?.SetTrigger("Die");

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController)
            playerController.enabled = false;

        onPlayerDeath?.Invoke();


    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
