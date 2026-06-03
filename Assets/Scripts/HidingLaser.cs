using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

public class HidingLaser : MonoBehaviour
{
    [Header("Semua Window yang Membutuhkan Laser")]
    public GameObject[] windows;

    [Header("Komponen Laser")]
    public XRInteractorLineVisual lineVisual;
    public LineRenderer lineRenderer;

    void Update()
    {
        bool shouldShowLaser = false;

        for (int i = 0; i < windows.Length; i++)
        {
            if (windows[i] != null && windows[i].activeInHierarchy)
            {
                shouldShowLaser = true;
                break;
            }
        }

        if (lineVisual != null)
        {
            lineVisual.enabled = shouldShowLaser;
        }

        if (lineRenderer != null)
        {
            lineRenderer.enabled = shouldShowLaser;
        }
    }
}