using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BF.Utility;

namespace BF.Game2D.Placers.ObjectPlacer
{
	public class ObjectPlacer : MonoBehaviour
	{
		public enum PathOriantation
		{
			Y_Axes,
			Z_Axes
		}

		public enum ObjectPathType
		{
			Circle,
			Box,
			line
		}

		public Vector3 centerOffset = new Vector3 ();
		public float colliderRadious = 1;
		public string objectPoolerName = "ObjectObjectPooler";
		public bool autoGetPooler = true;
		public ObjectPooler ObjectObject;
		public float ObjectObjectSize = 0.2f;
		public bool autoObjectSize = true;
		public bool alwaysDrawGizmos = false;
		public bool drawSolidGizmos = false;
		public int selectedSet = 0;
		public int placmentPercentage = 100;
		public bool attachedToParent = false;
		public bool regenerateAfterActive = true;
		public bool placeAllActiveSets = false;
		public bool addRowParents = false;
		public ObjectPlacerSet[] placerSets = new ObjectPlacerSet[0];

		private Vector3 myCenter { get { return gameObject.transform.position + centerOffset; } }

		private List<Collider2D> myCollider2Ds = new List<Collider2D> ();
		private Collider2D myCollider2D;

		void Reset ()
		{
			//theCenter = gameObject.transform.position;
			myCollider2Ds = gameObject.GetComponents<Collider2D> ().ToList ();
			myCollider2Ds = myCollider2Ds.Where (c => c.isActiveAndEnabled && c.isTrigger == false).ToList ();

			if (myCollider2Ds.Count > 1) {
				myCollider2Ds = myCollider2Ds.OrderByDescending (c => c is CircleCollider2D).ThenByDescending (c => c is BoxCollider2D).ToList ();
				myCollider2D = myCollider2Ds [0];
			} else
				myCollider2D = myCollider2Ds [0];

			colliderRadious = Mathf.Max (myCollider2D.bounds.extents.x, myCollider2D.bounds.extents.y);

			if (autoGetPooler) {
				//GameObject.Find (objectPoolerName);  //this can be used too
				ObjectObject = FindObjectsOfType<ObjectPooler> ().First (o => o.name == objectPoolerName);
			}

			if (ObjectObject != null) {
				Collider2D ObjectCollider = ObjectObject.pooledObject.GetComponents<Collider2D> ().ToList ().Where (c => c.isActiveAndEnabled).OrderByDescending (c => c is CircleCollider2D).ThenByDescending (c => c is BoxCollider2D).ToList ().FirstOrDefault ();
				if (ObjectCollider != null)
					ObjectObjectSize = Mathf.Max (ObjectCollider.bounds.extents.x, ObjectCollider.bounds.extents.y);
			}


		}

		void OnDrawGizmos ()
		{
#if UNITY_EDITOR
			if (alwaysDrawGizmos)
				OnDrawGizmosSelected ();
#endif
		}

		void OnDrawGizmosSelected ()
		{
#if UNITY_EDITOR
			if (!this.isActiveAndEnabled)
				return;

			if (selectedSet < 0)
				selectedSet = 0;

			if (selectedSet > placerSets.Count () - 1)
				selectedSet = placerSets.Count () - 1;
			if (placeAllActiveSets) {
				int curSelSet = selectedSet;
				for (int i = 0; i < placerSets.Count (); i++) {
					selectedSet = i;
					DrawGizmos ();
				}
				selectedSet = curSelSet;
			} else {
				DrawGizmos ();
			}
		}

