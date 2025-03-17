using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DoorInteract : MonoBehaviour
{
    public Transform player;
    public Image progressBar;
    public TextMeshProUGUI interactText;
    private bool isHoldingE = false;
    private float holdTime = 2f;
    private float currentHoldTime = 0f;

    void Start()
    {
        progressBar.fillAmount = 0f;
        progressBar.gameObject.SetActive(false);
        interactText.gameObject.SetActive(false);
        interactText.text = "Hold [E] for 2 seconds to open"; // 영어로 변경
    }

    void Update()
    {
        if (IsPlayerNearby() && IsLookingAtDoor()) // ✅ 플레이어가 문 근처에 있을 때만 실행
        {
            interactText.gameObject.SetActive(true); // UI 활성화
            Debug.Log("✅ 플레이어가 문 근처에 있음!");

            if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("🔘 E 키가 눌려짐!");
                if (!isHoldingE)
                {
                    isHoldingE = true;
                    StartCoroutine(HoldToTeleport());
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                isHoldingE = false;
                currentHoldTime = 0f;
                progressBar.fillAmount = 0f;
                progressBar.gameObject.SetActive(false);
            }
        }
        else
        {
            interactText.gameObject.SetActive(false);
            Debug.Log("❌ 플레이어가 문 근처에 없음!");
        }
    }


    // ✅ 문을 통과한 후 일정 시간 후 Collider 다시 활성화
    IEnumerator ReenableCollider(Collider collider)
    {
        yield return new WaitForSeconds(1f); // 1초 후 다시 활성화
        collider.enabled = true;
    }


    IEnumerator HoldToTeleport()
    {
        progressBar.gameObject.SetActive(true);

        while (isHoldingE && currentHoldTime < holdTime)
        {
            currentHoldTime += Time.deltaTime;
            progressBar.fillAmount = currentHoldTime / holdTime;
            yield return null;
        }

        if (currentHoldTime >= holdTime)
        {
            TeleportPlayer();
        }

        isHoldingE = false;
        currentHoldTime = 0f;
        progressBar.fillAmount = 0f;
        progressBar.gameObject.SetActive(false);
    }

    void TeleportPlayer()
    {
        Vector3 teleportOffset = transform.forward * 2f + transform.right * 0.5f;
        player.position = transform.position + teleportOffset;
        player.rotation = Quaternion.LookRotation(transform.forward);

        // ✅ 문(`Entrance_MainDoor`)의 Collider만 비활성화
        Collider doorCollider = GetComponent<Collider>();
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
            StartCoroutine(ReenableCollider(doorCollider));
        }

        Debug.Log("✅ 플레이어가 문을 통과하여 연구소 내부로 이동!");
    }

    //bool IsLookingAtDoor()
    //{
    //    Transform cameraTransform = Camera.main.transform;
    //    Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
    //    RaycastHit hit;

    //    int doorLayerMask = LayerMask.GetMask("Door"); // ✅ "Door" 레이어만 감지하도록 설정

    //    if (Physics.Raycast(ray, out hit, 7f, doorLayerMask)) // ✅ 문 레이어만 감지
    //    {
    //        Debug.Log($"🔍 Ray가 충돌한 오브젝트: {hit.collider.gameObject.name}");
    //        if (hit.collider.gameObject == gameObject)
    //        {
    //            Debug.Log("✅ 플레이어가 문을 바라보고 있음!");
    //            return true;
    //        }
    //    }

    //    Debug.Log("❌ 플레이어가 문을 바라보고 있지 않음!");
    //    return false;
    //}
    bool IsLookingAtDoor()
    {
        Transform cameraTransform = Camera.main.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        // ✅ Ray를 시각적으로 확인
        Debug.DrawRay(ray.origin, ray.direction * 7f, Color.red, 0.1f);

        int doorLayerMask = LayerMask.GetMask("Door"); // ✅ "Door" 레이어만 감지하도록 설정

        if (Physics.Raycast(ray, out hit, 7f, doorLayerMask)) // ✅ 문 레이어만 감지
        {
            Debug.Log($"🔍 Ray가 충돌한 오브젝트: {hit.collider.gameObject.name} (레이어: {hit.collider.gameObject.layer})");

            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log("✅ 플레이어가 문을 바라보고 있음!");
                return true;
            }
            else
            {
                Debug.Log("❌ Ray가 문이 아닌 다른 오브젝트를 감지함!");
            }
        }
        else
        {
            Debug.Log("❌ Ray가 아무것도 감지하지 않음!");
        }

        return false;
    }


    bool IsPlayerNearby()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        Debug.Log($"📏 플레이어와 문 거리: {distance}");

        return distance < 7f; // ✅ 문에서 3m 이내일 때만 true 반환
    }


}
