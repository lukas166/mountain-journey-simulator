using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class ShoeRunManager : MonoBehaviour
{
    [Header("Movement Provider")]
    public ContinuousMoveProvider moveProvider;

    [Header("Input Gerak Player")]
    public InputActionReference moveInput;

    [Header("Audio Langkah")]
    public AudioSource footstepSource;

    [Header("Socket Sepatu")]
    public XRSocketInteractor leftFootSocket;
    public XRSocketInteractor rightFootSocket;

    [Header("Tombol Grip")]
    public InputActionReference leftGripButton;
    public InputActionReference rightGripButton;

    [Header("Tombol Lari X Kiri")]
    public InputActionReference leftXButton;

    [Header("Tracking Posisi Ambil Sepatu")]
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    public Transform leftFootTransform;
    public Transform rightFootTransform;
    public float jarakAmbilSepatu = 0.45f;

    [Header("Anti Bug Cepat Lepas Pasang")]
    public float waktuBatalAmbil = 1.5f;

    [Header("Delay Setelah Grip Dilepas")]
    public float waktuToleransiGrip = 0.7f;

    [Header("Visual Sepatu")]
    public GameObject sepatuKiri;
    public GameObject sepatuKanan;

    [Header("Physics Sepatu")]
    public Rigidbody rbSepatuKiri;
    public Rigidbody rbSepatuKanan;
    public Collider[] colliderSepatuKiri;
    public Collider[] colliderSepatuKanan;
    public XRGrabInteractable grabSepatuKiri;
    public XRGrabInteractable grabSepatuKanan;

    [Header("Kecepatan")]
    public float kecepatanJalan = 5f;
    public float kecepatanLari = 12f;

    [Header("Stamina")]
    public float staminaMaksimal = 5f;
    public float staminaTurunPerDetik = 1f;
    public float staminaNaikPerDetik = 0.75f;
    public float batasInputGerak = 0.1f;

    [Header("Stamina Habis")]
    public float batasStaminaBisaLariLagi = 5f;

    [Header("Efek Merah di Kamera")]
    public CanvasGroup staminaRedCircle;
    public float alphaMerahMaksimal = 0.35f;
    public float kecepatanKedipMerah = 8f;
    public float kecepatanPudarMerah = 4f;

    private float staminaSekarang;
    private float alphaMerahSekarang = 0f;

    private bool staminaHabis = false;
    private bool pernahStaminaHabis = false;
    private bool modeLariAktif = false;

    private bool sepatuKiriDipakai = false;
    private bool sepatuKananDipakai = false;

    private bool sepatuKiriSiapDiambil = false;
    private bool sepatuKananSiapDiambil = false;

    private bool sepatuKiriSedangDipegang = false;
    private bool sepatuKananSedangDipegang = false;

    private bool bolehPakaiSepatuKiri = false;
    private bool bolehPakaiSepatuKanan = false;

    private float timerGripKiri = 0f;
    private float timerGripKanan = 0f;

    private Coroutine batalAmbilSepatuKiriCoroutine;
    private Coroutine batalAmbilSepatuKananCoroutine;

    void Awake()
    {
        AmbilKomponenSepatuOtomatis();
    }

    void OnEnable()
    {
        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Enable();
        }

        if (leftGripButton != null && leftGripButton.action != null)
        {
            leftGripButton.action.Enable();
            leftGripButton.action.performed += AmbilSepatuKiri;
        }

        if (rightGripButton != null && rightGripButton.action != null)
        {
            rightGripButton.action.Enable();
            rightGripButton.action.performed += AmbilSepatuKanan;
        }

        if (leftXButton != null && leftXButton.action != null)
        {
            leftXButton.action.Enable();
        }

        if (leftFootSocket != null)
        {
            leftFootSocket.selectEntered.AddListener(OnSepatuKiriMasuk);
            leftFootSocket.selectExited.AddListener(OnSepatuKiriKeluar);
            leftFootSocket.socketActive = false;
        }

        if (rightFootSocket != null)
        {
            rightFootSocket.selectEntered.AddListener(OnSepatuKananMasuk);
            rightFootSocket.selectExited.AddListener(OnSepatuKananKeluar);
            rightFootSocket.socketActive = false;
        }

        if (grabSepatuKiri != null)
        {
            grabSepatuKiri.selectEntered.AddListener(OnSepatuKiriSelected);
            grabSepatuKiri.selectExited.AddListener(OnSepatuKiriReleased);
        }

        if (grabSepatuKanan != null)
        {
            grabSepatuKanan.selectEntered.AddListener(OnSepatuKananSelected);
            grabSepatuKanan.selectExited.AddListener(OnSepatuKananReleased);
        }
    }

    void OnDisable()
    {
        if (leftFootSocket != null)
        {
            leftFootSocket.selectEntered.RemoveListener(OnSepatuKiriMasuk);
            leftFootSocket.selectExited.RemoveListener(OnSepatuKiriKeluar);
        }

        if (rightFootSocket != null)
        {
            rightFootSocket.selectEntered.RemoveListener(OnSepatuKananMasuk);
            rightFootSocket.selectExited.RemoveListener(OnSepatuKananKeluar);
        }

        if (grabSepatuKiri != null)
        {
            grabSepatuKiri.selectEntered.RemoveListener(OnSepatuKiriSelected);
            grabSepatuKiri.selectExited.RemoveListener(OnSepatuKiriReleased);
        }

        if (grabSepatuKanan != null)
        {
            grabSepatuKanan.selectEntered.RemoveListener(OnSepatuKananSelected);
            grabSepatuKanan.selectExited.RemoveListener(OnSepatuKananReleased);
        }

        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Disable();
        }

        if (leftGripButton != null && leftGripButton.action != null)
        {
            leftGripButton.action.performed -= AmbilSepatuKiri;
            leftGripButton.action.Disable();
        }

        if (rightGripButton != null && rightGripButton.action != null)
        {
            rightGripButton.action.performed -= AmbilSepatuKanan;
            rightGripButton.action.Disable();
        }

        if (leftXButton != null && leftXButton.action != null)
        {
            leftXButton.action.Disable();
        }
    }

    void Start()
    {
        staminaSekarang = staminaMaksimal;
        staminaHabis = false;
        pernahStaminaHabis = false;
        modeLariAktif = false;
        alphaMerahSekarang = 0f;

        if (moveProvider != null)
        {
            moveProvider.moveSpeed = kecepatanJalan;
        }

        if (staminaRedCircle != null)
        {
            staminaRedCircle.alpha = 0f;
        }

        SetVisualSepatu(sepatuKiri, true);
        SetVisualSepatu(sepatuKanan, true);

        AturColliderSolid(colliderSepatuKiri);
        AturColliderSolid(colliderSepatuKanan);

        if (leftFootSocket != null)
        {
            leftFootSocket.socketActive = false;
        }

        if (rightFootSocket != null)
        {
            rightFootSocket.socketActive = false;
        }
    }

    void Update()
    {
        AturSocketDenganGripDelay();

        bool memakaiSepatuLengkap = sepatuKiriDipakai && sepatuKananDipakai;
        bool sedangBergerak = CekPlayerBergerak();

        AturToggleLari(memakaiSepatuLengkap);
        AturStamina(memakaiSepatuLengkap, sedangBergerak);
        AturKecepatan(memakaiSepatuLengkap);
        AturSuaraLangkah(memakaiSepatuLengkap, sedangBergerak);
        AturEfekMerah(memakaiSepatuLengkap, sedangBergerak);
    }

    private void AmbilKomponenSepatuOtomatis()
    {
        if (sepatuKiri != null)
        {
            if (rbSepatuKiri == null)
            {
                rbSepatuKiri = sepatuKiri.GetComponent<Rigidbody>();
            }

            if (colliderSepatuKiri == null || colliderSepatuKiri.Length == 0)
            {
                colliderSepatuKiri = sepatuKiri.GetComponentsInChildren<Collider>();
            }

            if (grabSepatuKiri == null)
            {
                grabSepatuKiri = sepatuKiri.GetComponent<XRGrabInteractable>();
            }
        }

        if (sepatuKanan != null)
        {
            if (rbSepatuKanan == null)
            {
                rbSepatuKanan = sepatuKanan.GetComponent<Rigidbody>();
            }

            if (colliderSepatuKanan == null || colliderSepatuKanan.Length == 0)
            {
                colliderSepatuKanan = sepatuKanan.GetComponentsInChildren<Collider>();
            }

            if (grabSepatuKanan == null)
            {
                grabSepatuKanan = sepatuKanan.GetComponent<XRGrabInteractable>();
            }
        }
    }

    private void AturToggleLari(bool memakaiSepatuLengkap)
    {
        if (!memakaiSepatuLengkap)
        {
            modeLariAktif = false;
            return;
        }

        if (staminaHabis)
        {
            return;
        }

        if (leftXButton == null || leftXButton.action == null)
        {
            return;
        }

        if (leftXButton.action.WasPressedThisFrame())
        {
            modeLariAktif = !modeLariAktif;
        }
    }

    private void AturSocketDenganGripDelay()
    {
        bool leftGripPressed = false;
        bool rightGripPressed = false;

        if (leftGripButton != null && leftGripButton.action != null)
        {
            leftGripPressed = leftGripButton.action.IsPressed();
        }

        if (rightGripButton != null && rightGripButton.action != null)
        {
            rightGripPressed = rightGripButton.action.IsPressed();
        }

        if (leftGripPressed)
        {
            timerGripKiri = waktuToleransiGrip;
        }
        else
        {
            timerGripKiri -= Time.deltaTime;
        }

        if (rightGripPressed)
        {
            timerGripKanan = waktuToleransiGrip;
        }
        else
        {
            timerGripKanan -= Time.deltaTime;
        }

        timerGripKiri = Mathf.Max(timerGripKiri, 0f);
        timerGripKanan = Mathf.Max(timerGripKanan, 0f);

        bolehPakaiSepatuKiri = timerGripKiri > 0f;
        bolehPakaiSepatuKanan = timerGripKanan > 0f;

        if (leftFootSocket != null)
        {
            if (!sepatuKiriDipakai && bolehPakaiSepatuKiri)
            {
                leftFootSocket.socketActive = true;
            }
            else if (sepatuKiriDipakai)
            {
                leftFootSocket.socketActive = true;
            }
            else
            {
                leftFootSocket.socketActive = false;
            }
        }

        if (rightFootSocket != null)
        {
            if (!sepatuKananDipakai && bolehPakaiSepatuKanan)
            {
                rightFootSocket.socketActive = true;
            }
            else if (sepatuKananDipakai)
            {
                rightFootSocket.socketActive = true;
            }
            else
            {
                rightFootSocket.socketActive = false;
            }
        }
    }

    private bool CekPlayerBergerak()
    {
        if (moveInput == null || moveInput.action == null)
        {
            return false;
        }

        Vector2 input = moveInput.action.ReadValue<Vector2>();
        return input.magnitude > batasInputGerak;
    }

    private void AturStamina(bool memakaiSepatuLengkap, bool sedangBergerak)
    {
        bool sedangLari = memakaiSepatuLengkap && sedangBergerak && modeLariAktif && !staminaHabis;

        if (sedangLari)
        {
            staminaSekarang -= staminaTurunPerDetik * Time.deltaTime;
        }
        else
        {
            staminaSekarang += staminaNaikPerDetik * Time.deltaTime;
        }

        staminaSekarang = Mathf.Clamp(staminaSekarang, 0f, staminaMaksimal);

        if (staminaSekarang <= 0f)
        {
            staminaHabis = true;
            pernahStaminaHabis = true;
            modeLariAktif = true;
        }

        if (staminaHabis && staminaSekarang >= batasStaminaBisaLariLagi)
        {
            staminaHabis = false;
        }

        if (staminaSekarang >= staminaMaksimal)
        {
            pernahStaminaHabis = false;
        }

        if (!modeLariAktif)
        {
            pernahStaminaHabis = false;
        }
    }

    private void AturKecepatan(bool memakaiSepatuLengkap)
    {
        if (moveProvider == null)
        {
            return;
        }

        if (!memakaiSepatuLengkap)
        {
            moveProvider.moveSpeed = kecepatanJalan;
            return;
        }

        if (!modeLariAktif)
        {
            moveProvider.moveSpeed = kecepatanJalan;
            return;
        }

        if (staminaHabis)
        {
            moveProvider.moveSpeed = kecepatanJalan;
            return;
        }

        float staminaRatio = staminaSekarang / staminaMaksimal;

        float speedSekarang = Mathf.Lerp(
            kecepatanJalan,
            kecepatanLari,
            staminaRatio
        );

        moveProvider.moveSpeed = speedSekarang;
    }

    private void AturSuaraLangkah(bool memakaiSepatuLengkap, bool sedangBergerak)
    {
        if (footstepSource == null)
        {
            return;
        }

        if (memakaiSepatuLengkap && sedangBergerak)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }

    private void AturEfekMerah(bool memakaiSepatuLengkap, bool sedangBergerak)
    {
        if (staminaRedCircle == null)
        {
            return;
        }

        float targetAlpha = 0f;

        if (memakaiSepatuLengkap && modeLariAktif && pernahStaminaHabis)
        {
            float staminaRatio = staminaSekarang / staminaMaksimal;
            float staminaBelumPenuh = 1f - staminaRatio;

            if (sedangBergerak)
            {
                float kedip = Mathf.Abs(Mathf.Sin(Time.time * kecepatanKedipMerah));
                targetAlpha = staminaBelumPenuh * alphaMerahMaksimal * kedip;
            }
            else
            {
                targetAlpha = staminaBelumPenuh * alphaMerahMaksimal;
            }
        }

        alphaMerahSekarang = Mathf.MoveTowards(
            alphaMerahSekarang,
            targetAlpha,
            kecepatanPudarMerah * Time.deltaTime
        );

        staminaRedCircle.alpha = alphaMerahSekarang;
    }

    private void AmbilSepatuKiri(InputAction.CallbackContext context)
    {
        if (!sepatuKiriDipakai) return;

        Transform target = leftFootTransform;

        if (target == null && sepatuKiri != null)
        {
            target = sepatuKiri.transform;
        }

        if (leftHandTransform == null || target == null)
        {
            return;
        }

        float jarak = Vector3.Distance(leftHandTransform.position, target.position);

        if (jarak <= jarakAmbilSepatu)
        {
            SiapkanSepatuKiriUntukDiambil();
        }
    }

    private void AmbilSepatuKanan(InputAction.CallbackContext context)
    {
        if (!sepatuKananDipakai) return;

        Transform target = rightFootTransform;

        if (target == null && sepatuKanan != null)
        {
            target = sepatuKanan.transform;
        }

        if (rightHandTransform == null || target == null)
        {
            return;
        }

        float jarak = Vector3.Distance(rightHandTransform.position, target.position);

        if (jarak <= jarakAmbilSepatu)
        {
            SiapkanSepatuKananUntukDiambil();
        }
    }

    private void SiapkanSepatuKiriUntukDiambil()
    {
        if (sepatuKiriSiapDiambil) return;

        sepatuKiriSiapDiambil = true;
        sepatuKiriDipakai = false;

        PaksaSepatuKiriKeluarDariSocket();

        SetVisualSepatu(sepatuKiri, true);
        AturColliderSolid(colliderSepatuKiri);
        AturRigidbodySiapDiambil(rbSepatuKiri);

        if (batalAmbilSepatuKiriCoroutine != null)
        {
            StopCoroutine(batalAmbilSepatuKiriCoroutine);
        }

        batalAmbilSepatuKiriCoroutine = StartCoroutine(BatalAmbilSepatuKiriJikaTidakDipegang());
    }

    private void SiapkanSepatuKananUntukDiambil()
    {
        if (sepatuKananSiapDiambil) return;

        sepatuKananSiapDiambil = true;
        sepatuKananDipakai = false;

        PaksaSepatuKananKeluarDariSocket();

        SetVisualSepatu(sepatuKanan, true);
        AturColliderSolid(colliderSepatuKanan);
        AturRigidbodySiapDiambil(rbSepatuKanan);

        if (batalAmbilSepatuKananCoroutine != null)
        {
            StopCoroutine(batalAmbilSepatuKananCoroutine);
        }

        batalAmbilSepatuKananCoroutine = StartCoroutine(BatalAmbilSepatuKananJikaTidakDipegang());
    }

    private IEnumerator BatalAmbilSepatuKiriJikaTidakDipegang()
    {
        yield return new WaitForSeconds(waktuBatalAmbil);

        if (sepatuKiriSiapDiambil && !sepatuKiriSedangDipegang)
        {
            PakaiSepatuKiri();
        }

        batalAmbilSepatuKiriCoroutine = null;
    }

    private IEnumerator BatalAmbilSepatuKananJikaTidakDipegang()
    {
        yield return new WaitForSeconds(waktuBatalAmbil);

        if (sepatuKananSiapDiambil && !sepatuKananSedangDipegang)
        {
            PakaiSepatuKanan();
        }

        batalAmbilSepatuKananCoroutine = null;
    }

    private void OnSepatuKiriSelected(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor || args.interactorObject is XRRayInteractor)
        {
            sepatuKiriSedangDipegang = true;
            sepatuKiriDipakai = false;
            sepatuKiriSiapDiambil = false;

            if (batalAmbilSepatuKiriCoroutine != null)
            {
                StopCoroutine(batalAmbilSepatuKiriCoroutine);
                batalAmbilSepatuKiriCoroutine = null;
            }

            if (leftFootSocket != null)
            {
                leftFootSocket.socketActive = false;
            }

            SetVisualSepatu(sepatuKiri, true);
            AturColliderSolid(colliderSepatuKiri);
            AturRigidbodyDipegang(rbSepatuKiri);
        }
    }

    private void OnSepatuKananSelected(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor || args.interactorObject is XRRayInteractor)
        {
            sepatuKananSedangDipegang = true;
            sepatuKananDipakai = false;
            sepatuKananSiapDiambil = false;

            if (batalAmbilSepatuKananCoroutine != null)
            {
                StopCoroutine(batalAmbilSepatuKananCoroutine);
                batalAmbilSepatuKananCoroutine = null;
            }

            if (rightFootSocket != null)
            {
                rightFootSocket.socketActive = false;
            }

            SetVisualSepatu(sepatuKanan, true);
            AturColliderSolid(colliderSepatuKanan);
            AturRigidbodyDipegang(rbSepatuKanan);
        }
    }

    private void OnSepatuKiriReleased(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor || args.interactorObject is XRRayInteractor)
        {
            sepatuKiriSedangDipegang = false;
        }
    }

    private void OnSepatuKananReleased(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor || args.interactorObject is XRRayInteractor)
        {
            sepatuKananSedangDipegang = false;
        }
    }

    private void PaksaSepatuKiriKeluarDariSocket()
    {
        if (leftFootSocket == null)
        {
            return;
        }

        leftFootSocket.socketActive = false;

        if (grabSepatuKiri != null && leftFootSocket.interactionManager != null)
        {
            leftFootSocket.interactionManager.SelectExit(
                (IXRSelectInteractor)leftFootSocket,
                (IXRSelectInteractable)grabSepatuKiri
            );
        }
    }

    private void PaksaSepatuKananKeluarDariSocket()
    {
        if (rightFootSocket == null)
        {
            return;
        }

        rightFootSocket.socketActive = false;

        if (grabSepatuKanan != null && rightFootSocket.interactionManager != null)
        {
            rightFootSocket.interactionManager.SelectExit(
                (IXRSelectInteractor)rightFootSocket,
                (IXRSelectInteractable)grabSepatuKanan
            );
        }
    }

    private void AturColliderTrigger(Collider[] colliders)
    {
        if (colliders == null)
        {
            return;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = true;
                colliders[i].isTrigger = true;
            }
        }
    }

    private void AturColliderSolid(Collider[] colliders)
    {
        if (colliders == null)
        {
            return;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = true;
                colliders[i].isTrigger = false;
            }
        }
    }

    private void AturRigidbodyDipakai(Rigidbody rb)
    {
        if (rb == null)
        {
            return;
        }

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void AturRigidbodySiapDiambil(Rigidbody rb)
    {
        if (rb == null)
        {
            return;
        }

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void AturRigidbodyDipegang(Rigidbody rb)
    {
        if (rb == null)
        {
            return;
        }

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void SetVisualSepatu(GameObject sepatu, bool aktif)
    {
        if (sepatu == null)
        {
            return;
        }

        Renderer[] renderers = sepatu.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = aktif;
        }
    }

    private void OnSepatuKiriMasuk(SelectEnterEventArgs args)
    {
        if (!bolehPakaiSepatuKiri)
        {
            return;
        }

        PakaiSepatuKiri();
    }

    private void OnSepatuKiriKeluar(SelectExitEventArgs args)
    {
        if (!sepatuKiriSiapDiambil && !sepatuKiriSedangDipegang)
        {
            LepasSepatuKiri();
        }
    }

    private void OnSepatuKananMasuk(SelectEnterEventArgs args)
    {
        if (!bolehPakaiSepatuKanan)
        {
            return;
        }

        PakaiSepatuKanan();
    }

    private void OnSepatuKananKeluar(SelectExitEventArgs args)
    {
        if (!sepatuKananSiapDiambil && !sepatuKananSedangDipegang)
        {
            LepasSepatuKanan();
        }
    }

    public void PakaiSepatuKiri()
    {
        sepatuKiriDipakai = true;
        sepatuKiriSiapDiambil = false;
        sepatuKiriSedangDipegang = false;

        if (batalAmbilSepatuKiriCoroutine != null)
        {
            StopCoroutine(batalAmbilSepatuKiriCoroutine);
            batalAmbilSepatuKiriCoroutine = null;
        }

        if (leftFootSocket != null)
        {
            leftFootSocket.socketActive = true;
        }

        SetVisualSepatu(sepatuKiri, false);
        AturColliderTrigger(colliderSepatuKiri);
        AturRigidbodyDipakai(rbSepatuKiri);

        Debug.Log("Sepatu kiri dipakai.");
    }

    public void LepasSepatuKiri()
    {
        sepatuKiriDipakai = false;
        sepatuKiriSiapDiambil = false;

        if (batalAmbilSepatuKiriCoroutine != null)
        {
            StopCoroutine(batalAmbilSepatuKiriCoroutine);
            batalAmbilSepatuKiriCoroutine = null;
        }

        SetVisualSepatu(sepatuKiri, true);
        AturColliderSolid(colliderSepatuKiri);
        AturRigidbodyDipegang(rbSepatuKiri);

        Debug.Log("Sepatu kiri dilepas.");
    }

    public void PakaiSepatuKanan()
    {
        sepatuKananDipakai = true;
        sepatuKananSiapDiambil = false;
        sepatuKananSedangDipegang = false;

        if (batalAmbilSepatuKananCoroutine != null)
        {
            StopCoroutine(batalAmbilSepatuKananCoroutine);
            batalAmbilSepatuKananCoroutine = null;
        }

        if (rightFootSocket != null)
        {
            rightFootSocket.socketActive = true;
        }

        SetVisualSepatu(sepatuKanan, false);
        AturColliderTrigger(colliderSepatuKanan);
        AturRigidbodyDipakai(rbSepatuKanan);

        Debug.Log("Sepatu kanan dipakai.");
    }

    public void LepasSepatuKanan()
    {
        sepatuKananDipakai = false;
        sepatuKananSiapDiambil = false;

        if (batalAmbilSepatuKananCoroutine != null)
        {
            StopCoroutine(batalAmbilSepatuKananCoroutine);
            batalAmbilSepatuKananCoroutine = null;
        }

        SetVisualSepatu(sepatuKanan, true);
        AturColliderSolid(colliderSepatuKanan);
        AturRigidbodyDipegang(rbSepatuKanan);

        Debug.Log("Sepatu kanan dilepas.");
    }
}