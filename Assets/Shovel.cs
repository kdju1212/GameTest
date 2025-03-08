using UnityEngine;

public class Shovel : MonoBehaviour, IPickupable
{
    private bool isPickedUp = false;
    public Transform playerCamera;
    public Transform player;
    public float pickupRange = 2f; // ✅ 줍기 가능 거리
    public float attackRange = 5f; // ✅ 공격 거리
    public int attackDamage = 10; // ✅ 공격 데미지
    public float attackCooldown = 0.5f; // ✅ 공격 속도
    private float lastAttackTime;
    public Transform shovelTransform; // 삽의 위치
    private Vector3 originalScale;  // 아이템의 원래 스케일 (변경 부분)

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

        if (shovelTransform == null)  // shovelTransform이 할당되지 않았다면 경고
        {
            Debug.LogWarning("Shovel Transform이 할당되지 않았습니다! 삽의 Transform을 할당해주세요.");
        }

    }

    void Update()
    {
        // F 키 눌렀을 때 삽 줍기/놓기
        if (Input.GetKeyDown(KeyCode.F))
        {
            float distance = Vector3.Distance(transform.position, player.position);
            Debug.Log($"📏 플레이어와 삽 거리: {distance}");

            if (isPickedUp)
            {
                DropItem();
            }
            else if (distance <= pickupRange)
            {
                PickupItem();
            }
            else
            {
                Debug.Log("❌ 너무 멀어서 줍기 불가능!");
            }
        }

        // 삽을 들고 있을 때만 좌클릭하면 공격 애니메이션 실행
        if (isPickedUp && Input.GetMouseButtonDown(0) && Time.time > lastAttackTime + attackCooldown)
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
        originalScale = transform.localScale;  // 아이템의 원래 크기 저장

        if (playerHand != null)
        {
            transform.SetParent(playerHand);
            transform.localPosition = new Vector3(0.3f, -0.2f, 0.8f);
            transform.localScale = new Vector3(1f, 2.5f, 0.2f);  // 원래 크기로 설정
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = originalScale;  // 원래 크기 복원


        }

        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.isKinematic = true;   // 물리적인 영향을 받지 않도록 설정
            rb.useGravity = false;   // 중력 비활성화
        }
    }


    void DropItem()
    {
        isPickedUp = false;
        transform.SetParent(null);
        transform.position = player.position + player.forward * 1f; // 앞쪽에 놓기

        // 아이템의 `localScale`을 원래 크기로 설정
        // transform.localScale = new Vector3(1f, 2.5f, 0.2f);  // 원래 크기로 설정
        transform.localScale = originalScale;  // 원래 크기 복원

        if (col != null) col.enabled = true;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return; // 쿨다운 시간 초과 시에만 공격 실행
        lastAttackTime = Time.time;

        Debug.Log("🪓 삽 공격!");

        // 공격 애니메이션 실행
        if (animator != null && isPickedUp)
        {
            animator.SetTrigger("Attack"); // Attack 트리거로 애니메이션 실행
        }
    }

    public void DealDamage()
    {
        // Raycast 발사 위치를 shovelTransform.position으로 변경
        RaycastHit hit;
        if (Physics.Raycast(shovelTransform.position, shovelTransform.forward, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Enemy")) // 적을 맞추면
            {
                Debug.Log($"💥 적 타격! {hit.collider.gameObject.name}을 맞췄습니다!");

                // 적의 health를 감소시키는 TakeDamage() 함수 호출
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage); // 데미지 입히기
                }
            }
        }
    }
}
