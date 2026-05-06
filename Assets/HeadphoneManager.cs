using UnityEngine;

using UnityEngine.InputSystem;

public class HeadphoneManager : MonoBehaviour
{
    [Header("Visual & Audio")]
    public MeshRenderer headphoneRenderer; // Target gambar 3D yang akan disembunyikan
    public AudioSource musicSource;    
    public AudioSource sfxSource;      
    public AudioClip clickSound;       

    [Header("Input Controller (Mute)")]
    public InputActionReference rightTapAction; 
    public InputActionReference leftTapAction;  
    
    [Header("Tracking Posisi")]
    public Transform headTransform;        
    public Transform leftHandTransform;    
    public Transform rightHandTransform;   
    public float jarakKetuk = 0.35f;       

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isWearing = false;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (rightTapAction != null) rightTapAction.action.performed += KetukKanan;
        if (leftTapAction != null) leftTapAction.action.performed += KetukKiri;
    }

    private void OnDisable()
    {
        if (rightTapAction != null) rightTapAction.action.performed -= KetukKanan;
        if (leftTapAction != null) leftTapAction.action.performed -= KetukKiri;
    }

    // Fungsi ini dipanggil oleh Socket di kepala saat bersentuhan
    public void ValidasiPakaiHeadphone()
    {
        // Cek apakah headphone sedang dipegang oleh 2 tangan (atau lebih)
        // Note: Socket sendiri dihitung sebagai 1 interactor oleh Unity saat ditarik.
        // Tapi sebelum ditarik socket, harus dipegang 2 tangan.
        
        Pakai();
    }

    private void Pakai()
    {
        isWearing = true;
        
        // Matikan gambarnya saja, fisiknya (Collider) tetap hidup agar bisa diambil lagi
        if (headphoneRenderer != null) headphoneRenderer.enabled = false; 
        
        // Putar suara cetrek dan nyalakan musik
        if (clickSound != null) sfxSource.PlayOneShot(clickSound);
        if (!musicSource.isPlaying) musicSource.Play();
        musicSource.mute = false; 
    }

    // Fungsi ini dipanggil oleh Socket saat ditarik menjauh dari kepala
    public void LepasHeadphone()
    {
        isWearing = false;
        
        // Munculkan gambarnya kembali
        if (headphoneRenderer != null) headphoneRenderer.enabled = true; 
        
        // Hentikan (Pause) musik
        musicSource.Pause();
    }

    private void KetukKanan(InputAction.CallbackContext context)
    {
        if (!isWearing) return; 
        float jarak = Vector3.Distance(headTransform.position, rightHandTransform.position);
        if (jarak <= jarakKetuk) EksekusiMute();
    }

    private void KetukKiri(InputAction.CallbackContext context)
    {
        if (!isWearing) return; 
        float jarak = Vector3.Distance(headTransform.position, leftHandTransform.position);
        if (jarak <= jarakKetuk) EksekusiMute();
    }

    private void EksekusiMute()
    {
        if (clickSound != null) sfxSource.PlayOneShot(clickSound);
        musicSource.mute = !musicSource.mute; 
    }
}