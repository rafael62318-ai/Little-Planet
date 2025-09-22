using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody))]
public class GravityAttractor : MonoBehaviour
{
	public float gravity = -9.8f;

	/*public void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").G
    }

    public void Attract(Rigidbody body)
	{
		Vector3 gravityUp = (body.position - transform.position).normalized;
		Vector3 localUp = body.transform.up;

		// Apply downwards gravity to body
		body.AddForce(gravityUp * gravity);
		// Allign bodies up axis with the centre of planet
		body.rotation = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
	}  */

	public void Attract(Transform body)
	{
		Vector3 gravityUp = (body.position - transform.position).normalized;
		Vector3 bodyUp = body.transform.up;

		Vector3 targetDir = gravityUp;

		body.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.rotation;
		body.GetComponent<Rigidbody>().AddForce(targetDir * gravity);
	}
}
