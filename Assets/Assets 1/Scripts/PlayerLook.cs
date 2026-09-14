using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerLook : MonoBehaviour
{
    [Tooltip("Degrees per (second * mouse delta)")]
    public float sensitivity = 200f;

    [Tooltip("The player's Rigidbody to face the same yaw as the camera")]
    public Rigidbody playerRigidbody;

    [Tooltip("Clamp for vertical (pitch) in degrees")]
    public float pitchMin = -90f, pitchMax = 90f;

    float pitch;        // camera pitch (X, world)
    float targetYaw;    // desired world yaw driven by mouse

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Initialize from current world rotation to avoid a startup snap.
        Vector3 camWorldEuler = transform.rotation.eulerAngles;
        pitch = Normalize180(camWorldEuler.x);
        targetYaw = camWorldEuler.y;

        if (playerRigidbody != null)
            playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void LateUpdate()
    {
        // Read mouse (render-rate), build world-space look
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        targetYaw += mouseX;
        pitch = Mathf.Clamp(pitch - mouseY, pitchMin, pitchMax);

        // Set camera's WORLD rotation so it doesn't double-count the parent yaw.
        transform.rotation = Quaternion.Euler(pitch, targetYaw, 0f);
    }

    void FixedUpdate()
    {
        if (!playerRigidbody) return;

        // Rotate the player rigidbody to match the camera's world yaw.
        // Physics will apply this on the next fixed step; interpolation makes it look smooth.
        Quaternion desiredBodyRot = Quaternion.Euler(0f, targetYaw, 0f);
        playerRigidbody.MoveRotation(desiredBodyRot);
    }

    static float Normalize180(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        if (angle < -180f) angle += 360f;
        return angle;
    }
}
