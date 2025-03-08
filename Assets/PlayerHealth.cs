using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100; // �ʱ� ü��
    public int maxHealth = 100; // �ִ� ü��
    public GameManager gameManager; // ���� �Ŵ��� ����

    // �÷��̾ �������� ������ ȣ��Ǵ� �Լ�
    public void TakeDamage(int damage)
    {
        health -= damage; // ��������ŭ ü�� ����
        health = Mathf.Clamp(health, 0, maxHealth); // ü���� 0�� maxHealth ���̷� ����

        if (health <= 0)
        {
            Die(); // ü���� 0 ������ ��� Die() ȣ��
        }
    }

    // �÷��̾ ������ ȣ��Ǵ� �Լ�
    void Die()
    {
        Debug.Log("�÷��̾ �׾����ϴ�!");

        // ���� ���� UI�� ǥ���ϰų� ������ �����ϴ� ���� ó���� �߰��մϴ�.
        if (gameManager != null)
        {
            gameManager.GameOver(); // ���� ���� ó��
        }

        // �߰��� ����ŸƮ, �� ��ȯ ���� ó���� �� �� �ֽ��ϴ�.
        // ��: SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���� ���� ���ε�
    }

    // �÷��̾��� ü���� ȸ����Ű�� �Լ�
    public void Heal(int amount)
    {
        health += amount; // ü�� ȸ��
        health = Mathf.Clamp(health, 0, maxHealth); // ü���� 0�� maxHealth ���̷� ����
    }

    // ü�� ���¸� ����ϴ� ����׿� �Լ�
    public void PrintHealth()
    {
        Debug.Log("�÷��̾� ü��: " + health);
    }
}
