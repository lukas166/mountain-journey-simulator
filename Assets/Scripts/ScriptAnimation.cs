using UnityEngine;

public class ScriptAnimation : MonoBehaviour
{
    public Animator targetAnimator;
    public string parameterName = "Talking";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetAnimator.SetTrigger(parameterName);
        }
    }
}