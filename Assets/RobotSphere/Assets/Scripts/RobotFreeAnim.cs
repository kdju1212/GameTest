using UnityEngine;
using UnityEngine.AI;

public class RobotFreeAnim : MonoBehaviour
{
    Vector3 rot = Vector3.zero;
    float rotSpeed = 40f;
    Animator anim;
    NavMeshAgent agent;  // NavMeshAgent를 추가하여 AI가 이동하도록 설정
    Transform player;     // 플레이어 위치
    public float detectionRange = 10f; // 플레이어 추적 범위
    public float attackRange = 1.5f;   // 공격 범위
    public float attackCooldown = 1f;  // 공격 쿨타임
    private float lastAttackTime;      // 마지막 공격 시간

    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player")?.transform; // 플레이어를 찾습니다
    }

    void Update()
    {
        // 플레이어가 감지 범위 내에 있으면 추적
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                agent.SetDestination(player.position);  // 플레이어 추적

                // 공격 범위 내에 들어오면 공격
                if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
                {
                    Attack();
                }
                else
                {
                    anim.SetBool("Walk_Anim", true);  // 플레이어를 추적할 때 걷기 애니메이션
                }
            }
            else
            {
                anim.SetBool("Walk_Anim", false);  // 플레이어가 멀어지면 걷기 애니메이션 중지
            }
        }

        // 회전
        gameObject.transform.eulerAngles = rot;
    }

    // 공격 함수
    void Attack()
    {
        lastAttackTime = Time.time;

        anim.SetTrigger("Attack_Anim");  // 공격 애니메이션 실행
        // Raycast를 사용해 플레이어가 공격 범위 내에 있는지 확인하고 데미지 입히기
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // 플레이어에게 데미지 입히기
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(10);  // 예시로 10 데미지
                }
            }
        }
    }
}
