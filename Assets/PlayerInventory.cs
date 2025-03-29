using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> inventoryItems = new List<GameObject>();
    public int maxSlots = 8;
    public Transform handTransform;
    private int equippedIndex = -1;

    public Transform playerCamera; // 카메라 기준으로 던지기
    public float throwForce = 5f;

    void Start()
    {
        if (handTransform == null)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
            {
                handTransform = player.Find("Hand");
                if (handTransform == null)
                    Debug.LogError("❌ 'Hand' 오브젝트를 찾을 수 없습니다.");
            }
        }

        if (playerCamera == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                playerCamera = mainCam.transform;
        }
    }

    void Update()
    {
        HandleScrollInput();
        HandleDropInput(); // 👈 G키 처리
    }

    void HandleScrollInput()
    {
        if (maxSlots <= 0) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            equippedIndex = (equippedIndex - 1 + maxSlots) % maxSlots;
        }
        else if (scroll < 0f)
        {
            equippedIndex = (equippedIndex + 1) % maxSlots;
        }


        UpdateEquippedItem();
    }


    void HandleDropInput()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            DropEquippedItem();
        }
    }

    void UpdateEquippedItem()
    {
        if (handTransform == null)
            return;

        // 1️⃣ 현재 손에 있는 모든 아이템 비활성화
        foreach (Transform child in handTransform)
        {
            child.gameObject.SetActive(false);
        }

        // 2️⃣ 선택된 슬롯이 비어 있지 않다면 아이템 활성화
        if (equippedIndex >= 0 && equippedIndex < inventoryItems.Count)
        {
            GameObject item = inventoryItems[equippedIndex];
            if (item != null)
            {
                // 3️⃣ 손 위치에 배치
                item.transform.SetParent(handTransform);
                item.transform.localPosition = new Vector3(0.3f, -0.2f, 0.8f);
                item.transform.localRotation = Quaternion.identity;
                item.SetActive(true); // ✅ GameObject 활성화

                // 🔥 추가: 비활성화된 스크립트 강제 활성화
                MonoBehaviour[] components = item.GetComponents<MonoBehaviour>();
                foreach (var comp in components)
                {
                    comp.enabled = true;
                }

                // 4️⃣ 다음 프레임에 OnPickup() 호출
                StartCoroutine(DelayedOnPickup(item));
            }
        }
    }



    private System.Collections.IEnumerator DelayedOnPickup(GameObject item)
    {
        yield return null; // 🔄 한 프레임 기다림 (SetParent, SetActive 등 적용 완료 후)

        IPickupable pickupable = item.GetComponentInChildren<IPickupable>();

        if (pickupable != null)
        {
            pickupable.OnPickup();
            pickupable.OnEquip();
            Debug.Log($"✅ OnPickup() + OnEquip() 호출 완료 → {item.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ IPickupable이 없음 → {item.name}");
        }
    }





    public void AddItem(GameObject item)
    {
        if (inventoryItems.Count >= maxSlots)
        {
            Debug.Log("📦 인벤토리가 가득 찼습니다!");
            return;
        }

        inventoryItems.Add(item);
        item.SetActive(false);

        if (inventoryItems.Count == 1)
        {
            equippedIndex = 0;
            UpdateEquippedItem();
        }

        Debug.Log($"📥 인벤토리에 추가됨: {item.name}");
    }

    public void RemoveItem(GameObject item)
    {
        if (inventoryItems.Contains(item))
        {
            inventoryItems.Remove(item);
            Debug.Log($"🗑 인벤토리에서 제거됨: {item.name}");

            if (equippedIndex >= inventoryItems.Count)
            {
                equippedIndex = -1;
                UpdateEquippedItem();
            }
        }
    }

    void DropEquippedItem()
    {
        if (equippedIndex < 0 || equippedIndex >= inventoryItems.Count) return;

        GameObject item = inventoryItems[equippedIndex];
        inventoryItems.RemoveAt(equippedIndex);

        // 다음 아이템 선택
        if (inventoryItems.Count == 0)
            equippedIndex = -1;
        else
            equippedIndex = equippedIndex % inventoryItems.Count;

        UpdateEquippedItem();

        item.transform.SetParent(null);
        item.transform.position = handTransform.position + playerCamera.forward * 1f;
        item.transform.rotation = Quaternion.identity;
        item.SetActive(true);

        // Rigidbody가 있다면 던지기
        Rigidbody rb = item.GetComponent<Rigidbody>();
        Collider col = item.GetComponent<Collider>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(playerCamera.forward * throwForce + Vector3.up * 2f, ForceMode.Impulse);
        }

        if (col != null)
        {
            col.enabled = true;
        }

        IPickupable pickupable = item.GetComponent<IPickupable>();
        pickupable?.OnDrop(item.transform.position);

        Debug.Log("🚮 아이템을 던졌습니다: " + item.name);
    }

    public int GetEquippedIndex()
    {
        return equippedIndex;
    }
    public GameObject GetEquippedItem()
    {
        if (equippedIndex >= 0 && equippedIndex < inventoryItems.Count)
        {
            return inventoryItems[equippedIndex];
        }
        return null;
    }

    public void EquipItem(GameObject item)
    {
        int index = inventoryItems.IndexOf(item);
        if (index != -1)
        {
            equippedIndex = index;
            UpdateEquippedItem();
        }
    }

    public bool IsHolding(GameObject item)
    {
        if (equippedIndex >= 0 && equippedIndex < inventoryItems.Count)
        {
            return inventoryItems[equippedIndex] == item;
        }
        return false;
    }


}
