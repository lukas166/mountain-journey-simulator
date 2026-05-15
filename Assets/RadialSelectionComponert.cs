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

    // KODE BARU: Array untuk menampung gambar ikon (Exit, Quest, Sound)
    [Header("Menu Icons")]
    public Sprite[] menuIcons; 

    [Header("Menu Colors")]
    public Color normalColor = Color.white;       // Warna saat diam
    public Color selectedColor = Color.cyan;      // Warna saat disorot (Default: Biru Muda/Cyan)

    public UnityEvent<int> OnPartSelected;

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = 0;
    private bool isMenuActive = false;

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
        OnPartSelected.Invoke(currentSelectedRadialPart);
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
                // KODE YANG DIUBAH: Menggunakan warna dari Inspector
                spawnedParts[i].GetComponent<Image>().color = selectedColor; 
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                // KODE YANG DIUBAH: Menggunakan warna normal dari Inspector
                spawnedParts[i].GetComponent<Image>().color = normalColor; 
                spawnedParts[i].transform.localScale = Vector3.one;
            }
        }
    }

    public void SpawnRadialPart()
    {
        radialPartCanvas.gameObject.SetActive(true);

        FollowHeadView();

        foreach (var item in spawnedParts)
        {
            Destroy(item);
        }

        spawnedParts.Clear();

        for (int i = 0; i < numberOfRadialPart; i++)
        {
            // Menghitung rotasi untuk potongan menu ini
            float angle = -i * 360 / numberOfRadialPart - angleBetweenPart / 2;
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);

            GameObject spawnedRadialPart = Instantiate(RadialPartPrefab, radialPartCanvas);
            spawnedRadialPart.transform.position = radialPartCanvas.position;
            spawnedRadialPart.transform.localEulerAngles = radialPartEulerAngle;

            spawnedRadialPart.GetComponent<Image>().fillAmount =
                (1f / numberOfRadialPart) - (angleBetweenPart / 360f);

            // ---------------------------------------------------------
            // KODE BARU: Memasang Ikon ke masing-masing bagian
            // ---------------------------------------------------------
            
            // 1. Cari objek anak bernama "IconImage" di dalam prefab yang baru di-spawn
            Transform iconTransform = spawnedRadialPart.transform.Find("IconImage");
            
            if (iconTransform != null && i < menuIcons.Length)
            {
                // 2. Ambil komponen Image-nya dan masukkan gambar dari array menuIcons
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null && menuIcons[i] != null)
                {
                    iconImage.sprite = menuIcons[i];
                    
                    // 3. Putar balik ikonnya agar posisinya tegak (tidak ikut miring/terbalik karena parent-nya diputar)
                    // Kita memutarnya berlawanan arah dari rotasi parent (angle)
                    iconTransform.localEulerAngles = new Vector3(0, 0, -angle);
                }
            }
            // ---------------------------------------------------------

            spawnedParts.Add(spawnedRadialPart);
        }
    }
}