using UnityEngine;
using System.Collections;

public class KinectFPSController : MonoBehaviour
{
		public SkeletonWrapper sw;
		public float sensorPollTime = 0.2f;
		public int player;
		public Transform cameraToMove;
		public float speed = 7f;
		public float rotSpeed = 0.25f;
		public float deadZoneAngles = 15f;
		public float armLengthDeadZone = 0.15f;
		public float maxArmLength = 0.4f;
		private Vector3 rightArmVector;

		/// <summary>
		/// Couroutine that polls sensor for bones position every <paramref name="sensorPollTime"/> seconds. 
		/// Also sets rotation of right arm.
		/// </summary>
		/// <returns>The poll couroutine.</returns>
		IEnumerator SensorPollCouroutine ()
		{
				while (Application.isPlaying) {
						rightArmVector = PollSensorForVector (8, 11);

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
		void FixedUpdate ()
		{
				float rotationFactor = 0f;

				Quaternion rightArmRotation;

				// Set rotation of arm
				if (rightArmVector.sqrMagnitude < 0.05f) {
						rightArmRotation = Quaternion.Euler (0f, 180f, 0f);
				} else {
						// Rotation from right shoulder to right hand.
						rightArmRotation = Quaternion.FromToRotation (-Vector3.forward, rightArmVector);
				}

				// Set horizontal rotation. Turn the character!
				if (Mathf.Abs (rightArmRotation.eulerAngles.y - 180f) > deadZoneAngles) {
						rotationFactor = (rightArmRotation.eulerAngles.y - 180f);
				}

				// Set physical rotation
				rigidbody.angularVelocity = new Vector3 (0f, rotationFactor * rotSpeed, 0f);

				// Rotate camera
				float cameraRotFactor = -rightArmRotation.eulerAngles.x;
				cameraRotFactor = Mathf.Clamp (cameraRotFactor, -35f, 0f);
				cameraToMove.localRotation = Quaternion.Euler (cameraRotFactor, 0f, 0f);

				float speedModifier = 0f;

				// Set speed multiplier based on arm length.
				if (rightArmVector.sqrMagnitude > armLengthDeadZone * armLengthDeadZone) {
						speedModifier = rightArmVector.sqrMagnitude / (maxArmLength * maxArmLength);
				}
				rigidbody.velocity = transform.forward * speed * speedModifier;

				Debug.Log (rightArmRotation.eulerAngles.x + "; " + rigidbody.velocity);
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
