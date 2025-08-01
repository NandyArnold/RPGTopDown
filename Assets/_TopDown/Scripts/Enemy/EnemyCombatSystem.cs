using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
public class EnemyCombatSystem : MonoBehaviour
{
    
    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth { get; private set; }

    
    [Header("Combat Settings")]
    public int attackDamage = 5;
    public float attackRange = 1.0f;
    public float attackCooldown = 1.0f;
    public float attackFrontDistance = 1.2f;
    public LayerMask playerLayer;


    [Header("Reward Settings")]
    public int experienceReward = 10;
    public GameObject[] possibleDrops;
    [Range(0,1)]
    public float dropChance = 0.3f;


    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged;
    public UnityEvent onEnemyDeath;


    private float lastAttackTime;
    private Animator animator;
    private Enemy enemy;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
    }



    private void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        animator?.SetTrigger("Hurt");

        if(currentHealth <=0)
        {
            Die();
        }
    }


    public void Attack(PlayerCombatSystem player)
    {
        if (Time.time - lastAttackTime < attackCooldown || player.currentHealth <= 0)
            return;

        lastAttackTime = Time.time;

        animator?.SetTrigger("Attack");

    

    }


    private void Die()
    {

        animator?.SetTrigger("Die");

        if(enemy != null)
        {
            enemy.enabled = false;
        }

        onEnemyDeath?.Invoke();

        if(QuestManager.Instance != null)
        {
            QuestManager.Instance.EnemyKilled();
        }

        PlayerCombatSystem player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerCombatSystem>();

        if(player !=null)
        {
            Debug.Log("Player received " + experienceReward + " experience points!");
        }

        DropItem();

        Destroy(gameObject, 2.0f);
    }



    private void DropItem()
    {
        if (possibleDrops.Length == 0 || Random.value > dropChance)
            return;

        int randomIndex = Random.Range(0, possibleDrops.Length);

        GameObject drop = possibleDrops[randomIndex];

        if(drop != null)
        {
            Instantiate(drop, transform.position, Quaternion.identity);
        }
    }


    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);

    }

    public void OnAttackEvent()
    {
        Vector2  attackPosition = transform.position;

        bool isFacingRight = transform.localScale.x < 0;

        if(isFacingRight)
        {
            attackPosition.x += attackFrontDistance;
        }
        else
        {
            attackPosition.x -= attackFrontDistance;
        }
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPosition, attackRange, playerLayer);

        foreach (Collider2D player in hitPlayer)
        {
            PlayerCombatSystem playerCombatSystem = player.GetComponent<PlayerCombatSystem>();
            if(playerCombatSystem != null)
            {
                playerCombatSystem.TakeDamage(attackDamage);
            }
        }



    }
}
