using UnityEngine;
using System.Collections;

public class L5 : MonoBehaviour
{
		public Transform point1;
		public Transform point2;
		public Transform player;
		public Vector3 offset;
		private bool isPoint1 = true;

		// Use this for initialization
		void Start ()
		{
		
				player.position = point1.position + offset;
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (Input.GetKeyDown (KeyCode.Space)) {
						if (isPoint1) {
								player.position = point2.position + offset;
						} else {
								player.position = point1.position + offset;
						}
						isPoint1 = !isPoint1;
				}
		}
}
