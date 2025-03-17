using System.Collections; // ✅ IEnumerator를 사용하기 위해 추가
using UnityEngine;

public class Shovel : MonoBehaviour, IPickupable
{
    private bool isPickedUp = false;
    public Transform playerCamera;
    public Transform player;
    public float pickupRange = 2f;
    public float attackRange = 5f;
    public int attackDamage = 10;
    public float attackCooldown = 0.5f;
    private float lastAttackTime;
    private bool canAttack = true; // ✅ 클릭 한 번에 1회 공격 제한

    private Vector3 originalScale;

    private Rigidbody rb;
    private Collider col;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main?.transform;
        }
    }

    void Update()
    {
        // F 키 눌렀을 때 삽 줍기/놓기
        if (Input.GetKeyDown(KeyCode.F))
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (isPickedUp)
            {
                DropItem();
            }
            else if (distance <= pickupRange)
            {
                PickupItem();
            }
        }

        // ✅ 클릭 한 번에 1번만 공격 실행
        if (isPickedUp && Input.GetMouseButtonDown(0) && canAttack)
        {
            Attack();
        }
    }

    public void OnPickup()
    {
        PickupItem();
    }

    public void OnDrop(Vector3 dropPosition)
    {
        DropItem();
    }

    void PickupItem()
    {
        isPickedUp = true;
        Transform playerHand = GameObject.FindWithTag("Player")?.transform.Find("Hand");
        originalScale = transform.localScale;

        if (playerHand != null)
        {
            transform.SetParent(playerHand);
            transform.localPosition = new Vector3(0.3f, -0.2f, 0.8f);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = originalScale;
        }

        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void DropItem()
    {
        isPickedUp = false;
        transform.SetParent(null);
        transform.position = player.position + player.forward * 1f;
        transform.localScale = originalScale;

        if (col != null) col.enabled = true;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;

        // ✅ 공격 실행 중 추가 입력 방지
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
        

        if (animator != null && isPickedUp)
        {
            animator.SetTrigger("Attack"); // ✅ 애니메이션 실행 (이후 이벤트에서 `DealDamage()` 호출됨)
        }
    }

    // ✅ 공격 가능 상태로 복구하는 함수
    void ResetAttack()
    {
        canAttack = true;
    }

    public void DealDamage()
    {
        if (playerCamera == null)
        {
            Debug.LogError("❌ 플레이어 카메라가 없습니다!");
            return;
        }

        RaycastHit hit;
        Vector3 rayOrigin = playerCamera.position;
        Vector3 rayDirection = playerCamera.forward;

        Debug.DrawRay(rayOrigin, rayDirection * attackRange, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage); // ✅ `EnemyBase`에서 체력 관리
                }

                Rigidbody enemyRb = hit.collider.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    StartCoroutine(FreezeEnemy(enemyRb, 0.5f)); // ✅ 0.5초 동안 적 멈춤
                }
            }
        }
    }

    // ✅ 적을 일정 시간 동안 멈추게 하는 함수
    private IEnumerator FreezeEnemy(Rigidbody enemyRb, float freezeDuration)
    {
        if (enemyRb == null) yield break; // ✅ 적이 이미 삭제되었다면 코루틴 종료

        enemyRb.isKinematic = true; // ✅ 적을 물리적으로 멈춤

        yield return new WaitForSeconds(freezeDuration);

        if (enemyRb != null) // ✅ 적이 아직 존재하면 다시 움직이게 설정
        {
            enemyRb.isKinematic = false;
        }
    }
}
