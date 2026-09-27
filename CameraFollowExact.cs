using UnityEngine;

public class CameraFollowExact : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform playerTarget; // 7t hna l-Player dyalk

    [Header("Mouse Control")]
    public float mouseSensitivity = 3.0f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 60f;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Start()
    {
        if (playerTarget != null)
        {
            // Nrj3o l-camera child dyal l-player m مؤقتاً wla nḥsbo l-offset b ḍabṭ
            // Bach tbqa mhafda 3la l-wḍi3ia b ḍabṭ b ḍabṭ
            transform.SetParent(playerTarget);
        }

        // Nsjllo l-pos w rotation li ḥtitiha b ydk f l-Editor
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        // I5fa2 l-mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (playerTarget == null) return;

        // Qra2at ḥaraka l-mouse
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, yMinLimit, yMaxLimit);

        // Doran dyal l-player b l-mouse (Yaw)
        playerTarget.rotation = Quaternion.Euler(0, yaw, 0);

        // L-camera katbqa dima f dak l-makan b ḍabṭ w katdor m3a l-mouse
        transform.localPosition = initialLocalPosition;
        transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
}