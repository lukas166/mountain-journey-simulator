using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShoeManager : MonoBehaviour
{
    [Header("Visual & Audio")]
    public MeshRenderer shoeRenderer; 
    public AudioSource footstepSource; // Audio Langkah Kaki (Loop)

    [Header("Pengaturan Pergerakan")]
    public float kecepatanLari = 7.0f; 
    private float kecepatanAwal = 2.0f;

    private ActionBasedContinuousMoveProvider moveProvider;
    private bool isWearing = false;

    private void Awake()
    {
        // Mencari sistem gerak player secara otomatis
        moveProvider = FindFirstObjectByType<ActionBasedContinuousMoveProvider>();

        // Simpan kecepatan awal player sebelum pakai sepatu
        if (moveProvider != null) kecepatanAwal = moveProvider.moveSpeed;
    }

    private void Update()
    {
        // Logika Suara Langkah Otomatis hanya jika sedang dipakai
        if (isWearing && moveProvider != null)
        {
            // Baca input joystick dari tangan kiri
            Vector2 inputGerak = moveProvider.leftHandMoveAction.action.ReadValue<Vector2>();
            
            // Jika joystick digerakkan lebih dari ambang batas (0.1)
            if (inputGerak.magnitude > 0.1f) 
            {
                if (!footstepSource.isPlaying) footstepSource.Play();
            }
            else
            {
                // Jika berhenti jalan, suara juga berhenti
                if (footstepSource.isPlaying) footstepSource.Stop();
            }
        }
    }

    // Dipanggil saat masuk ke FootSocket
    public void PakaiSepatu()
    {
        isWearing = true;
        if (shoeRenderer != null) shoeRenderer.enabled = false;
        if (moveProvider != null) moveProvider.moveSpeed = kecepatanLari;
    }

    // Dipanggil saat ditarik keluar dari FootSocket
    public void LepasSepatu()
    {
        isWearing = false;
        if (shoeRenderer != null) shoeRenderer.enabled = true;
        if (moveProvider != null) moveProvider.moveSpeed = kecepatanAwal;
        if (footstepSource.isPlaying) footstepSource.Stop();
    }
}