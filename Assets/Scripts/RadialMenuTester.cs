using UnityEngine;

public class RadialMenuTester : MonoBehaviour
{
    [Header("Masukkan Objek Terrain di sini")]
    public Terrain groundTerrain;

    [Header("Buat Material kosong di Unity & masukkan ke sini")]
    public Material testMaterial;

    [Header("Masukkan UI Quest Window di sini")]
    public GameObject questWindow;

    [Header("Masukkan UI Exit Confirm Window di sini")]
    public GameObject exitConfirmWindow;

    [Header("Masukkan Objek Laser Kanan di sini")]
    public GameObject rightHandLaser;

    private Material originalMaterial;

    void Start()
    {
        if (groundTerrain != null)
        {
            originalMaterial = groundTerrain.materialTemplate;
        }

        if (questWindow != null) questWindow.SetActive(false);
        if (exitConfirmWindow != null) exitConfirmWindow.SetActive(false);

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
                targetColor = Color.gray;
                if (exitConfirmWindow != null) exitConfirmWindow.SetActive(true);
                if (questWindow != null) questWindow.SetActive(false);
                if (rightHandLaser != null) rightHandLaser.SetActive(true);
                break;

            case 1:
                targetColor = Color.magenta;

                if (questWindow != null) questWindow.SetActive(true);
                if (exitConfirmWindow != null) exitConfirmWindow.SetActive(false);
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

            if (testMaterial.HasProperty("_BaseColor"))
            {
                testMaterial.SetColor("_BaseColor", targetColor);
            }
        }
    }

    public void TutupQuestWindow()
    {
        if (questWindow != null) questWindow.SetActive(false);
        if (rightHandLaser != null) rightHandLaser.SetActive(false);

        Debug.Log("Quest Window ditutup, Laser dimatikan.");
    }

    public void TutupExitConfirmWindow()
    {
        if (exitConfirmWindow != null) exitConfirmWindow.SetActive(false);
        if (rightHandLaser != null) rightHandLaser.SetActive(false);

        Debug.Log("Exit Confirm Window ditutup, Laser dimatikan.");
    }

    public void KeluarGame()
    {
        Debug.Log("Game keluar.");

        Application.Quit();
    }

    void OnDestroy()
    {
        if (groundTerrain != null)
        {
            groundTerrain.materialTemplate = originalMaterial;
        }
    }
}