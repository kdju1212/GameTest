using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    public PlayerInventory playerInventory; // �κ��丮 ����
    public RectTransform selector;          // ���� �׵θ� (SlotSelector)
    public RectTransform[] slotRects;       // ���� ��ġ�� (Slot_0 ~ Slot_8)

    void Update()
    {
        if (playerInventory == null || selector == null || slotRects == null || slotRects.Length == 0)
            return;

        int index = playerInventory.GetEquippedIndex(); // ���� ���� �ε���

        // ���� �˻�
        if (index >= 0 && index < slotRects.Length)
        {
            selector.position = slotRects[index].position;
        }
    }
}
