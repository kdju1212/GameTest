using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType
    {
        InstantDeath,     // 플레이어와 닿으면 즉시 죽는 적
        TimedDeath,       // 플레이어와 3초 동안 닿으면 죽는 적
    }

    public EnemyType enemyType = EnemyType.InstantDeath;  // 적의 타입 설정 (Inspector에서 선택 가능)
    public Transform player; // 플레이어의 위치
    private NavMeshAgent agent;
    private GameManager gameManager;
    private EnemyBase enemyBase; // ✅ EnemyBase 사용
    private Rigidbody rb;

    // 타이머 관련 변수
    private float timeInContact = 0f;
    private bool isInContact = false;
    public float contactTimeForDeath = 3f;

    // 공격 횟수
    public int maxHitsBeforeDeath = 3;
    private int currentHits = 0;

    private bool isAttachedToPlayer = false;
    private Transform playerHead;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>();
        enemyBase = GetComponent<EnemyBase>(); // ✅ EnemyBase 연결
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

        if (player != null)
        {
            playerHead = player.Find("Head");
        }
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);

            if (enemyType == EnemyType.TimedDeath && isInContact)
            {
                timeInContact += Time.deltaTime;
                if (timeInContact >= contactTimeForDeath)
                {
                    gameManager.GameOver();
                    PlayerDie();
                }
            }

            if (isAttachedToPlayer)
            {
                transform.position = playerHead.position + new Vector3(0, 1.5f, 0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyType == EnemyType.InstantDeath)
            {
                gameManager.GameOver();
                PlayerDie();
            }
            else if (enemyType == EnemyType.TimedDeath)
            {
                isInContact = true;
                timeInContact = 0f;
            }

            isAttachedToPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInContact = false;
            timeInContact = 0f;
            isAttachedToPlayer = false;
            rb.isKinematic = false;
        }
    }

    // ✅ 데미지 처리: EnemyBase에서 체력 관리
    public void TakeDamage(int damage)
    {
        if (enemyBase != null)
        {
            enemyBase.TakeDamage(damage); // ✅ EnemyBase에서 체력 감소 처리
        }

        currentHits++;
        if (currentHits >= maxHitsBeforeDeath)
        {
            Die();
        }

        if (isAttachedToPlayer)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            isAttachedToPlayer = false;
        }
    }

    void Die()
    {
        Debug.Log("적이 죽었습니다!");
        Destroy(gameObject);
    }

    void PlayerDie()
    {
        Debug.Log("플레이어가 죽었습니다!");
    }
}
