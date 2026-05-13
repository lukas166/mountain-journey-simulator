using UnityEngine;

public class QuestWindowFollowHead : MonoBehaviour
{
    public Transform headTransform;

    public float distance = 0.6f;
    public float downwardOffset = 0.05f;
    public float followSpeed = 12f;

    void LateUpdate()
    {
        if (headTransform == null)
        {
            return;
        }

        Vector3 forwardDir = headTransform.forward;
        forwardDir.y = 0;
        forwardDir.Normalize();

        Vector3 targetPosition = headTransform.position + forwardDir * distance;
        targetPosition.y -= downwardOffset;

        Quaternion targetRotation = Quaternion.LookRotation(forwardDir);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * followSpeed
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * followSpeed
        );
    }
}