		void DrawGizmos ()
		{
			if (placerSets.Count () == 0)
				return;


			if (placerSets [selectedSet].pathType == ObjectPathType.Circle) {
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere (myCenter + placerSets [selectedSet].localOffset, colliderRadious);
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere (myCenter + placerSets [selectedSet].localOffset, colliderRadious + placerSets [selectedSet].marginRadious / 2);
			} else {
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube (myCenter + placerSets [selectedSet].localOffset, new Vector3 (colliderRadious * 2, colliderRadious * 2, colliderRadious * 2));
				Gizmos.color = Color.blue;
				Gizmos.DrawWireCube (myCenter + placerSets [selectedSet].localOffset, new Vector3 (colliderRadious * 2 + placerSets [selectedSet].marginRadious, colliderRadious * 2 + placerSets [selectedSet].marginRadious, colliderRadious * 2 + placerSets [selectedSet].marginRadious));
			}

			float x = (colliderRadious + placerSets [selectedSet].marginRadious / 2) * Mathf.Cos (Mathf.Deg2Rad * 0);
			float y = (colliderRadious + placerSets [selectedSet].marginRadious / 2) * Mathf.Sin (Mathf.Deg2Rad * 0);
			x += myCenter.x;
			if (placerSets [selectedSet].oriantation == PathOriantation.Y_Axes) {
				y += myCenter.y;
				Gizmos.DrawSphere (new Vector3 (x, y, myCenter.z) + placerSets [selectedSet].localOffset, 0.02f);
				UnityEditor.Handles.Label (new Vector3 (x, y, myCenter.z) + placerSets [selectedSet].localOffset, "0-360");
			} else {
				y += myCenter.z;
				Gizmos.DrawSphere (new Vector3 (x, myCenter.y, y) + placerSets [selectedSet].localOffset, 0.02f);
				UnityEditor.Handles.Label (new Vector3 (x, myCenter.y, y) + placerSets [selectedSet].localOffset, "0-360");
			}

			x = (colliderRadious + placerSets [selectedSet].marginRadious / 2) * Mathf.Cos (Mathf.Deg2Rad * 90);
			y = (colliderRadious + placerSets [selectedSet].marginRadious / 2) * Mathf.Sin (Mathf.Deg2Rad * 90);
			x += myCenter.x;
			if (placerSets [selectedSet].oriantation == PathOriantation.Y_Axes) {
				y += myCenter.y;
				Gizmos.DrawSphere (new Vector3 (x, y, myCenter.z) + placerSets [selectedSet].localOffset, 0.02f);
				UnityEditor.Handles.Label (new Vector3 (x, y, myCenter.z) + placerSets [selectedSet].localOffset, "90");
			} else {
				y += myCenter.z;
				Gizmos.DrawSphere (new Vector3 (x, myCenter.y, y) + placerSets [selectedSet].localOffset, 0.02f);
				UnityEditor.Handles.Label (new Vector3 (x, myCenter.y, y) + placerSets [selectedSet].localOffset, "90");
			}

			x = (colliderRadious + placerSets [selectedSet].marginRadious / 2) * Mathf.Cos (Mathf.Deg2Rad * 180);
			y = (colliderRadious + placerSets [selectedSet].marginRadious / 2) * Mathf.Sin (Mathf.Deg2Rad * 180);
			x += myCenter.x;
			if (placerSets [selectedSet].oriantation == PathOriantation.Y_Axes) {
				y += myCenter.y;
				Gizmos.DrawSphere (new Vector3 (x, y, myCenter.z) + placerSets [selectedSet].localOffset, 0.02f);
				UnityEditor.Handles.Label (new Vector3 (x, y, myCenter.z) + placerSets [selectedSet].localOffset, "180");
			} else {
				y += myCenter.z;
				Gizmos.DrawSphere (new Vector3 (x, myCenter.y, y) + placerSets [selectedSet].localOffset, 0.02f);
				UnityEditor.Handles.Label (new Vector3 (x, myCenter.y, y) + placerSets [selectedSet].localOffset, "180");
			}

			x = (colliderRadious + placerSets [selectedSet].marginRadious / 2) * Mathf.Cos (Mathf.Deg2Rad * 270);
			y = (colliderRadious + placerSets [selectedSet].marginRadious / 2) * Mathf.Sin (Mathf.Deg2Rad * 270);
			x += myCenter.x;
			if (placerSets [selectedSet].oriantation == PathOriantation.Y_Axes) {
				y += myCenter.y;
				Gizmos.DrawSphere (new Vector3 (x, y, myCenter.z) + placerSets [selectedSet].localOffset, 0.02f);
				UnityEditor.Handles.Label (new Vector3 (x, y, myCenter.z) + placerSets [selectedSet].localOffset, "270");

			} else {
				y += myCenter.z;
				Gizmos.DrawSphere (new Vector3 (x, myCenter.y, y) + placerSets [selectedSet].localOffset, 0.02f);
				UnityEditor.Handles.Label (new Vector3 (x, myCenter.y, y) + placerSets [selectedSet].localOffset, "270");

			}

			Gizmos.color = Color.yellow;

			float spaceBetween = 360.0f / placerSets [selectedSet].numberOfObject;

			if (autoObjectSize)
				ObjectObjectSize = (spaceBetween * ((placerSets [selectedSet].marginRadious + colliderRadious) / 3)) * Mathf.Deg2Rad;
			
			Sprite sp = null;
			if (ObjectObject != null) {
				var spr = ObjectObject.pooledObject.GetComponentInChildren<SpriteRenderer> ();
				if (spr != null) {
					sp = spr.sprite;
					if (autoObjectSize)
						ObjectObjectSize = Mathf.Max (sp.bounds.extents.x, sp.bounds.extents.y);
				}
			}


			for (int i = 0; i < placerSets [selectedSet].numberOfObject; i++) {

				float angle = (placerSets [selectedSet].startingAngle + (i * spaceBetween + i * placerSets [selectedSet].spaceing));
				if (placerSets [selectedSet].pathType == ObjectPathType.line) {
					angle = (placerSets [selectedSet].startingAngle + (i * spaceBetween));
					if (angle >= 0 && angle < 90)
						angle = 0;
					if (angle >= 90 && angle < 180)
						angle = 90;
					if (angle >= 180 && angle < 270)
						angle = 180;
					if (angle >= 270 && angle < 360)
						angle = 270;
				}

				if (angle >= 0 && angle <= 180)
					angle += placerSets [selectedSet].zeroTo180Padding;
				if (angle >= 180 && angle <= 360)
					angle += placerSets [selectedSet].oneeightyTo360Padding;
				if (angle >= 90 && angle <= 270)
					angle += placerSets [selectedSet].ninetyTo270Padding;
				if ((angle >= 270 && angle <= 360) || (angle >= 0 && angle <= 90))
					angle += placerSets [selectedSet].twoseventyTo90Padding;

				float angleInDeg = angle;
				angle *= Mathf.Deg2Rad;

				float ObjectObjectSizeDeg = (ObjectObjectSize * Mathf.Rad2Deg) + placerSets [selectedSet].fineTuneRemovalP;

				if (placerSets [selectedSet].clampAfter360 && angleInDeg > 360)
					continue;
				if (placerSets [selectedSet].clampAfter180 && angleInDeg > 180)
					continue;
				if (placerSets [selectedSet].clampAfter90 && angleInDeg > 90)
					continue;
				if (placerSets [selectedSet].clampAfter270 && angleInDeg > 270)
					continue;
				if (placerSets [selectedSet].clampBefore0 && angleInDeg < 0)
					continue;
				if (placerSets [selectedSet].endingAngle > 0 && angleInDeg > placerSets [selectedSet].endingAngle)
					continue;

				if (angleInDeg > 360)
					angleInDeg = angleInDeg % 360;
				if (angleInDeg < 0)
					angleInDeg = 360 - (Mathf.Abs (angleInDeg) % 360);
				if (placerSets [selectedSet].removeEastWest && ((angleInDeg >= 0.0f - ObjectObjectSizeDeg - placerSets [selectedSet].fineTuneRemovalN && angleInDeg <= 0.0f + ObjectObjectSizeDeg + placerSets [selectedSet].fineTuneRemovalP) ||
				    (angleInDeg >= 180.0f - ObjectObjectSizeDeg - placerSets [selectedSet].fineTuneRemovalN && angleInDeg <= 180.0f + ObjectObjectSizeDeg + placerSets [selectedSet].fineTuneRemovalP)
				    || (angleInDeg >= 360.0f - ObjectObjectSizeDeg - placerSets [selectedSet].fineTuneRemovalN && angleInDeg <= 360.0f + ObjectObjectSizeDeg + placerSets [selectedSet].fineTuneRemovalP)))
					continue;
				if (placerSets [selectedSet].removeNorthSouth && ((angleInDeg >= 90.0f - ObjectObjectSizeDeg - placerSets [selectedSet].fineTuneRemovalN && angleInDeg <= 90.0f + ObjectObjectSizeDeg + placerSets [selectedSet].fineTuneRemovalP) ||
				    (angleInDeg >= 270.0f - ObjectObjectSizeDeg - placerSets [selectedSet].fineTuneRemovalN && angleInDeg <= 270.0f + ObjectObjectSizeDeg + placerSets [selectedSet].fineTuneRemovalP)))
					continue;

				if (placerSets [selectedSet].removeTopSide && (angleInDeg >= 45.0f - ObjectObjectSizeDeg - placerSets [selectedSet].fineTuneRemovalN && angleInDeg <= 135.0f + ObjectObjectSizeDeg + placerSets [selectedSet].fineTuneRemovalP))
					continue;
				if (placerSets [selectedSet].removeBottomSide && (angleInDeg >= 180 + 45.0f - ObjectObjectSizeDeg - placerSets [selectedSet].fineTuneRemovalN && angleInDeg <= 270 + 45.0f + ObjectObjectSizeDeg + placerSets [selectedSet].fineTuneRemovalP))
					continue;
				if (placerSets [selectedSet].removeLeftSide && (angleInDeg >= 135.0f - ObjectObjectSizeDeg - placerSets [selectedSet].fineTuneRemovalN && angleInDeg <= 225.0f + ObjectObjectSizeDeg + placerSets [selectedSet].fineTuneRemovalP))
					continue;
				if (placerSets [selectedSet].removeRightSide && (angleInDeg >= 270 + 45.0f - ObjectObjectSizeDeg - placerSets [selectedSet].fineTuneRemovalN && angleInDeg <= 360.0f + ObjectObjectSizeDeg + placerSets [selectedSet].fineTuneRemovalP))
					continue;
				if (placerSets [selectedSet].removeRightSide && (angleInDeg >= 0.0f - ObjectObjectSizeDeg - placerSets [selectedSet].fineTuneRemovalN && angleInDeg <= 45.0f + ObjectObjectSizeDeg + placerSets [selectedSet].fineTuneRemovalP))
					continue;

				if (placerSets [selectedSet].pathType == ObjectPathType.Circle) {
					x = ((colliderRadious + placerSets [selectedSet].marginRadious / 2) - placerSets [selectedSet].bendingXOffset) * Mathf.Cos (angle);
					y = ((colliderRadious + placerSets [selectedSet].marginRadious / 2) - placerSets [selectedSet].bendingYOffset) * Mathf.Sin (angle);
				} else if (placerSets [selectedSet].pathType == ObjectPathType.Box) {
					x = ((colliderRadious + placerSets [selectedSet].marginRadious / 2) + ((colliderRadious + placerSets [selectedSet].marginRadious / 2) / 100 * 40f) - placerSets [selectedSet].bendingXOffset) * Mathf.Cos (angle);
					y = ((colliderRadious + placerSets [selectedSet].marginRadious / 2) + ((colliderRadious + placerSets [selectedSet].marginRadious / 2) / 100 * 40f) - placerSets [selectedSet].bendingYOffset) * Mathf.Sin (angle);
					x = Mathf.Clamp (x, -(colliderRadious + placerSets [selectedSet].marginRadious / 2), (colliderRadious + placerSets [selectedSet].marginRadious / 2));
					y = Mathf.Clamp (y, -(colliderRadious + placerSets [selectedSet].marginRadious / 2), (colliderRadious + placerSets [selectedSet].marginRadious / 2));
				} else if (placerSets [selectedSet].pathType == ObjectPathType.line) {
					float spaceInLine = (i % (placerSets [selectedSet].numberOfObject / 4)) * (placerSets [selectedSet].spaceing + (ObjectObjectSize * 2));
					x = ((spaceInLine + colliderRadious + placerSets [selectedSet].marginRadious / 2) - placerSets [selectedSet].bendingXOffset) * Mathf.Cos (angle);
					y = ((spaceInLine + colliderRadious + placerSets [selectedSet].marginRadious / 2) - placerSets [selectedSet].bendingYOffset) * Mathf.Sin (angle);
				}

				x += myCenter.x;
				if (placerSets [selectedSet].oriantation == PathOriantation.Y_Axes) {
					y += myCenter.y;
				} else {
					y += myCenter.z;
				}

				//Gizmos.DrawWireSphere (new Vector3 (x, y, myCenter.z), ObjectObjectSize);
				//UnityEditor.Handles.Label (new Vector3 (x, y, myCenter.z), i + " = " + x + "_" + y);
				if (placerSets [selectedSet].oriantation == PathOriantation.Y_Axes) {
					if (drawSolidGizmos) {
						Gizmos.DrawSphere (new Vector3 (x, y, myCenter.z) + placerSets [selectedSet].localOffset, ObjectObjectSize);
					} else {
						Gizmos.DrawWireSphere (new Vector3 (x, y, myCenter.z) + placerSets [selectedSet].localOffset, ObjectObjectSize);
					}

					UnityEditor.Handles.Label (new Vector3 (x, y, myCenter.z) + placerSets [selectedSet].localOffset, i + " = " + x + "_" + y);
				} else {
					if (drawSolidGizmos) {
						Gizmos.DrawSphere (new Vector3 (x, myCenter.y, y) + placerSets [selectedSet].localOffset, ObjectObjectSize);
					} else {
						Gizmos.DrawWireSphere (new Vector3 (x, myCenter.y, y) + placerSets [selectedSet].localOffset, ObjectObjectSize);
					}

					UnityEditor.Handles.Label (new Vector3 (x, myCenter.y, y) + placerSets [selectedSet].localOffset, i + " = " + x + "_" + y);
				}

				//if (sp != null)
				//	DrawSprite (sp, new Vector3 (x, y, myCenter.z));
				// Gizmos.DrawGUITexture(new Rect(x - ObjectObjectSize / 2, y - ObjectObjectSize / 2, ObjectObjectSize * 2, ObjectObjectSize * 2), sp.texture);
			}
#endif
		}
		// Use this for initialization
		void Awake ()
		{

		}


