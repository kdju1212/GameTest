using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100; // 초기 체력
    public int maxHealth = 100; // 최대 체력
    public GameManager gameManager; // 게임 매니저 참조

    // 플레이어가 데미지를 받으면 호출되는 함수
    public void TakeDamage(int damage)
    {
        health -= damage; // 데미지만큼 체력 감소
        health = Mathf.Clamp(health, 0, maxHealth); // 체력은 0과 maxHealth 사이로 유지

        if (health <= 0)
        {
            Die(); // 체력이 0 이하일 경우 Die() 호출
        }
    }

    // 플레이어가 죽으면 호출되는 함수
    void Die()
    {
        Debug.Log("플레이어가 죽었습니다!");

        // 게임 오버 UI를 표시하거나 게임을 종료하는 등의 처리를 추가합니다.
        if (gameManager != null)
        {
            gameManager.GameOver(); // 게임 오버 처리
        }

        // 추가로 리스타트, 씬 전환 등의 처리를 할 수 있습니다.
        // 예: SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬을 리로드
    }

    // 플레이어의 체력을 회복시키는 함수
    public void Heal(int amount)
    {
        health += amount; // 체력 회복
        health = Mathf.Clamp(health, 0, maxHealth); // 체력은 0과 maxHealth 사이로 유지
    }

    // 체력 상태를 출력하는 디버그용 함수
    public void PrintHealth()
    {
        Debug.Log("플레이어 체력: " + health);
    }
}
