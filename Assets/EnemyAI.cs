using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType
    {
        InstantDeath,     // 플레이어와 닿으면 즉시 죽는 적
        TimedDeath,       // 플레이어와 3초 동안 닿으면 죽는 적
    }

    public EnemyType enemyType = EnemyType.InstantDeath;  // 적의 타입 설정 (Inspector에서 선택 가능)
    public Transform player;           // 플레이어의 위치
    private UnityEngine.AI.NavMeshAgent agent;
    private GameManager gameManager;
    private EnemyHealth enemyHealth;

    // 타이머 관련 변수
    private float timeInContact = 0f;    // 플레이어와 접촉한 시간
    private bool isInContact = false;    // 플레이어와 접촉 중 여부
    public float contactTimeForDeath = 3f; // 플레이어가 3초 동안 접촉하면 죽음

    // 공격 횟수
    public int maxHitsBeforeDeath = 3;
    private int currentHits = 0;

    private Rigidbody rb;  // 적의 Rigidbody
    private bool isAttachedToPlayer = false; // 적이 플레이어에 달라붙어 있는지 여부
    private Transform playerHead;  // 플레이어의 머리 위치

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>(); // GameManager 찾기
        enemyHealth = GetComponent<EnemyHealth>(); // 적의 체력 스크립트 찾기
        rb = GetComponent<Rigidbody>();
        playerHead = player.Find("Head");  // 플레이어의 머리 위치 찾기
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position); // 플레이어를 추적

            // 타이머와 충돌 처리
            if (enemyType == EnemyType.TimedDeath && isInContact)
            {
                timeInContact += Time.deltaTime; // 접촉 시간을 늘려가며
                if (timeInContact >= contactTimeForDeath)
                {
                    // 플레이어가 3초 동안 접촉하면 적 죽이기
                    gameManager.GameOver(); // 게임 오버 처리
                    PlayerDie();  // 플레이어 죽음
                }
            }

            // 적이 플레이어의 머리 위에 달라붙도록 처리
            if (isAttachedToPlayer)
            {
                transform.position = playerHead.position + new Vector3(0, 1.5f, 0); // 플레이어 머리 위 1.5m 높이에 위치시킴
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어와 충돌 시
            if (enemyType == EnemyType.InstantDeath)
            {
                // 즉시 죽이는 적
                gameManager.GameOver(); // 게임 오버 처리
                PlayerDie();  // 플레이어 죽음
            }
            else if (enemyType == EnemyType.TimedDeath)
            {
                // 3초 동안 닿아야 죽는 적
                isInContact = true;  // 플레이어와 접촉 시작
                timeInContact = 0f;  // 타이머 초기화
            }

            // 플레이어와 충돌 시 적을 플레이어의 머리 위에 붙이기
            isAttachedToPlayer = true;  // 적이 플레이어에 달라붙었다고 설정
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어와 접촉이 끝나면 타이머 초기화
            isInContact = false;
            timeInContact = 0f;
            isAttachedToPlayer = false; // 적이 플레이어와 떨어지면
            rb.isKinematic = false; // 물리적 힘을 다시 활성화
        }
    }

    // 다른 플레이어가 적을 때리면 떨어지게 하기
    public void TakeDamage(int damage)
    {
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        // 3번 맞으면 적이 죽음
        currentHits++;
        if (currentHits >= maxHitsBeforeDeath)
        {
            Die();
        }

        // 물리적으로 떨어지게 하기 (다른 플레이어가 때리면)
        if (isAttachedToPlayer)
        {
            rb.isKinematic = false; // 물리적인 영향을 받도록 설정
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse); // 위로 튕기게 설정
            isAttachedToPlayer = false; // 떨어졌으므로 더 이상 붙지 않음
        }
    }

    // 적이 죽을 때 처리
    void Die()
    {
        Debug.Log("적이 죽었습니다!");
        Destroy(gameObject); // 적 오브젝트 제거
    }

    void PlayerDie()
    {
        Debug.Log("플레이어가 죽었습니다!");
    }

}
