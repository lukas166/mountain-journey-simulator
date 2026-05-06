using UnityEngine;

public class RadialMenuTester : MonoBehaviour
{
    [Header("Masukkan Objek Terrain di sini")]
    public Terrain groundTerrain;

    [Header("Buat Material kosong di Unity & masukkan ke sini")]
    public Material testMaterial;

    [Header("Masukkan UI Quest Window di sini")]
    public GameObject questWindow;

    [Header("Masukkan Objek Laser Kanan di sini")]
    public GameObject rightHandLaser; // ✅ Variabel baru untuk mengontrol Laser

    private Material originalMaterial;

    void Start()
    {
        if (groundTerrain != null)
        {
            originalMaterial = groundTerrain.materialTemplate;
        }

        if (questWindow != null) questWindow.SetActive(false);

        // ✅ Matikan laser saat game baru dimulai
        if (rightHandLaser != null) rightHandLaser.SetActive(false);
    }

    public void ChangeGroundColor(int menuIndex)
    {
        if (groundTerrain == null || testMaterial == null) return;

        groundTerrain.materialTemplate = testMaterial;
        Color targetColor = Color.white;
        bool changeTerrainColor = true;

        switch (menuIndex)
        {
            case 0:
                targetColor = Color.red;
                break;
            case 1:
                // ✅ LOGIKA BARU: Buka Jendela & Nyalakan Laser
                changeTerrainColor = false;
                if (questWindow != null) questWindow.SetActive(true);
                if (rightHandLaser != null) rightHandLaser.SetActive(true);
                break;
            case 2:
                targetColor = Color.yellow;
                break;
            case 3:
                targetColor = Color.magenta;
                break;
            default:
                targetColor = Color.gray;
                break;
        }

        if (changeTerrainColor)
        {
            testMaterial.color = targetColor;
            if (testMaterial.HasProperty("_BaseColor")) testMaterial.SetColor("_BaseColor", targetColor);
        }
    }

    // ✅ FUNGSI BARU: Untuk dipanggil oleh Tombol "Ya, Saya Paham"
    public void TutupQuestWindow()
    {
        if (questWindow != null) questWindow.SetActive(false); // Tutup jendela
        if (rightHandLaser != null) rightHandLaser.SetActive(false); // Matikan laser
        Debug.Log("Quest Window ditutup, Laser dimatikan.");
    }

    void OnDestroy()
    {
        if (groundTerrain != null) groundTerrain.materialTemplate = originalMaterial;
    }
}