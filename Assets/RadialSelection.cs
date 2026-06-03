using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RadialSelection : MonoBehaviour
{
    public InputActionReference spawnButton; // Tombol B
    public InputActionReference selectButton; // Tombol Trigger

    [Range(2, 10)]
    public int numberOfRadialPart;
    public GameObject RadialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;

    public Transform handTransform;
    public Transform headTransform;

    public float spawnDistance = 0.3f;
    public float downwardOffset = 0.15f;

    public float followSpeed = 20f;

    [Header("Menu Icons")]
    public Sprite[] menuIcons;

    [Header("Audio Menu")]
    public bool aktifkanGlobalAudioToggle = true;
    public int audioMenuIndex = 2;
    public Sprite audioOnIcon;
    public Sprite audioMutedIcon;
    public float normalVolume = 1f;

    [Header("Menu Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.cyan;

    public UnityEvent<int> OnPartSelected;

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = 0;
    private bool isMenuActive = false;
    private bool audioMuted = false;

    private void OnEnable()
    {
        if (spawnButton != null && spawnButton.action != null)
        {
            spawnButton.action.Enable();
        }

        if (selectButton != null && selectButton.action != null)
        {
            selectButton.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (spawnButton != null && spawnButton.action != null)
        {
            spawnButton.action.Disable();
        }

        if (selectButton != null && selectButton.action != null)
        {
            selectButton.action.Disable();
        }
    }

    void Start()
    {
        radialPartCanvas.gameObject.SetActive(false);

        UpdateAudioIcon();
        TerapkanAudioMute();
    }

    void Update()
    {
        if (spawnButton != null && spawnButton.action != null && spawnButton.action.WasPressedThisFrame())
        {
            if (!isMenuActive)
            {
                SpawnRadialPart();
                isMenuActive = true;
            }
            else
            {
                radialPartCanvas.gameObject.SetActive(false);
                isMenuActive = false;
            }
        }

        if (isMenuActive)
        {
            FollowHeadView();

            GetSelectedRadialPart();

            if (selectButton != null && selectButton.action != null && selectButton.action.WasPressedThisFrame())
            {
                HideAndTriggerSelected();
            }
        }
    }

    private void FollowHeadView()
    {
        if (headTransform == null)
        {
            return;
        }

        Vector3 forwardDir = headTransform.forward;
        forwardDir.y = 0;
        forwardDir.Normalize();

        Vector3 targetPosition = headTransform.position + (forwardDir * spawnDistance);
        targetPosition.y -= downwardOffset;

        Quaternion targetRotation = Quaternion.LookRotation(forwardDir);

        radialPartCanvas.position = Vector3.Lerp(
            radialPartCanvas.position,
            targetPosition,
            Time.deltaTime * followSpeed
        );

        radialPartCanvas.rotation = Quaternion.Slerp(
            radialPartCanvas.rotation,
            targetRotation,
            Time.deltaTime * followSpeed
        );
    }

    public void HideAndTriggerSelected()
    {
        if (aktifkanGlobalAudioToggle && currentSelectedRadialPart == audioMenuIndex)
        {
            ToggleGlobalAudio();
        }
        else
        {
            OnPartSelected.Invoke(currentSelectedRadialPart);
        }

        radialPartCanvas.gameObject.SetActive(false);
        isMenuActive = false;
    }

    public void GetSelectedRadialPart()
    {
        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

        float angle = Vector3.SignedAngle(radialPartCanvas.up, centerToHandProjected, radialPartCanvas.forward);
        angle = Mathf.Repeat(360f - angle, 360f);

        currentSelectedRadialPart = (int)(angle * numberOfRadialPart / 360);

        for (int i = 0; i < spawnedParts.Count; i++)
        {
            if (i == currentSelectedRadialPart)
            {
                spawnedParts[i].GetComponent<Image>().color = selectedColor;
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                spawnedParts[i].GetComponent<Image>().color = normalColor;
                spawnedParts[i].transform.localScale = Vector3.one;
            }
        }
    }

    public void SpawnRadialPart()
    {
        UpdateAudioIcon();

        radialPartCanvas.gameObject.SetActive(true);

        FollowHeadView();

        foreach (var item in spawnedParts)
        {
            Destroy(item);
        }

        spawnedParts.Clear();

        for (int i = 0; i < numberOfRadialPart; i++)
        {
            float angle = -i * 360 / numberOfRadialPart - angleBetweenPart / 2;
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);

            GameObject spawnedRadialPart = Instantiate(RadialPartPrefab, radialPartCanvas);
            spawnedRadialPart.transform.position = radialPartCanvas.position;
            spawnedRadialPart.transform.localEulerAngles = radialPartEulerAngle;

            spawnedRadialPart.GetComponent<Image>().fillAmount =
                (1f / numberOfRadialPart) - (angleBetweenPart / 360f);

            Transform iconTransform = spawnedRadialPart.transform.Find("IconImage");

            if (iconTransform != null && i < menuIcons.Length)
            {
                Image iconImage = iconTransform.GetComponent<Image>();

                if (iconImage != null && menuIcons[i] != null)
                {
                    iconImage.sprite = menuIcons[i];
                    iconTransform.localEulerAngles = new Vector3(0, 0, -angle);
                }
            }

            spawnedParts.Add(spawnedRadialPart);
        }
    }

    private void ToggleGlobalAudio()
    {
        audioMuted = !audioMuted;

        TerapkanAudioMute();
        UpdateAudioIcon();

        Debug.Log(audioMuted ? "Semua audio dimute." : "Semua audio dinyalakan.");
    }

    private void TerapkanAudioMute()
    {
        if (audioMuted)
        {
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.volume = normalVolume;
        }
    }

    private void UpdateAudioIcon()
    {
        if (!aktifkanGlobalAudioToggle) return;
        if (menuIcons == null) return;
        if (audioMenuIndex < 0 || audioMenuIndex >= menuIcons.Length) return;

        if (audioMuted)
        {
            if (audioMutedIcon != null)
            {
                menuIcons[audioMenuIndex] = audioMutedIcon;
            }
        }
        else
        {
            if (audioOnIcon != null)
            {
                menuIcons[audioMenuIndex] = audioOnIcon;
            }
        }

        if (audioMenuIndex < spawnedParts.Count)
        {
            Transform iconTransform = spawnedParts[audioMenuIndex].transform.Find("IconImage");

            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();

                if (iconImage != null && menuIcons[audioMenuIndex] != null)
                {
                    iconImage.sprite = menuIcons[audioMenuIndex];
                }
            }
        }
    }
}