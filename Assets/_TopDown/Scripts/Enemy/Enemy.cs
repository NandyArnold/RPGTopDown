using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitTime = 1f;

    [Header("Detection Settings")]
    public Transform playerTransform;
    public float detectionRadius = 4f;
    public float  chaseSpeed = 2.5f;
    public float chaseTime = 4f;
    public LayerMask obstacleLayer;



    private int currentPointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private Rigidbody2D rb;

    private bool isCHasing = false;
    private float chaseTimer = 0f;
    private float currentSpeed;

    private enum EnemyState
    {
        Patrol,
        Chase
    }

    private EnemyState currentState = EnemyState.Patrol;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = patrolSpeed;
        if(playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

   
    private void Start()
    {
        if(patrolPoints.Length ==0)
        {
            Debug.LogWarning("No patrol points set for " + gameObject.gameObject.name);
            enabled = false;
        }
        
    }


    private void Update()
    {
        if (!playerTransform)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if(distanceToPlayer <= detectionRadius)
        {
            bool hasLineOfSight = !Physics2D.Raycast(transform.position, (playerTransform.position - transform.position).normalized, distanceToPlayer, obstacleLayer);

            if(hasLineOfSight)
            {
                currentState = EnemyState.Chase;
                isCHasing = true;
                chaseTimer = chaseTime;

            }

        }

        if(isCHasing && currentState == EnemyState.Chase && distanceToPlayer > detectionRadius)
        {
            chaseTimer -= Time.deltaTime;

            if(chaseTimer <= 0)
            {
                isCHasing = false;
                currentState = EnemyState.Patrol;
            }

        }

        switch(currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;

            case EnemyState.Chase:
                HandleChase();
                break;

        }


    }





    private void HandlePatrol()
    {
        currentSpeed = patrolSpeed;

        if (patrolPoints.Length == 0)
            return;

        if(isWaiting)
        {
            waitTimer -= Time.deltaTime; // can be Time.time, just a personal choice

            if(waitTimer <=0)
            {
                isWaiting = false;
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            }
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetPosition = patrolPoints[currentPointIndex].position;

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        rb.linearVelocity = direction * currentSpeed; // patrolSpeed before ???

        if(direction.x !=0)
        {
            transform.localScale = new Vector3(direction.x < 0 ? 1 : -1,1);
        }

        if (Vector2.Distance(transform.position,targetPosition) < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;

            isWaiting = true;

            waitTimer = waitTime;
        }


    }


    private void HandleChase()
    {
        currentSpeed = chaseSpeed;

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        rb.linearVelocity = direction * currentSpeed;

        if(direction.x !=0)
        {
            transform.localScale = new Vector3(direction.x < 0 ?1 : -1,1);
        }

    }

    private void OnDrawnGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

    }


}
