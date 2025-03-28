using UnityEngine;

public class Pickup : MonoBehaviour
{
    public KeyCode pickupKey = KeyCode.F;
    private Transform player;
    private PlayerInventory inventory;
    private IPickupable pickupItem;
    private float pickupRange = 5f;

    void Start()
    {
        pickupItem = GetComponent<IPickupable>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj)
        {
            player = playerObj.transform;
            inventory = playerObj.GetComponent<PlayerInventory>();
        }

        if (player == null || inventory == null)
        {
            Debug.LogError("❌ 플레이어나 인벤토리 컴포넌트를 찾을 수 없음");
        }
    }

    void Update()
    {
        if (player == null || inventory == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (Input.GetKeyDown(pickupKey) && distance <= pickupRange)
        {
            if (inventory.IsHolding(this.gameObject))
            {
                Debug.Log("❌ 이미 손에 든 아이템입니다.");
                return; // 이미 손에 들고 있으면 다시 줍지 않음
            }

            PickupItem();
        }
    }


    void PickupItem()
    {
        Debug.Log("🎯 PickupItem() 실행됨");

        if (pickupItem == null)
        {
            pickupItem = GetComponentInChildren<IPickupable>();
            if (pickupItem == null)
            {
                Debug.LogError("❌ IPickupable이 없음!");
                return;
            }
        }

        // ✅ 중복 체크
        if (inventory.inventoryItems.Contains(this.gameObject))
        {
            Debug.LogWarning("⚠️ 이미 인벤토리에 존재하는 아이템입니다!");
            return;
        }

        pickupItem.OnPickup();

       

        inventory?.AddItem(this.gameObject);
        Debug.Log("✅ 인벤토리에 아이템 추가 완료!");
    }


}
