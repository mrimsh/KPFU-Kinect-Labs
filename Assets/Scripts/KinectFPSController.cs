using UnityEngine;
using System.Collections;

public class KinectFPSController : MonoBehaviour
{
		public SkeletonWrapper sw;
		public float sensorPollTime = 0.2f;
		public int player;
		public Transform cameraToMove;
		public float speed = 7f;
		public float maxRotSpeed = 0.25f;
		public float deadZoneAngles = 15f;
		public float maxRotationAngles = 30f;
		public float armMovingSensibleAngle = 15f;
		public float armLengthDeadZone = 0.15f;
		public float maxArmLength = 0.4f;
		public float maxHeadAngle = 30f;
		public float headTurnMultiplier = 2f;
		private Vector3 rightArmVector;
		private Vector3 shouldersVector;
		private Vector3 headVector;

		/// <summary>
		/// Couroutine that polls sensor for bones position every <paramref name="sensorPollTime"/> seconds.
		/// </summary>
		/// <returns>The poll couroutine.</returns>
		IEnumerator SensorPollCouroutine ()
		{
				while (Application.isPlaying) {
						if (sw.pollSkeleton ()) {
								// vector from left shoulder, to right
								shouldersVector = sw.bonePos [player, 8] - sw.bonePos [player, 4];
								// vector from right shoulder, to hand
								rightArmVector = sw.bonePos [player, 11] - sw.bonePos [player, 8];
								// vector from shoulder center, to head
								headVector = sw.bonePos [player, 3] - sw.bonePos [player, 2];
						}

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
				// if we've got shoulders position from sensor..
				if (shouldersVector != Vector3.zero) {
						// variable for turning angle
						float turningFactor = 0f;

						// we don't need horizontal component of angle between shoulders 
						shouldersVector.y = 0f;
						// calculating angle between shoulders. It is clamped for variable turning speed
						float shouldersAngle = Mathf.Clamp (Vector3.Angle (shouldersVector, Vector3.right), 0f, maxRotationAngles);
				
						if (shouldersAngle > deadZoneAngles) {
								// set direction
								turningFactor = shouldersAngle * ((shouldersVector.z < 0f) ? 1f : -1f);
								// set speed of turning
								turningFactor = maxRotSpeed * turningFactor / maxRotationAngles;
						}

						rigidbody.angularVelocity = new Vector3 (0f, turningFactor, 0f);
				}

		
				// if we've got right arm position from sensor..
				if (rightArmVector != Vector3.zero) {
						// only if hand extended and looking forward..
						if (rightArmVector.sqrMagnitude > armLengthDeadZone * armLengthDeadZone &&
								Vector3.Angle (rightArmVector, Vector3.forward) < armMovingSensibleAngle) {
								// ..move us forward
								rigidbody.velocity = transform.forward * speed;
						} else {
								rigidbody.velocity = Vector3.zero;
						}
				}

		
				if (headVector != Vector3.zero) {
						float cameraRotFactor;

						if (headVector.z >= 0) {
								headVector.x = 0f;
								cameraRotFactor = Vector3.Angle (headVector, Vector3.up) * headTurnMultiplier;
								cameraRotFactor = Mathf.Clamp (cameraRotFactor, 0f, maxHeadAngle);
						} else {
								cameraRotFactor = 0f;
						}

						cameraToMove.localRotation = Quaternion.Lerp (cameraToMove.localRotation, Quaternion.Euler (cameraRotFactor, 0f, 0f), Time.fixedDeltaTime);
				}
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