		void Start ()
		{
			if (autoGetPooler && ObjectObject == null) {
				ObjectObject = FindObjectsOfType<ObjectPooler> ().First (o => o.name == objectPoolerName);
			}
			if (!regenerateAfterActive) {
				if (Random.Range (0, 100) <= placmentPercentage) {

					if (ObjectObject != null) {
						ObjectPlacerSet selectedSet = placerSets.Where (p => p.active).ToList () [Random.Range (0, placerSets.Count (p => p.active))];
						if (placeAllActiveSets) {
							for (int i = 0; i < placerSets.Count (); i++) {
								if (addRowParents) {
									GameObject newRowObject = new GameObject ("Row#" + i); 
									if (attachedToParent)
										newRowObject.transform.parent = gameObject.transform;
									CalculateAndPlaceObjects (placerSets [i], i, newRowObject.transform);
								} else {
									CalculateAndPlaceObjects (placerSets [i], i);
								}
							}
						} else {
							CalculateAndPlaceObjects (selectedSet);
						}
					}
				}
			}
		}

		void OnEnable ()
		{
			if (regenerateAfterActive) {
				if (Random.Range (0, 100) <= placmentPercentage) {
					if (autoGetPooler && ObjectObject == null) {
						ObjectObject = FindObjectsOfType<ObjectPooler> ().First (o => o.name == objectPoolerName);
					}
					if (ObjectObject != null) {
						ObjectPlacerSet selectedSet = placerSets.Where (p => p.active).ToList () [Random.Range (0, placerSets.Count (p => p.active))];
						if (placeAllActiveSets) {
							for (int i = 0; i < placerSets.Count (); i++) {
								if (addRowParents) {
									GameObject newRowObject = new GameObject ("Row#" + i); 
									if (attachedToParent)
										newRowObject.transform.parent = gameObject.transform;
									CalculateAndPlaceObjects (placerSets [i], i, newRowObject.transform);
								} else {
									CalculateAndPlaceObjects (placerSets [i], i);
								}
							}
						} else {
							CalculateAndPlaceObjects (selectedSet);
						}
					}
				}
			}
		}

