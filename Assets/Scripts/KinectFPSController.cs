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
		private Quaternion rightArmRotation;

		IEnumerator SensorPollCouroutine ()
		{
				while (true) {
						// Rotation from right shoulder to right hand.
			rightArmRotation = PollSensorForLineRotation (8, 11);

						yield return new WaitForSeconds (sensorPollTime);
				}
		}
	
		// Use this for initialization
		void Start ()
		{
				StartCoroutine (SensorPollCouroutine);
		}
	
		// Update is called once per frame
		void Update ()
		{
				float rotationFactor = 0f;
		
				if (sw.pollSkeleton ()) {
						Vector3 direction = GetArmDirection ();
						Quaternion armRotation = Quaternion.FromToRotation (-Vector3.forward, direction);

						if (Mathf.Abs (armRotation.eulerAngles.y - 180f) > deadZoneAngles) {
								rotationFactor = (armRotation.eulerAngles.y - 180f);
						}

						Debug.Log (armRotation.eulerAngles + "; " + rotationFactor);
			
						rigidbody.velocity = transform.forward * speed;
				}

				rigidbody.angularVelocity = new Vector3 (0f, rotationFactor * rotSpeed, 0f);
		}

		/// <summary>
		/// Polls the sensor and gets rotation of line between two bones.
		/// </summary>
		/// <returns>Line rotation.</returns>
		private Quaternion PollSensorForLineRotation (int firstBone, int secondBone)
		{
				if (sw.pollSkeleton ()) {
						// Get the arm direction. It's a normalized vector from first bone to second.
						Vector3 armDirection = (sw.bonePos [player, secondBone] - sw.bonePos [player, firstBone]).normalized;
						// Get rotation of the line.
						Quaternion armRotation = Quaternion.FromToRotation (-Vector3.forward, armDirection);

						return armRotation;
				}

				return null;
		}
}
