using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MyRigidbodyBodyEvaluator : MonoBehaviour
{
    public Rigidbody playerRigidbody;

    public void GetBodyPositionAndRotation(out Vector3 position, out Quaternion rotation)
    {
        if (playerRigidbody != null)
        {
            position = playerRigidbody.position;
            rotation = playerRigidbody.rotation;
            return;
        }

        position = transform.position;
        rotation = transform.rotation;
    }
}