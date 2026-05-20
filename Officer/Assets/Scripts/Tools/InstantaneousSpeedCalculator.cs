using UnityEngine;

public class InstantaneousSpeedCalculator : MonoBehaviour
{
    private Transform parent;
    private Vector3 previousPosition;
    private Vector3 currentPosition;
    private Vector3 instantaneousSpeed;

    public Vector3 InstantaneousSpeed { get => instantaneousSpeed; set => instantaneousSpeed = value; }

    void Start()
    {
        // Cache the starting position as the previous frame position.
        parent = transform.parent;
        previousPosition = parent.position;
    }

    void Update()
    {
        // Read the current frame position.
        currentPosition = transform.position;

        // Compute instantaneous speed.
        instantaneousSpeed = (currentPosition - previousPosition) / Time.deltaTime;

        // Store this frame position for the next update.
        previousPosition = currentPosition;
    }
}
