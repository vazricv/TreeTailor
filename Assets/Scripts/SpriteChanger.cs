using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF.Game2D.SpritePlugins
{
	public class SpriteChanger : MonoBehaviour
	{
		public SpriteRenderer targetSpriteRenderer;
		public Sprite[] spriteSource;
		public bool selectRandomly = true;
		public int selectedSprite = 0;
		public bool shrinkDown = false;
		public float shrinkingDurationPS = 1000f;
		public bool fadeOut = false;
		public float fadeOutDurationPS = 1000f;
		public bool resetAfterCycle = true;

		private float scaleFactor = 0;
		private float fadeFactor = 0;
		private Vector3 initialScale;
		private Color initialColor;
		private bool animationEnded = false;
		private float initialScaleDuration;
		private float initialFadeDuration;
		// Use this for initialization
		void Start ()
		{
			if (targetSpriteRenderer != null) {
				if (spriteSource.Length > 0 && selectRandomly)
					selectedSprite = Random.Range (0, spriteSource.Length);
				if (spriteSource.Length > 0)
					targetSpriteRenderer.sprite = spriteSource [selectedSprite];
				initialColor = targetSpriteRenderer.color;
				initialScale = targetSpriteRenderer.transform.localScale;
				initialScaleDuration = shrinkingDurationPS;
				initialFadeDuration = fadeOutDurationPS;
			}
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
			if (animationEnded) {
				if (resetAfterCycle) {
					targetSpriteRenderer.transform.localScale = initialScale;
					targetSpriteRenderer.color = initialColor;
					shrinkingDurationPS = initialScaleDuration;
					fadeOutDurationPS = initialFadeDuration;
					Start ();
					animationEnded = false;
				}
				return;
			}

			float fps = (1.0f / Time.fixedDeltaTime);
			if (scaleFactor == 0)
				scaleFactor = (targetSpriteRenderer.transform.localScale.x * 10 / shrinkingDurationPS);			
			if (fadeFactor == 0)
				fadeFactor = (targetSpriteRenderer.color.a * 10 / shrinkingDurationPS);
			
			if (shrinkDown && shrinkingDurationPS > 0) {
				shrinkingDurationPS -= Time.fixedDeltaTime;
				targetSpriteRenderer.transform.localScale -= (new Vector3 (0.1f, 0.1f, 0.1f) * scaleFactor / fps);
				if (targetSpriteRenderer.transform.localScale.x < 0 || targetSpriteRenderer.transform.localScale.y < 0) {
					targetSpriteRenderer.transform.localScale = Vector3.zero;
					shrinkingDurationPS = 0;
				}
			}
			if (fadeOut && fadeOutDurationPS > 0) {
				fadeOutDurationPS -= Time.fixedDeltaTime;
				targetSpriteRenderer.color = new Color (targetSpriteRenderer.color.r, targetSpriteRenderer.color.g, targetSpriteRenderer.color.b, targetSpriteRenderer.color.a - (0.1f * fadeFactor / fps));
				if (targetSpriteRenderer.color.a <= 0)
					fadeOutDurationPS = 0;
			}

			if (shrinkDown && shrinkingDurationPS <= 0.0f) {
				animationEnded = true;
			}
			if (fadeOut && fadeOutDurationPS < 0.0f) {
				animationEnded = true;
			}

		}
	}
}