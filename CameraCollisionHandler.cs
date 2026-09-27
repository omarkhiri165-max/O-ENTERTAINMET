using UnityEngine;

[DefaultExecutionOrder(100)]
public class CameraCollisionHandler : MonoBehaviour
{
    [Header("Collision Settings")]
    public float cameraRadiusRatio = 0.15f; // Nesba dyal hjm kora Raycast m3a scale
    public float smoothSpeed = 25f;

    [Header("Auto-Scaling Adjustments")]
    public float baseHeightOffset = 1.4f;   // Mostawa rass f scale 1.0
    public float baseMinDistance = 0.4f;    // A9rab msafa f scale 1.0

    private CameraFollowExact mainFollowScript;
    private Transform playerTarget;
    private Vector3 defaultLocalPos;

    void Start()
    {
        mainFollowScript = GetComponent<CameraFollowExact>();
        if (mainFollowScript != null)
        {
            playerTarget = mainFollowScript.playerTarget;
        }

        defaultLocalPos = transform.localPosition;

        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.nearClipPlane = 0.01f;
        }
    }

    void LateUpdate()
    {
        if (playerTarget == null) return;

        // Kanhsbo scale l-7ali dyal player
        float playerScale = playerTarget.lossyScale.y;

        // Dbt l-9iyam dynamic 3la hsab scale
        float dynamicHeight = baseHeightOffset * playerScale;
        float dynamicMinDist = baseMinDistance * playerScale;
        float dynamicRadius = cameraRadiusRatio * playerScale;
        float maxDistance = defaultLocalPos.magnitude;

        // Bdayat raycast mn rass l-player
        Vector3 origin = playerTarget.position + Vector3.up * dynamicHeight;
        
        Vector3 targetWorldPos = transform.parent != null 
            ? transform.parent.TransformPoint(defaultLocalPos) 
            : playerTarget.TransformPoint(defaultLocalPos);

        Vector3 direction = (targetWorldPos - origin).normalized;
        float desiredDistance = maxDistance;

        // SphereCastAll bach may-ratich l-hyot
        RaycastHit[] hits = Physics.SphereCastAll(origin, dynamicRadius, direction, maxDistance);
        float closestHit = maxDistance;

        foreach (var hit in hits)
        {
            // Tjahl l-player w ay child dyalo
            if (hit.transform == playerTarget || hit.transform.IsChildOf(playerTarget))
                continue;

            if (hit.collider.isTrigger)
                continue;

            if (hit.distance < closestHit)
            {
                closestHit = hit.distance;
            }
        }

        if (closestHit < maxDistance)
        {
            // Kay7bes 9bel l-7yt b daqa
            desiredDistance = Mathf.Clamp(closestHit - dynamicRadius, dynamicMinDist, maxDistance);
        }

        // T7rik l-camera b sor3a w wodo7
        Vector3 targetLocalPos = defaultLocalPos.normalized * desiredDistance;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * smoothSpeed);
    }
}