//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.InputSystem; // ✅ Wajib ditambahkan untuk membaca controller XR
//// REMOVE this to avoid conflict
//// using UnityEngine.UIElements;

//public class RadialSelection : MonoBehaviour
//{
//    public InputActionReference spawnButton;

//    [Range(2, 10)]
//    public int numberOfRadialPart;
//    public GameObject RadialPartPrefab;
//    public Transform radialPartCanvas;
//    public float angleBetweenPart = 10;

//    public Transform handTransform; // ✅ add this

//    // ✅ Tambahan referensi untuk kepala player dan jarak spawn
//    public Transform headTransform;
//    public float spawnDistance = 1000f; // 0.2 = 20 centimeter dari tangan

//    public UnityEvent<int> OnPartSelected; 

//    private List<GameObject> spawnedParts = new List<GameObject>();
//    private int currentSelectedRadialPart = 0; // ✅ fix init

//    // ✅ Tambahkan variabel ini untuk melacak apakah menu sedang aktif
//    private bool isMenuActive = false;

//    // ✅ TAMBAHKAN INI: Wajib untuk mengaktifkan tombol di New Input System
//    private void OnEnable()
//    {
//        if (spawnButton != null && spawnButton.action != null)
//        {
//            spawnButton.action.Enable();
//        }
//    }

//    // ✅ TAMBAHKAN INI: Mematikan tombol saat script tidak aktif agar tidak bocor memori
//    private void OnDisable()
//    {
//        if (spawnButton != null && spawnButton.action != null)
//        {
//            spawnButton.action.Disable();
//        }
//    }

//    void Start()
//    {
//        //SpawnRadialPart();
//        radialPartCanvas.gameObject.SetActive(false);
//    }

//    void Update()
//    {
//        //GetSelectedRadialPart();
//        if (spawnButton == null || spawnButton.action == null) return;

//        // Hanya mengecek saat tombol ditekan (klik)
//        if (spawnButton.action.WasPressedThisFrame())
//        {
//            if (!isMenuActive)
//            {
//                // Jika menu sedang tertutup, BUKA menu
//                SpawnRadialPart();
//                isMenuActive = true;
//            }
//            else
//            {
//                // Jika menu sedang terbuka, PILIH menu dan TUTUP
//                HideAndTriggerSelected();
//                isMenuActive = false;
//            }
//        }

//        // Selama menu berstatus aktif (terbuka), jalankan deteksi highlight terus menerus
//        if (isMenuActive)
//        {
//            GetSelectedRadialPart();
//        }
//    }

//    public void HideAndTriggerSelected()
//    {
//        OnPartSelected.Invoke(currentSelectedRadialPart);
//        radialPartCanvas.gameObject.SetActive(false);
//        isMenuActive = false; // ✅ Pastikan statusnya reset
//    }

//    public void GetSelectedRadialPart()
//    {
//        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
//        Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

//        float angle = Vector3.SignedAngle(radialPartCanvas.up, centerToHandProjected, radialPartCanvas.forward);
//        angle = Mathf.Repeat(360f - angle, 360f);

//        Debug.Log("ANGLE" + angle);
//        currentSelectedRadialPart = (int)(angle * numberOfRadialPart / 360); // ✅ fix typo

//        for (int i = 0; i < spawnedParts.Count; i++)
//        {
//            if (i == currentSelectedRadialPart)
//            {
//                spawnedParts[i].GetComponent<Image>().color = Color.blue;
//                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
//            }
//            else
//            {
//                spawnedParts[i].GetComponent<Image>().color = Color.white;
//                spawnedParts[i].transform.localScale = Vector3.one;
//            }
//        }
//    }

//    public void SpawnRadialPart()
//    {
//        radialPartCanvas.gameObject.SetActive(true);
//        //radialPartCanvas.position = handTransform.position;
//        //radialPartCanvas.rotation = handTransform.rotation;

//        //foreach (var item in spawnedParts)
//        //{
//        //    Destroy(item);
//        //}

//        //spawnedParts.Clear();

//        //for (int i = 0; i < numberOfRadialPart; i++)
//        //{
//        //    float angle = - i * 360 / numberOfRadialPart - angleBetweenPart / 2;
//        //    Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);

//        //    GameObject spawnedRadialPart = Instantiate(RadialPartPrefab, radialPartCanvas);
//        //    spawnedRadialPart.transform.position = radialPartCanvas.position;
//        //    spawnedRadialPart.transform.localEulerAngles = radialPartEulerAngle;

