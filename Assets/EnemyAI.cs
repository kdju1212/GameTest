using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType
    {
        InstantDeath,     // �÷��̾�� ������ ��� �״� ��
        TimedDeath,       // �÷��̾�� 3�� ���� ������ �״� ��
    }

    public EnemyType enemyType = EnemyType.InstantDeath;  // ���� Ÿ�� ���� (Inspector���� ���� ����)
    public Transform player;           // �÷��̾��� ��ġ
    private UnityEngine.AI.NavMeshAgent agent;
    private GameManager gameManager;
    private EnemyHealth enemyHealth;

    // Ÿ�̸� ���� ����
    private float timeInContact = 0f;    // �÷��̾�� ������ �ð�
    private bool isInContact = false;    // �÷��̾�� ���� �� ����
    public float contactTimeForDeath = 3f; // �÷��̾ 3�� ���� �����ϸ� ����

    // ���� Ƚ��
    public int maxHitsBeforeDeath = 3;
    private int currentHits = 0;

    private Rigidbody rb;  // ���� Rigidbody
    private bool isAttachedToPlayer = false; // ���� �÷��̾ �޶�پ� �ִ��� ����
    private Transform playerHead;  // �÷��̾��� �Ӹ� ��ġ

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>(); // GameManager ã��
        enemyHealth = GetComponent<EnemyHealth>(); // ���� ü�� ��ũ��Ʈ ã��
        rb = GetComponent<Rigidbody>();
        playerHead = player.Find("Head");  // �÷��̾��� �Ӹ� ��ġ ã��
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position); // �÷��̾ ����

            // Ÿ�̸ӿ� �浹 ó��
            if (enemyType == EnemyType.TimedDeath && isInContact)
            {
                timeInContact += Time.deltaTime; // ���� �ð��� �÷�����
                if (timeInContact >= contactTimeForDeath)
                {
                    // �÷��̾ 3�� ���� �����ϸ� �� ���̱�
                    gameManager.GameOver(); // ���� ���� ó��
                    PlayerDie();  // �÷��̾� ����
                }
            }

            // ���� �÷��̾��� �Ӹ� ���� �޶�ٵ��� ó��
            if (isAttachedToPlayer)
            {
                transform.position = playerHead.position + new Vector3(0, 1.5f, 0); // �÷��̾� �Ӹ� �� 1.5m ���̿� ��ġ��Ŵ
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾�� �浹 ��
            if (enemyType == EnemyType.InstantDeath)
            {
                // ��� ���̴� ��
                gameManager.GameOver(); // ���� ���� ó��
                PlayerDie();  // �÷��̾� ����
            }
            else if (enemyType == EnemyType.TimedDeath)
            {
                // 3�� ���� ��ƾ� �״� ��
                isInContact = true;  // �÷��̾�� ���� ����
                timeInContact = 0f;  // Ÿ�̸� �ʱ�ȭ
            }

            // �÷��̾�� �浹 �� ���� �÷��̾��� �Ӹ� ���� ���̱�
            isAttachedToPlayer = true;  // ���� �÷��̾ �޶�پ��ٰ� ����
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾�� ������ ������ Ÿ�̸� �ʱ�ȭ
            isInContact = false;
            timeInContact = 0f;
            isAttachedToPlayer = false; // ���� �÷��̾�� ��������
            rb.isKinematic = false; // ������ ���� �ٽ� Ȱ��ȭ
        }
    }

    // �ٸ� �÷��̾ ���� ������ �������� �ϱ�
    public void TakeDamage(int damage)
    {
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        // 3�� ������ ���� ����
        currentHits++;
        if (currentHits >= maxHitsBeforeDeath)
        {
            Die();
        }

        // ���������� �������� �ϱ� (�ٸ� �÷��̾ ������)
        if (isAttachedToPlayer)
        {
            rb.isKinematic = false; // �������� ������ �޵��� ����
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse); // ���� ƨ��� ����
            isAttachedToPlayer = false; // ���������Ƿ� �� �̻� ���� ����
        }
    }

    // ���� ���� �� ó��
    void Die()
    {
        Debug.Log("���� �׾����ϴ�!");
        Destroy(gameObject); // �� ������Ʈ ����
    }

    void PlayerDie()
    {
        Debug.Log("�÷��̾ �׾����ϴ�!");
    }

}
