using UnityEngine;

public class Flashlight : MonoBehaviour, IPickupable
{
    private Light flashlight;
    private bool isOn = false;
    private bool isPickedUp = false;
    public Transform playerCamera;
    public Transform player;  // 🎯 플레이어 위치 참조

    private Rigidbody rb;
    private Collider col;
    private MeshRenderer meshRenderer;

    public float pickupRange = 2f; // ✅ 줍기 가능 거리 (2m)

    private Vector3 originalPosition;  // 아이템의 원래 월드 위치
    private Quaternion originalRotation;  // 아이템의 원래 월드 회전
    private Vector3 originalScale;  // 아이템의 원래 스케일 (변경 부분)

    void Start()
    {
        flashlight = GetComponentInChildren<Light>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (playerCamera == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerCamera = player.transform.Find("Main Camera");
            }

            if (playerCamera == null)
            {
                Debug.LogError("❌ 'Main Camera'를 찾을 수 없음! 'playerCamera'를 수동으로 설정하세요.");
            }
        }

        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("❌ 'Player' 태그가 설정된 오브젝트를 찾을 수 없음!");
            }
        }
    }

    void Update()
    {
        if (isPickedUp)
        {
            // ✅ 손전등이 플레이어의 시야를 따라가도록 설정
            transform.rotation = playerCamera.rotation;

            // 좌클릭하면 손전등 On/Off
            if (Input.GetMouseButtonDown(0))
            {
                ToggleFlashlight();
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            float distance = Vector3.Distance(transform.position, player.position);
            Debug.Log($"📏 플레이어와 손전등 거리: {distance}");

            if (isPickedUp)
            {
                Debug.Log("🛑 손전등을 버리려 합니다!");
                DropItem();
            }
            else if (distance <= pickupRange)
            {
                Debug.Log("🎯 손전등을 주우려 합니다!");
                PickupItem();
            }
            else
            {
                Debug.Log("❌ 너무 멀어서 줍기 불가능!");
            }
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
        Debug.Log("✅ PickupItem() 실행됨!");

        isPickedUp = true;
        Transform playerHand = GameObject.FindWithTag("Player")?.transform.Find("Hand");

        if (playerHand != null)
        {
            // 아이템의 원래 위치, 회전, 크기 저장
            originalPosition = transform.position;  // 아이템의 원래 월드 위치 저장
            originalRotation = transform.rotation;  // 아이템의 원래 월드 회전 저장
            originalScale = transform.localScale;  // 아이템의 원래 크기 저장

            transform.SetParent(playerHand);
            transform.localPosition = new Vector3(0.3f, -0.2f, 0.8f);
            transform.localRotation = Quaternion.Euler(0, 0, 0);

            // 아이템의 크기를 (1, 1, 1)로 초기화하여 부모 손의 크기 영향을 받지 않도록 설정
            // transform.localScale = Vector3.one;  // 부모의 영향을 받지 않도록 (1, 1, 1)로 설정
            transform.localScale = originalScale;  // 원래 크기 복원
        }
        else
        {
            Debug.LogError("❌ Hand 오브젝트를 찾을 수 없음!");
        }

        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }
    }

    void DropItem()
    {
        Debug.Log("🛑 DropItem() 실행됨!");

        isPickedUp = false;
        transform.SetParent(null);

        // 아이템의 `localScale`을 원래 크기로 설정
        transform.localScale = originalScale;  // 원래 크기 복원

        if (col != null) col.enabled = true;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(transform.forward * 0.2f + Vector3.down * 0.5f, ForceMode.Impulse);
        }

        if (flashlight != null)
        {
            isOn = false;
            flashlight.enabled = false;
        }
    }

    void ToggleFlashlight()
    {
        isOn = !isOn;
        if (flashlight != null)
        {
            flashlight.enabled = isOn;
            Debug.Log("🔦 손전등 상태: " + (isOn ? "켜짐" : "꺼짐"));
        }
    }

    Vector3 FindSafeDropPosition(Vector3 startPosition)
    {
        RaycastHit hit;
        Vector3 newPosition = startPosition + Vector3.up * 0.5f;

        if (Physics.Raycast(startPosition, Vector3.down, out hit, 2f))
        {
            newPosition = hit.point + Vector3.up * 0.2f;
        }

        return newPosition;
    }
}
