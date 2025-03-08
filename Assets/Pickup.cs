using UnityEngine;

public class Pickup : MonoBehaviour
{
    public KeyCode pickupKey = KeyCode.F;  // 줍기 키
    private bool canPickup = false;
    private Transform playerHand;
    private IPickupable pickupItem;
    private bool isPickedUp = false;  // 현재 아이템을 들고 있는지 여부

    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        pickupItem = GetComponentInChildren<IPickupable>();  // 자식에서 IPickupable 찾기
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            playerHand = player.transform.Find("Hand");
            if (playerHand == null)
            {
                Debug.LogError("❌ 'Hand' 오브젝트를 찾을 수 없음! 'Hand'가 'Player'의 자식인지 확인하세요.");
            }
        }
        else
        {
            Debug.LogError("❌ 'Player' 태그를 가진 오브젝트를 찾을 수 없음!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            Debug.Log("🔹 F 키 입력됨");

            if (isPickedUp)
            {
                DropItem();
            }
            else if (canPickup && playerHand != null)
            {
                PickupItem();
            }
        }
    }

    void PickupItem()
    {
        Debug.Log("🎯 PickupItem() 실행됨");

        pickupItem = GetComponentInChildren<IPickupable>();  // 자식에서도 찾기

        if (pickupItem == null)
        {
            Debug.LogError("❌ 주운 아이템이 IPickupable을 구현하지 않음! (" + gameObject.name + ")");
            return;
        }

        Debug.Log("✅ 주운 아이템: " + pickupItem.GetType().Name +
                  " (오브젝트: " + ((MonoBehaviour)pickupItem).gameObject.name + ")");

        pickupItem.OnPickup();

        if (transform.parent == null)
        {
            transform.SetParent(playerHand);
            transform.localPosition = new Vector3(0.3f, -0.2f, 0.8f);
            transform.localRotation = Quaternion.identity;

            // 아이템의 `localScale`을 (1, 1, 1)로 설정
            transform.localScale = Vector3.one;

            // 부모의 영향을 받지 않도록 worldScale을 고정
            transform.localScale = transform.lossyScale; // world scale을 그대로 설정
            isPickedUp = true;
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
        Debug.Log("🛑 DropItem() 실행됨!");

        if (!isPickedUp) return; // 이미 내려놓았으면 실행하지 않음

        isPickedUp = false;
        transform.SetParent(null);  // 부모 연결 해제

        // 🔥 바닥 감지 후, 안전한 위치에 놓기
        Vector3 dropPosition = FindSafeDropPosition(transform.position);
        transform.position = dropPosition;

        // 아이템의 `localScale`을 원래 크기 (1, 2.5, 0.2)로 설정
        transform.localScale = new Vector3(1f, 2.5f, 0.2f);

        if (col != null) col.enabled = true;  // 🟢 충돌 다시 활성화
        if (rb != null)
        {
            rb.isKinematic = false;  // 🟢 물리 활성화 (중력 받도록)
            rb.velocity = Vector3.zero;  // 🔄 이전 속도 제거
            rb.angularVelocity = Vector3.zero;  // 🔄 회전 속도 제거

            // 🔥 자연스럽게 떨어지도록 약한 힘 추가
            rb.AddForce(transform.forward * 0.2f + Vector3.down * 0.5f, ForceMode.Impulse);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickup = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickup = false;
        }
    }

    // 🔥 바닥 감지 후, 안전한 위치 찾기
    Vector3 FindSafeDropPosition(Vector3 startPosition)
    {
        RaycastHit hit;
        Vector3 newPosition = startPosition + Vector3.up * 0.5f; // 살짝 위에서 시작

        // 바닥 방향으로 Raycast 발사 (2미터 범위)
        if (Physics.Raycast(startPosition, Vector3.down, out hit, 2f))
        {
            newPosition = hit.point + Vector3.up * 0.2f; // 바닥보다 살짝 위로 조정
        }

        return newPosition;
    }
}