		// Update is called once per frame
		void Update ()
		{

		}

		void DrawSprite (Sprite sprite, Vector3 position)
		{

			Rect dstRect = new Rect (position.x - sprite.bounds.max.x,
				               position.y + sprite.bounds.max.y,
				               sprite.bounds.size.x,
				               -sprite.bounds.size.y);

			Rect srcRect = new Rect (sprite.rect.x / sprite.texture.width,
				               sprite.rect.y / sprite.texture.height,
				               sprite.rect.width / sprite.texture.width,
				               sprite.rect.height / sprite.texture.height);


			GUI.DrawTexture (dstRect, sprite.texture);

			Graphics.DrawTexture (dstRect,
				sprite.texture,
				srcRect,
				0, 0, 0, 0);
			//GUI.DrawTexture (dstRect, sprite.texture);

		}

		void CalculateAndPlaceObjects (ObjectPlacerSet placerset, int row = 0, Transform parent = null)
		{
			if (!this.isActiveAndEnabled)
				return;

			if (placerset == null)
				return;

			if (placerSets.Count () == 0)
				return;


			float x = (colliderRadious + placerset.marginRadious / 2) * Mathf.Cos (Mathf.Deg2Rad * 0);
			float y = (colliderRadious + placerset.marginRadious / 2) * Mathf.Sin (Mathf.Deg2Rad * 0);
			x += myCenter.x;
			if (placerset.oriantation == PathOriantation.Y_Axes) {
				y += myCenter.y;
			} else {
				y += myCenter.z;
			}
			float spaceBetween = 360.0f / placerset.numberOfObject;

			if (autoObjectSize)
				ObjectObjectSize = (spaceBetween * ((placerset.marginRadious + colliderRadious) / 3)) * Mathf.Deg2Rad;
			Sprite sp = null;
			if (ObjectObject != null) {
				var spr = ObjectObject.pooledObject.GetComponentInChildren<SpriteRenderer> ();
				if (spr != null) {
					sp = spr.sprite;
					if (autoObjectSize)
						ObjectObjectSize = Mathf.Max (sp.bounds.extents.x, sp.bounds.extents.y);
				}
			}

			for (int i = 0; i < placerset.numberOfObject; i++) {

				float angle = (placerset.startingAngle + (i * spaceBetween + i * placerset.spaceing));
				if (placerset.pathType == ObjectPathType.line) {
					angle = (placerset.startingAngle + (i * spaceBetween));
					if (angle >= 0 && angle < 90)
						angle = 0;
					if (angle >= 90 && angle < 180)
						angle = 90;
					if (angle >= 180 && angle < 270)
						angle = 180;
					if (angle >= 270 && angle < 360)
						angle = 270;
				}

				if (angle >= 0 && angle <= 180)
					angle += placerset.zeroTo180Padding;
				if (angle >= 180 && angle <= 360)
					angle += placerset.oneeightyTo360Padding;
				if (angle >= 90 && angle <= 270)
					angle += placerset.ninetyTo270Padding;
				if ((angle >= 270 && angle <= 360) || (angle >= 0 && angle <= 90))
					angle += placerset.twoseventyTo90Padding;

				float angleInDeg = angle;
				angle *= Mathf.Deg2Rad;

				float ObjectObjectSizeDeg = (ObjectObjectSize * Mathf.Rad2Deg) + placerset.fineTuneRemovalP;

				if (placerset.clampAfter360 && angleInDeg > 360)
					continue;
				if (placerset.clampAfter180 && angleInDeg > 180)
					continue;
				if (placerset.clampAfter90 && angleInDeg > 90)
					continue;
				if (placerset.clampAfter270 && angleInDeg > 270)
					continue;
				if (placerset.clampBefore0 && angleInDeg < 0)
					continue;
				if (placerset.endingAngle > 0 && angleInDeg > placerset.endingAngle)
					continue;

				if (angleInDeg > 360)
					angleInDeg = angleInDeg % 360;
				if (angleInDeg < 0)
					angleInDeg = 360 - (Mathf.Abs (angleInDeg) % 360);
				if (placerset.removeEastWest && ((angleInDeg >= 0.0f - ObjectObjectSizeDeg - placerset.fineTuneRemovalN && angleInDeg <= 0.0f + ObjectObjectSizeDeg + placerset.fineTuneRemovalP) ||
				    (angleInDeg >= 180.0f - ObjectObjectSizeDeg - placerset.fineTuneRemovalN && angleInDeg <= 180.0f + ObjectObjectSizeDeg + placerset.fineTuneRemovalP)
				    || (angleInDeg >= 360.0f - ObjectObjectSizeDeg - placerset.fineTuneRemovalN && angleInDeg <= 360.0f + ObjectObjectSizeDeg + placerset.fineTuneRemovalP)))
					continue;
				if (placerset.removeNorthSouth && ((angleInDeg >= 90.0f - ObjectObjectSizeDeg - placerset.fineTuneRemovalN && angleInDeg <= 90.0f + ObjectObjectSizeDeg + placerset.fineTuneRemovalP) ||
				    (angleInDeg >= 270.0f - ObjectObjectSizeDeg - placerset.fineTuneRemovalN && angleInDeg <= 270.0f + ObjectObjectSizeDeg + placerset.fineTuneRemovalP)))
					continue;

				if (placerset.removeTopSide && (angleInDeg >= 45.0f - ObjectObjectSizeDeg - placerset.fineTuneRemovalN && angleInDeg <= 135.0f + ObjectObjectSizeDeg + placerset.fineTuneRemovalP))
					continue;
				if (placerset.removeBottomSide && (angleInDeg >= 180 + 45.0f - ObjectObjectSizeDeg - placerset.fineTuneRemovalN && angleInDeg <= 270 + 45.0f + ObjectObjectSizeDeg + placerset.fineTuneRemovalP))
					continue;
				if (placerset.removeLeftSide && (angleInDeg >= 135.0f - ObjectObjectSizeDeg - placerset.fineTuneRemovalN && angleInDeg <= 225.0f + ObjectObjectSizeDeg + placerset.fineTuneRemovalP))
					continue;
				if (placerset.removeRightSide && (angleInDeg >= 270 + 45.0f - ObjectObjectSizeDeg - placerset.fineTuneRemovalN && angleInDeg <= 360.0f + ObjectObjectSizeDeg + placerset.fineTuneRemovalP))
					continue;
				if (placerset.removeRightSide && (angleInDeg >= 0.0f - ObjectObjectSizeDeg - placerset.fineTuneRemovalN && angleInDeg <= 45.0f + ObjectObjectSizeDeg + placerset.fineTuneRemovalP))
					continue;

				if (placerset.pathType == ObjectPathType.Circle) {
					x = ((colliderRadious + placerset.marginRadious / 2) - placerset.bendingXOffset) * Mathf.Cos (angle);
					y = ((colliderRadious + placerset.marginRadious / 2) - placerset.bendingYOffset) * Mathf.Sin (angle);
				} else if (placerset.pathType == ObjectPathType.Box) {
					x = ((colliderRadious + placerset.marginRadious / 2) + ((colliderRadious + placerset.marginRadious / 2) / 100 * 40f) - placerset.bendingXOffset) * Mathf.Cos (angle);
					y = ((colliderRadious + placerset.marginRadious / 2) + ((colliderRadious + placerset.marginRadious / 2) / 100 * 40f) - placerset.bendingYOffset) * Mathf.Sin (angle);
					x = Mathf.Clamp (x, -(colliderRadious + placerset.marginRadious / 2), (colliderRadious + placerset.marginRadious / 2));
					y = Mathf.Clamp (y, -(colliderRadious + placerset.marginRadious / 2), (colliderRadious + placerset.marginRadious / 2));
				} else if (placerset.pathType == ObjectPathType.line) {
					float spaceInLine = (i % (placerset.numberOfObject / 4)) * (placerset.spaceing + (ObjectObjectSize * 2));
					x = ((spaceInLine + colliderRadious + placerset.marginRadious / 2) - placerset.bendingXOffset) * Mathf.Cos (angle);
					y = ((spaceInLine + colliderRadious + placerset.marginRadious / 2) - placerset.bendingYOffset) * Mathf.Sin (angle);
				}

				x += myCenter.x;
				if (placerset.oriantation == PathOriantation.Y_Axes) {
					y += myCenter.y;
				} else {
					y += myCenter.z;
				}

				if (ObjectObject != null) {
                    GameObject newObject;
                    // if there is a custom object pooler for the place set then use that instead of default one
                    if (placerset.CustomObjectPooler == null)
                        newObject = ObjectObject.GetPooledObject();
                    else 
                        newObject = placerset.CustomObjectPooler.GetPooledObject();
                    
					newObject.name = newObject.name.Replace ("(Clone)", "") + "-Row(" + row + ")-Cell(" + i + ")";
					//newObject.transform.position = new Vector3 (x, y, myCenter.z);
					if (placerset.oriantation == PathOriantation.Y_Axes) {
						newObject.transform.position = new Vector3 (x, y, myCenter.z) + placerset.localOffset;
					} else {
						newObject.transform.position = new Vector3 (x, myCenter.y, y) + placerset.localOffset;
					}

					newObject.SetActive (true);
					//		Camera2DPerspectiveMove perspectiveCamera = newObject.GetComponent<Camera2DPerspectiveMove> ();
					//		if (perspectiveCamera != null) {
					//		perspectiveCamera.ResetCameraLocation ();
					//}
					if (parent == null)
						parent = gameObject.transform;
					newObject.transform.SetParent (parent);
					//newObject.transform.SetParent (gameObject.transform);
					if (!attachedToParent)
						newObject.transform.SetParent (null);
				}
               
			}
		}
	}

