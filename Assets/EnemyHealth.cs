using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 100; // 적의 체력

    // 데미지 입히기 함수
    public void TakeDamage(int damage)
    {
        health -= damage; // 체력 감소
        Debug.Log($"적이 {damage}만큼 데미지를 받음! 남은 체력: {health}");

        if (health <= 0)
        {
            Die(); // 체력이 0 이하로 떨어지면 죽음 처리
        }
    }

    void Die()
    {
        // 적이 죽었을 때 처리할 로직
        Debug.Log("적이 죽었습니다!");
        Destroy(gameObject); // 적을 제거
    }
}
