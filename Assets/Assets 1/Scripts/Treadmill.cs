using UnityEngine;

public class Treadmill : MonoBehaviour
{
    [Tooltip("Direction and speed of the treadmill")]
    public Vector3 moveDirection = Vector3.forward;

    [Tooltip("How strong the treadmill pushes")]
    public float speed = 3f;

    private void OnCollisionStay(Collision collision)
    {
        // Only affect objects with a Rigidbody
        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        // Optional: limit to player only
        // if (!collision.gameObject.CompareTag("Player")) return;

        // Apply treadmill motion WITHOUT locking the player
        Vector3 treadmillVelocity = moveDirection.normalized * speed;

        // Add movement while preserving player's own control
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x + treadmillVelocity.x,
            rb.linearVelocity.y,
            rb.linearVelocity.z + treadmillVelocity.z
        );
    }
}