using System.Collections;
using UnityEngine;

public class Shovel : MonoBehaviour, IPickupable
{
    private bool isPickedUp = false;
    public Transform playerCamera;
    public Transform player;
    public float attackRange = 5f;
    public int attackDamage = 10;
    public float attackCooldown = 0.5f;
    private float lastAttackTime;
    private bool canAttack = true;

    private Vector3 originalScale;
    private Rigidbody rb;
    private Collider col;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;

        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        if (playerCamera == null)
            playerCamera = Camera.main?.transform;
    }

    void Update()
    {
        
        if (isPickedUp && Input.GetMouseButtonDown(0) && canAttack)
        {
            Attack();
        }
    }



    public void OnPickup()
    {
        isPickedUp = true;
        Transform hand = GameObject.FindWithTag("Player")?.transform.Find("Hand");

        if (hand != null)
        {
            transform.SetParent(hand);
            transform.localPosition = new Vector3(0.3f, -0.2f, 0.8f);
            transform.localRotation = Quaternion.identity;
            transform.localScale = originalScale;
            

        }

        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public void OnDrop(Vector3 dropPosition)
    {
        isPickedUp = false;

        transform.SetParent(null);
        transform.position = dropPosition;
        transform.localScale = originalScale;

        if (col != null) col.enabled = true;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    public void DealDamage()
    {
        if (playerCamera == null) return;

        RaycastHit hit;
        Vector3 origin = playerCamera.position;
        Vector3 direction = playerCamera.forward;

        if (Physics.Raycast(origin, direction, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                }

                Rigidbody enemyRb = hit.collider.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    StartCoroutine(FreezeEnemy(enemyRb, 0.5f));
                }
            }
        }
    }

    private IEnumerator FreezeEnemy(Rigidbody enemyRb, float duration)
    {
        if (enemyRb == null) yield break;

        enemyRb.isKinematic = true;
        yield return new WaitForSeconds(duration);

        if (enemyRb != null)
        {
            enemyRb.isKinematic = false;
        }
    }

    public void OnEquip()
    {
        isPickedUp = true;
    }

    public void OnUnequip()
    {
        isPickedUp = false;
    }

}