	[System.Serializable]
	public class ObjectPlacerSet
	{
		public bool active = true;
        public ObjectPooler CustomObjectPooler;
		public float marginRadious = 0.5f;
		public ObjectPlacer.ObjectPathType pathType = ObjectPlacer.ObjectPathType.Circle;
		public ObjectPlacer.PathOriantation oriantation = ObjectPlacer.PathOriantation.Y_Axes;
		public Vector3 localOffset = new Vector3 ();
		public int numberOfObject = 5;
		public float startingAngle = 0;
		public float endingAngle = 0;
		public bool removeEastWest = false;
		public bool removeNorthSouth = false;
		public bool removeRightSide = false;
		public bool removeLeftSide = false;
		public bool removeTopSide = false;
		public bool removeBottomSide = false;
		public float spaceing = 0.0f;
		public float fineTuneRemovalP = 0.0f;
		public float fineTuneRemovalN = 0.0f;
		public float zeroTo180Padding = 0.0f;
		public float oneeightyTo360Padding = 0.0f;
		public float ninetyTo270Padding = 0.0f;
		public float twoseventyTo90Padding = 0.0f;
		public float bendingXOffset = 0.0f;
		public float bendingYOffset = 0.0f;
		public bool clampBefore0 = true;
		public bool clampAfter90 = false;
		public bool clampAfter180 = false;
		public bool clampAfter270 = false;
		public bool clampAfter360 = true;
	}
}