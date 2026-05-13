using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

public class HidingLaser : MonoBehaviour
{
    public GameObject questWindow;
    public GameObject exitConfirmWindow;

    public XRInteractorLineVisual lineVisual;
    public LineRenderer lineRenderer;

    void Update()
    {
        bool questActive = questWindow != null && questWindow.activeSelf;
        bool exitActive = exitConfirmWindow != null && exitConfirmWindow.activeSelf;

        bool shouldShowLaser = questActive || exitActive;

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