using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 100; // ���� ü��

    // ������ ������ �Լ�
    public void TakeDamage(int damage)
    {
        health -= damage; // ü�� ����
        Debug.Log($"���� {damage}��ŭ �������� ����! ���� ü��: {health}");

        if (health <= 0)
        {
            Die(); // ü���� 0 ���Ϸ� �������� ���� ó��
        }
    }

    void Die()
    {
        // ���� �׾��� �� ó���� ����
        Debug.Log("���� �׾����ϴ�!");
        Destroy(gameObject); // ���� ����
    }
}
