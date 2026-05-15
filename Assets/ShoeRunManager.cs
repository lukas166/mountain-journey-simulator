using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
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

    [Header("Delay Setelah Grip Dilepas")]
    public float waktuToleransiGrip = 0.7f;

    [Header("Visual Sepatu")]
    public GameObject sepatuKiri;
    public GameObject sepatuKanan;

    [Header("Kecepatan")]
    public float kecepatanJalan = 5f;
    public float kecepatanLari = 12f;

    [Header("Stamina")]
    public float staminaMaksimal = 5f;
    public float staminaTurunPerDetik = 1f;
    public float staminaNaikPerDetik = 0.75f;
    public float batasInputGerak = 0.1f;

    [Header("Stamina Habis")]
    public float batasStaminaBisaLariLagi = 2.5f;

    [Header("Efek Merah di Kamera")]
    public CanvasGroup staminaRedCircle;
    public float alphaMerahMaksimal = 0.35f;

    private float staminaSekarang;
    private bool staminaHabis = false;

    private bool sepatuKiriDipakai = false;
    private bool sepatuKananDipakai = false;

    private float timerGripKiri = 0f;
    private float timerGripKanan = 0f;

    void OnEnable()
    {
        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Enable();
        }

        if (leftGripButton != null && leftGripButton.action != null)
        {
            leftGripButton.action.Enable();
        }

        if (rightGripButton != null && rightGripButton.action != null)
        {
            rightGripButton.action.Enable();
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

        if (moveInput != null && moveInput.action != null)
        {
            moveInput.action.Disable();
        }

        if (leftGripButton != null && leftGripButton.action != null)
        {
            leftGripButton.action.Disable();
        }

        if (rightGripButton != null && rightGripButton.action != null)
        {
            rightGripButton.action.Disable();
        }
    }

    void Start()
    {
        staminaSekarang = staminaMaksimal;
        staminaHabis = false;

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
    }

    void Update()
    {
        AturSocketDenganGripDelay();

        bool memakaiSepatuLengkap = sepatuKiriDipakai && sepatuKananDipakai;
        bool sedangBergerak = CekPlayerBergerak();

        AturStamina(memakaiSepatuLengkap, sedangBergerak);
        AturKecepatan(memakaiSepatuLengkap);
        AturSuaraLangkah(memakaiSepatuLengkap, sedangBergerak);
        AturEfekMerah();
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

        bool socketKiriBolehAktif = timerGripKiri > 0f;
        bool socketKananBolehAktif = timerGripKanan > 0f;

        if (leftFootSocket != null)
        {
            if (socketKiriBolehAktif || leftFootSocket.hasSelection)
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
            if (socketKananBolehAktif || rightFootSocket.hasSelection)
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
        if (memakaiSepatuLengkap && sedangBergerak && !staminaHabis)
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
        }

        if (staminaHabis && staminaSekarang >= batasStaminaBisaLariLagi)
        {
            staminaHabis = false;
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

    private void AturEfekMerah()
    {
        if (staminaRedCircle == null)
        {
            return;
        }

        float staminaRatio = staminaSekarang / staminaMaksimal;
        float staminaHilang = 1f - staminaRatio;

        staminaRedCircle.alpha = staminaHilang * alphaMerahMaksimal;
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
        PakaiSepatuKiri();
    }

    private void OnSepatuKiriKeluar(SelectExitEventArgs args)
    {
        LepasSepatuKiri();
    }

    private void OnSepatuKananMasuk(SelectEnterEventArgs args)
    {
        PakaiSepatuKanan();
    }

    private void OnSepatuKananKeluar(SelectExitEventArgs args)
    {
        LepasSepatuKanan();
    }

    public void PakaiSepatuKiri()
    {
        sepatuKiriDipakai = true;
        SetVisualSepatu(sepatuKiri, false);

        Debug.Log("Sepatu kiri dipakai dan visual disembunyikan.");
    }

    public void LepasSepatuKiri()
    {
        sepatuKiriDipakai = false;
        SetVisualSepatu(sepatuKiri, true);

        Debug.Log("Sepatu kiri dilepas dan visual ditampilkan.");
    }

    public void PakaiSepatuKanan()
    {
        sepatuKananDipakai = true;
        SetVisualSepatu(sepatuKanan, false);

        Debug.Log("Sepatu kanan dipakai dan visual disembunyikan.");
    }

    public void LepasSepatuKanan()
    {
        sepatuKananDipakai = false;
        SetVisualSepatu(sepatuKanan, true);

        Debug.Log("Sepatu kanan dilepas dan visual ditampilkan.");
    }
}