using UnityEngine;  // 🚀 Vector3를 사용하려면 필요함!

public interface IPickupable
{
    void OnPickup();
    void OnDrop(Vector3 dropPosition);
  
}

