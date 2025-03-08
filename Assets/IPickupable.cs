using UnityEngine;  // 🚀 Vector3를 사용하려면 필요함!

public interface IPickupable
{
    void OnPickup();  // 아이템 줍기
    void OnDrop(Vector3 dropPosition);  // 아이템 드롭 (위치 지정)
}
