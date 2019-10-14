using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace BF.Utility
{
		public class Utilities
		{
	
				public  bool	AlmostTheSameVector3 (Vector3 a, Vector3 b, float offset)
				{
						if (Mathf.Abs (Vector3.Distance (a, b)) <= Mathf.Abs (offset))
								return true;
						//if(a.x-offset > b.x && a.x + offset < b.x && a.y-offset<b.y && a.y + offset > b.y && a.z -offset <b.z && a.z + offset > b.z)
						//	return true;
						return false;
				}

				public  bool	AlmostTheSameVector2 (Vector2 a, Vector2 b, float offset)
				{
						//if(a.x-offset > b.x && a.x + offset < b.x && a.y-offset<b.y && a.y + offset > b.y)
						//		return true;
						if (Mathf.Abs (Vector2.Distance (a, b)) <= Mathf.Abs (offset))
								return true;
						return false;
				}


		}

		public static class BFExtensions
		{
		
				public static bool	AlmostTheSameVector3 (this Vector3 a, Vector3 b, float offset)
				{
						if (Mathf.Abs (Vector3.Distance (a, b)) <= Mathf.Abs (offset))
								return true;
						return false;
				}

				public static bool	AlmostTheSameVector2 (this Vector2 a, Vector2 b, float offset)
				{
						if (Mathf.Abs (Vector2.Distance (a, b)) <= Mathf.Abs (offset))
								return true;

						return false;
				}

				public static bool AlmostTheSameAngle (this Quaternion rotA, Quaternion rotB, float offset =0)
				{
						float angle = Quaternion.Angle (rotA, rotB);
						if (angle < offset && angle > 0 - offset)
								return true;
						return false;
				}

				public static bool AlmostTheSame (this Transform t, Vector3 pos, Quaternion rot, Vector3 scale, float offset, bool ignorePos = false, bool ignoreRot = false, bool ignoreScale = false)
				{
						bool result = true;
						if (!ignorePos)
								result = pos.AlmostTheSameVector3 (t.position, offset);
						if (!ignoreRot)
								result = result && rot.AlmostTheSameAngle (t.rotation, offset);
						if (!ignoreScale)
								result = result && scale.AlmostTheSameVector3 (t.localScale, offset);
						return result;
				}

        		/*public static T GetAttribute2<T>(Enum enumValue) where T : Attribute
        		{
        			T attribute;

        			MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString())
        											.FirstOrDefault();

        			if (memberInfo != null)
        			{
        				attribute = (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        				return attribute;
        			}
        			return null;
        		}*/

        		public static TAttribute GetAttribute<TAttribute>(this Enum value)
        		where TAttribute : Attribute
        		{
        			var enumType = value.GetType();
        			var name = Enum.GetName(enumType, value);
        			return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        		}

		}

	
		[System.Serializable]
		public class Tuple<t1,t2>
		{
				public t1 item1;
				public t2 item2;

				private static readonly IEqualityComparer<t1> Item1Comparer = EqualityComparer<t1>.Default;
				private static readonly IEqualityComparer<t2> Item2Comparer = EqualityComparer<t2>.Default;

				public Tuple (t1 item1, t2 item2)
				{
						this.item1 = item1;
						this.item2 = item2;
				}
				public override string ToString ()
				{
						return string.Format ("<{0}, {1}>", item1, item2);
				}
		
				public static bool operator == (Tuple<t1, t2> a, Tuple<t1, t2> b)
				{
						if (Tuple<t1, t2>.IsNull (a) && !Tuple<t1, t2>.IsNull (b))
								return false;
			
						if (!Tuple<t1, t2>.IsNull (a) && Tuple<t1, t2>.IsNull (b))
								return false;
			
						if (Tuple<t1, t2>.IsNull (a) && Tuple<t1, t2>.IsNull (b))
								return true;
			
						return
				a.item1.Equals (b.item1) &&
								a.item2.Equals (b.item2);
				}
		
				public static bool operator != (Tuple<t1, t2> a, Tuple<t1, t2> b)
				{
						return !(a == b);
				}
		
				public override int GetHashCode ()
				{
						int hash = 17;
						hash = hash * 23 + item1.GetHashCode ();
						hash = hash * 23 + item2.GetHashCode ();
						return hash;
				}
		
				public override bool Equals (object obj)
				{

						var other = obj as Tuple<t1, t2>;
						if (object.ReferenceEquals (other, null))
								return false;
						else
								return Item1Comparer.Equals (item1, other.item1) &&
										Item2Comparer.Equals (item2, other.item2);
				}
		
				private static bool IsNull (object obj)
				{
						return object.ReferenceEquals (obj, null);
				}
		}
}
