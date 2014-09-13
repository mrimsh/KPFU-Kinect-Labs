using UnityEngine;
using System.Collections;

public class KinectFPSController : MonoBehaviour
{
		public SkeletonWrapper sw;
		public int player;
		public float speed = 7f;
		public float rotSpeed = 0.25f;
		public float deadZoneAngles = 15f;
		public float sensorPollTime = 0.2f;
		public Vector3 rightArmVector;
		private Quaternion rightArmRotation;

		/// <summary>
		/// Couroutine that polls sensor for bones position every <paramref name="sensorPollTime"/> seconds. 
		/// Also sets rotation of right arm.
		/// </summary>
		/// <returns>The poll couroutine.</returns>
		IEnumerator SensorPollCouroutine ()
		{
				while (Application.isPlaying) {
			
						rightArmVector = PollSensorForVector (8, 11);
						// Rotation from right shoulder to right hand.
						rightArmRotation = Quaternion.FromToRotation (-Vector3.forward, rightArmVector);

						yield return new WaitForSeconds (sensorPollTime);
				}

				yield return null;
		}
	
		// Use this for initialization
		void Start ()
		{
				StartCoroutine (SensorPollCouroutine ());
		}
	
		// Update is called once per frame
		void Update ()
		{
				float rotationFactor = 0f;
		
				if (Mathf.Abs (rightArmRotation.eulerAngles.y - 180f) > deadZoneAngles) {
						rotationFactor = (rightArmRotation.eulerAngles.y - 180f);
				}
		
				Debug.Log (rightArmRotation.eulerAngles + "; " + rotationFactor);
		
				rigidbody.velocity = transform.forward * speed;

				rigidbody.angularVelocity = new Vector3 (0f, rotationFactor * rotSpeed, 0f);
		}

		/// <summary>
		/// Polls the sensor and gets vector of line between two bones.
		/// </summary>
		/// <returns>Line between two bones.</returns>
		private Vector3 PollSensorForVector (int firstBone, int secondBone)
		{
				if (sw.pollSkeleton ()) {
						// Get the arm direction. It's a normalized vector from first bone to second.
						Vector3 lineDirection = sw.bonePos [player, secondBone] - sw.bonePos [player, firstBone];

						return lineDirection;
				}

				return Vector3.zero;
		}
}