//        //    spawnedRadialPart.GetComponent<Image>().fillAmount =
//        //        (1f / numberOfRadialPart) - (angleBetweenPart / 360f);

//        //    spawnedParts.Add(spawnedRadialPart);
//        //}

//        // ✅ LOGIKA BARU: Mengatur posisi dan rotasi berdasarkan pandangan mata
//        if (headTransform != null)
//        {
//            // Mengambil arah pandangan player ke depan
//            Vector3 forwardDir = headTransform.forward;
//            // Mengunci sumbu Y (atas/bawah) agar menu tidak ikut miring saat player menunduk/mendongak
//            forwardDir.y = 0;
//            forwardDir.Normalize();

//            // Menu diletakkan sejauh 'spawnDistance' di depan tangan, mengikuti arah tubuh player
//            radialPartCanvas.position = handTransform.position + (forwardDir * spawnDistance);

//            // Membuat menu selalu berdiri tegak dan menghadap lurus ke depan
//            radialPartCanvas.rotation = Quaternion.LookRotation(forwardDir);
//        }
//        else
//        {
//            // Fallback (cadangan) jika headTransform lupa diisi di Inspector
//            radialPartCanvas.position = handTransform.position;
//            radialPartCanvas.rotation = handTransform.rotation;
//        }

//        foreach (var item in spawnedParts)
//        {
//            Destroy(item);
//        }

//        spawnedParts.Clear();

//        for (int i = 0; i < numberOfRadialPart; i++)
//        {
//            float angle = -i * 360 / numberOfRadialPart - angleBetweenPart / 2;
//            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);

//            GameObject spawnedRadialPart = Instantiate(RadialPartPrefab, radialPartCanvas);
//            spawnedRadialPart.transform.position = radialPartCanvas.position;
//            spawnedRadialPart.transform.localEulerAngles = radialPartEulerAngle;

//            spawnedRadialPart.GetComponent<Image>().fillAmount =
//                (1f / numberOfRadialPart) - (angleBetweenPart / 360f);

//            spawnedParts.Add(spawnedRadialPart);
//        }
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RadialSelection : MonoBehaviour
{
    public InputActionReference spawnButton; // Tombol B
    public InputActionReference selectButton; // ✅ TAMBAHAN: Tombol Trigger

    [Range(2, 10)]
    public int numberOfRadialPart;
    public GameObject RadialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;

    public Transform handTransform;
    public Transform headTransform;

    public float spawnDistance = 0.3f;
    public float downwardOffset = 0.15f;

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
        // ✅ Aktifkan tombol Trigger
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
        // ✅ Nonaktifkan tombol Trigger
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
        // 1. Logika Tombol B (Membuka / Membatalkan Menu)
        if (spawnButton != null && spawnButton.action != null && spawnButton.action.WasPressedThisFrame())
        {
            if (!isMenuActive)
            {
                SpawnRadialPart();
                isMenuActive = true;
            }
            else
            {
                // Tutup menu TANPA mengeksekusi pilihan (Cancel)
                radialPartCanvas.gameObject.SetActive(false);
                isMenuActive = false;
            }
        }

        // 2. Logika Navigasi dan Pemilihan (Hanya berjalan jika menu sedang terbuka)
        if (isMenuActive)
        {
            GetSelectedRadialPart();

            // ✅ Logika Tombol Trigger (Konfirmasi Pilihan)
            if (selectButton != null && selectButton.action != null && selectButton.action.WasPressedThisFrame())
            {
                HideAndTriggerSelected();
            }
        }
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
                spawnedParts[i].GetComponent<Image>().color = Color.blue;
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                spawnedParts[i].GetComponent<Image>().color = Color.white;
                spawnedParts[i].transform.localScale = Vector3.one;
            }
        }
    }

    public void SpawnRadialPart()
    {
        radialPartCanvas.gameObject.SetActive(true);

        if (headTransform != null)
        {
            Vector3 forwardDir = headTransform.forward;
            forwardDir.y = 0;
            forwardDir.Normalize();

            Vector3 targetPosition = headTransform.position + (forwardDir * spawnDistance);
            targetPosition.y -= downwardOffset;

            radialPartCanvas.position = targetPosition;
            radialPartCanvas.rotation = Quaternion.LookRotation(forwardDir);
        }
        else
        {
            radialPartCanvas.position = handTransform.position;
            radialPartCanvas.rotation = handTransform.rotation;
        }

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

            spawnedParts.Add(spawnedRadialPart);
        }
    }
}