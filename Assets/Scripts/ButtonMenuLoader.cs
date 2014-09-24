using UnityEngine;
using System.Collections;

public class ButtonMenuLoader : MonoBehaviour
{
		public int levelToLoad = -1;
		public Color onClickColor = Color.gray;
		private Transform _myBGSpriteTR;
		private SpriteRenderer _myBGSprite;
		private bool _isHovered = false;

		// Use this for initialization
		void Start ()
		{
				_myBGSpriteTR = transform.Find ("buttons_bg");
				_myBGSprite = _myBGSpriteTR.GetComponent<SpriteRenderer> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
					
		}
	
		void OnMouseEnter ()
		{
				_myBGSpriteTR.localScale = new Vector3 (1f, -1f, 1f);
				_isHovered = true;
		}
	
		void OnMouseExit ()
		{
				_myBGSpriteTR.localScale = Vector3.one;
				_isHovered = false;
		}
	
		void OnMouseDown ()
		{
				_myBGSprite.color = onClickColor;
		}

		void OnMouseUp ()
		{
				_myBGSprite.color = Color.white;
		
				if (_isHovered) {
						Application.LoadLevel (levelToLoad);
				}
		}
}
