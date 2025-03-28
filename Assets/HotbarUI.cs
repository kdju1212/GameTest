using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    public PlayerInventory playerInventory; // 인벤토리 참조
    public RectTransform selector;          // 선택 테두리 (SlotSelector)
    public RectTransform[] slotRects;       // 슬롯 위치들 (Slot_0 ~ Slot_8)

    void Update()
    {
        if (playerInventory == null || selector == null || slotRects == null || slotRects.Length == 0)
            return;

        int index = playerInventory.GetEquippedIndex(); // 현재 선택 인덱스

        // 범위 검사
        if (index >= 0 && index < slotRects.Length)
        {
            selector.position = slotRects[index].position;
        }
    }
}
