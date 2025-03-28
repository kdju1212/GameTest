using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DoorInteract : MonoBehaviour
{
    public Transform player;
    public Transform exitPoint_A;
    public Transform exitPoint_B;

    public static Image progressBarFill;
    public static GameObject progressBarBackground;
    public static TextMeshProUGUI interactText;
    public static Transform canvasTransform;

    private static DoorInteract currentDoor = null;

    private bool isHoldingE = false;
    private float holdTime = 1f;
    private float currentHoldTime = 0f;
    private Vector3 doorForward;

    void Start()
    {
        doorForward = transform.forward;

        GameObject canvasObj = GameObject.Find("DoorCanvas");
        if (canvasObj != null)
        {
            canvasTransform = canvasObj.transform;
            interactText = canvasTransform.Find("InteractText")?.GetComponent<TextMeshProUGUI>();

            Transform background = canvasTransform.Find("ProgressBarBackground");
            if (background != null)
            {
                progressBarBackground = background.gameObject;
                progressBarFill = background.Find("ProgressBarFill")?.GetComponent<Image>();
            }
        }
        else
        {
            Debug.LogError("🚨 DoorCanvas를 찾을 수 없습니다!");
        }

        ResetAllUI();
        if (canvasTransform != null)
            canvasTransform.gameObject.SetActive(false);
    }

    void Update()
    {
        if (progressBarFill == null || interactText == null || canvasTransform == null || progressBarBackground == null)
            return;

        bool isNearby = IsPlayerNearby();
        bool isLooking = IsLookingAtDoor();

        if (isNearby && isLooking)
        {
            if (currentDoor != this)
            {
                currentDoor = this;
                MoveCanvasToDoor();
                canvasTransform.gameObject.SetActive(true);
                interactText.gameObject.SetActive(true);
                progressBarBackground.SetActive(false);
                progressBarFill.gameObject.SetActive(false);
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
                ResetProgressBar();
            }
        }
        else if (currentDoor == this && (!isNearby || !isLooking))
        {
            ResetAllUI();
            canvasTransform.gameObject.SetActive(false);
            currentDoor = null;
        }
    }

    IEnumerator HoldToTeleport()
    {
        progressBarBackground.SetActive(true);
        progressBarFill.gameObject.SetActive(true);

        while (isHoldingE && currentHoldTime < holdTime)
        {
            currentHoldTime += Time.deltaTime;
            progressBarFill.fillAmount = currentHoldTime / holdTime;
            yield return null;
        }

        if (currentHoldTime >= holdTime)
        {
            TeleportPlayer();
        }

        ResetProgressBar();
    }

    void TeleportPlayer()
    {
        Transform selectedExit = GetCorrectExit();
        if (selectedExit != null)
        {
            player.position = selectedExit.position;
            Debug.Log($"🚪 {gameObject.name} → {selectedExit.name} 로 텔레포트 완료!");
        }
        else
        {
            Debug.LogError("❌ exitPoint가 설정되지 않음!");
        }
    }

    void MoveCanvasToDoor()
    {
        if (canvasTransform == null) return;

        Vector3 doorPos = transform.position;
        doorPos.y = 3f;

        Vector3 doorToPlayer = (player.position - doorPos).normalized;
        canvasTransform.position = doorPos + doorToPlayer * 1f;

        // 문과 평행하게 UI 회전
        Vector3 forward = transform.forward;
        forward.y = 0;
        Quaternion baseRotation = Quaternion.LookRotation(forward);

        float dot = Vector3.Dot(transform.forward, (player.position - transform.position).normalized);
        if (dot > 0)
        {
            canvasTransform.rotation = baseRotation * Quaternion.Euler(0, 180f, 0);
        }
        else
        {
            canvasTransform.rotation = baseRotation;
        }
    }

    Transform GetCorrectExit()
    {
        Vector3 doorToPlayer = (player.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, doorToPlayer);
        Debug.Log($"{gameObject.name} → dot: {dot}");
        return dot < 0 ? exitPoint_B : exitPoint_A;
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

    void ResetProgressBar()
    {
        isHoldingE = false;
        currentHoldTime = 0f;
        progressBarFill.fillAmount = 0f;
        progressBarFill.gameObject.SetActive(false);
        progressBarBackground.SetActive(false);
    }

    void ResetAllUI()
    {
        ResetProgressBar();
        if (interactText != null) interactText.gameObject.SetActive(false);
    }
}
