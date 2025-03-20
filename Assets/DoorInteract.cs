using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DoorInteract : MonoBehaviour
{
    public Transform player;
    public Transform exitPoint_A; // 문 앞쪽 출구
    public Transform exitPoint_B; // 문 뒤쪽 출구
    public static Image progressBar;
    public static TextMeshProUGUI interactText;
    public static Transform canvasTransform; // ✅ UI가 배치될 `Canvas`
    private static DoorInteract currentDoor = null; // ✅ 현재 활성화된 문

    private bool isHoldingE = false;
    private float holdTime = 1.5f;
    private float currentHoldTime = 0f;
    private Vector3 doorForward; // 문의 방향 저장

    void Start()
    {
        doorForward = transform.forward; // 문의 정면 방향 저장

        // ✅ `DoorCanvas`를 찾고 UI 요소 가져오기
        GameObject canvasObj = GameObject.Find("DoorCanvas");
        if (canvasObj != null)
        {
            canvasTransform = canvasObj.transform;
            progressBar = canvasObj.transform.Find("ProgressBar")?.GetComponent<Image>();
            interactText = canvasObj.transform.Find("InteractText")?.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("🚨 [DoorInteract] `DoorCanvas`를 찾을 수 없습니다! Hierarchy에서 확인하세요.");
        }

        // ✅ UI 초기 상태 설정
        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (interactText != null) interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (progressBar == null || interactText == null || canvasTransform == null)
            return;

        bool isNearby = IsPlayerNearby();
        bool isLooking = IsLookingAtDoor();

        if (isNearby && isLooking)
        {
            if (currentDoor != this) // ✅ 현재 문이 바뀌었을 때만 UI를 업데이트
            {
                currentDoor = this;
                MoveCanvasToExitPoint(); // ✅ exitPoint 기준으로 UI 위치 변경
                interactText.gameObject.SetActive(true);
                progressBar.gameObject.SetActive(true);
                Debug.Log($"✅ {gameObject.name} UI 활성화됨!");
            }

            if (Input.GetKey(KeyCode.E))
            {
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
                interactText.gameObject.SetActive(false);
            }
        }
        else if (currentDoor == this)
        {
            currentDoor = null;
            interactText.gameObject.SetActive(false);
            progressBar.gameObject.SetActive(false);
            Debug.Log($"❌ {gameObject.name} UI 비활성화됨!");
        }
    }

    void MoveCanvasToExitPoint()
    {
        // ✅ 현재 이동할 출구 선택
        Transform selectedExit = GetCorrectExit();

        // ✅ 반대편 출구를 선택
        Transform uiPositionExit = (selectedExit == exitPoint_A) ? exitPoint_B : exitPoint_A;

        if (uiPositionExit != null)
        {
            // ✅ UI를 반대편 출구 위로 이동
            canvasTransform.position = uiPositionExit.position + new Vector3(0, 1.5f, 0);
        }

        // ✅ UI가 플레이어를 바라보도록 설정
        Vector3 lookDirection = player.position - canvasTransform.position;
        lookDirection.y = 0; // Y축 고정 (UI가 이상한 각도로 회전하지 않도록)
        canvasTransform.rotation = Quaternion.LookRotation(-lookDirection);
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
        Transform selectedExit = GetCorrectExit();

        if (selectedExit != null)
        {
            player.position = selectedExit.position;
            Debug.Log($"🚀 플레이어가 {selectedExit.name}로 텔레포트됨!");
        }
        else
        {
            Debug.LogError($"🚨 {gameObject.name} 출구 위치가 설정되지 않음!");
        }
    }

    Transform GetCorrectExit()
    {
        Vector3 playerToDoor = player.position - transform.position;
        float dot = Vector3.Dot(playerToDoor.normalized, doorForward);
        return dot > 0 ? exitPoint_A : exitPoint_B;
    }

    bool IsLookingAtDoor()
    {
        Transform cameraTransform = Camera.main.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Door");

        if (Physics.Raycast(ray, out hit, 7f, layerMask))
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }

    bool IsPlayerNearby()
    {
        return Vector3.Distance(player.position, transform.position) < 7f;
    }
}
