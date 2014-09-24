using UnityEngine;
using System.Collections;

public class L1 : MonoBehaviour
{
		public Animation target;

		// Use this for initialization
		void Start ()
		{
				target.Play ("Armature|Clouse");
		}
	
		void OnTriggerEnter ()
		{
				target.CrossFade ("Armature|Open");
		}

		void OnTriggerExit ()
		{
				target.CrossFade ("Armature|Clouse");
		}
}
