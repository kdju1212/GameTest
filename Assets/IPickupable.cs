using UnityEngine;  // 🚀 Vector3를 사용하려면 필요함!

public interface IPickupable
{
    void OnPickup();
    void OnDrop(Vector3 dropPosition);
    void OnEquip();     // 🔥 손에 들었을 때 실행
    void OnUnequip();   // 🔥 손에서 내려놨을 때 실행
}

