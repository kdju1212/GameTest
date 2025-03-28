using UnityEngine;

public class Flashlight : MonoBehaviour, IPickupable
{
    private Light flashlight;
    private bool isPickedUp = false;
    public Transform playerCamera;
    public Transform player;
    private Vector3 originalScale;
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        flashlight = GetComponentInChildren<Light>();
        originalScale = transform.localScale;
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        if (playerCamera == null)
            playerCamera = Camera.main?.transform;
    }

    void Update()
    {
        // 좌클릭 시 손전등 켜기/끄기 (들고 있는 상태에서만 가능)
        if (transform.parent != null && Input.GetMouseButtonDown(0))
        {
            ToggleFlashlight();
        }
    }


    public void OnPickup()
    {
        isPickedUp = true;
        Transform hand = GameObject.FindWithTag("Player")?.transform.Find("Hand");
        flashlight.enabled = false;

        if (hand != null)
        {
            transform.SetParent(hand);
            transform.localPosition = new Vector3(0.3f, -0.2f, 0.8f);
            transform.localRotation = Quaternion.identity;
            transform.localScale = originalScale;
        }

        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public void OnDrop(Vector3 dropPosition)
    {
        isPickedUp = false;

        transform.SetParent(null);
        transform.position = dropPosition;
        transform.localScale = originalScale;
        flashlight.enabled = false;

        if (col != null) col.enabled = true;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
    //public void OnPickup()
    //{
    //    // 필요하면 실행 시점에 초기화할 내용 (예: 꺼진 상태)
    //    Debug.Log("🔦 손전등이 줍혔습니다!");
    //    flashlight.enabled = false;
    //    isPickedUp = false;
    //}

    //public void OnDrop(Vector3 dropPosition)
    //{

    //    // 손전등이 바닥에 떨어지면 자동 꺼짐
    //    flashlight.enabled = false;
    //    isPickedUp = false;
    //    Debug.Log("💡 손전등이 바닥에 떨어졌습니다!");
    //}

    void ToggleFlashlight()
    {
        isPickedUp = !isPickedUp;
        if (flashlight != null)
        {
            flashlight.enabled = isPickedUp;
            Debug.Log("🔦 손전등 상태: " + (isPickedUp ? "켜짐" : "꺼짐"));
        }
    }
}
