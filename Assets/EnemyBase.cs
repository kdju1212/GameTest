using UnityEngine;
using UnityEngine.AI; // ✅ AI 이동을 위해 NavMeshAgent 사용

public class EnemyBase : MonoBehaviour
{
    public int health = 100; // ✅ 적의 체력
    public float speed = 3.5f; // ✅ 이동 속도
    public int damage = 10; // ✅ 공격 데미지
    public float detectionRange = 15f; // ✅ 플레이어 감지 범위
    public float attackRange = 2f; // ✅ 공격 가능 거리

    protected Transform player;
    protected NavMeshAgent agent;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player")?.transform;
        agent.speed = speed;
    }

    protected virtual void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            agent.SetDestination(player.position);
        }
    }

    // ✅ 체력 관리 (EnemyHealth와 동일한 기능)
    public virtual void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log($"🔴 {gameObject.name}이 {damageAmount}만큼 데미지를 받음! 남은 체력: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"💀 {gameObject.name} 사망!");
        Destroy(gameObject);
    }
}
