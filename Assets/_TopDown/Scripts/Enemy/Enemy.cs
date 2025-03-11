using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float waitTime = 1f;

    private int currentPointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if(isWaiting)
        {
            waitTimer -= Time.deltaTime; // can be Time.time, just a personal choice

            if(waitTimer <=0)
            {
                isWaiting = false;
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            }
            return;
        }

        Vector2 targetPosition = patrolPoints[currentPointIndex].position;

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        rb.linearVelocity = direction * moveSpeed;

        if(direction.x !=0)
        {
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1,1);
        }

        if (Vector2.Distance(transform.position,targetPosition) < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;

            isWaiting = true;

            waitTimer = waitTime;
        }


    }








}
