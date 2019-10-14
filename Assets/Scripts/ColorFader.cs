using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;

namespace BF.Utility
{
    public class ColorFader : MonoBehaviour
    {
        public enum fadeingMethod
        {
            FadeIn,
            FadeOut,
            PingPong,
        }
        public bool GetAllChilds = false;
        public GameObject DeactiveObjAfterFadeOut = null;
        [SerializeField]
        List<Component>
                components;
        public bool startTheFade = false;
        [SerializeField]
        public Color
                FadeToColor = Color.clear;
        public float speed = 2.0f;
        [SerializeField]
        public fadeingMethod
                FadingMethod = fadeingMethod.FadeOut;
        //List<Tuple<object,PropertyInfo>> listofProp = new List<Tuple<object, PropertyInfo>> ();
        //List<Tuple<object,FieldInfo>> listofField = new List<Tuple<object,FieldInfo>> ();
        List<colorsLib> colorComps = new List<colorsLib>();
        void Start()
        {
            foreach (var c in components)
            {
                foreach (var cc in c.GetType().GetProperties().Where(p => p.PropertyType.Equals(typeof(Color))))
                {
                    colorComps.Add(new colorsLib(c, cc, null, (Color)cc.GetValue(c, null)));

                }

                foreach (var cc in c.GetType().GetFields().Where(p => p.FieldType.Equals(typeof(Color))))
                {
                    colorComps.Add(new colorsLib(c, null, cc, (Color)cc.GetValue(c)));
                }
                if (GetAllChilds)
                {
                    foreach (var c2 in c.GetComponents(typeof(Component)))
                    {
                        foreach (var cc in c2.GetType().GetProperties().Where(p => p.PropertyType.Equals(typeof(Color))))
                        {
                            colorComps.Add(new colorsLib(c2, cc, null, (Color)cc.GetValue(c2, null)));

                        }

                        foreach (var cc in c2.GetType().GetFields().Where(p => p.FieldType.Equals(typeof(Color))))
                        {
                            colorComps.Add(new colorsLib(c2, null, cc, (Color)cc.GetValue(c2)));
                        }
                    }
                    foreach (var c2 in c.GetComponentsInChildren(typeof(Component)))
                    {
                        foreach (var cc in c2.GetType().GetProperties().Where(p => p.PropertyType.Equals(typeof(Color))))
                        {
                            colorComps.Add(new colorsLib(c2, cc, null, (Color)cc.GetValue(c2, null)));

                        }

                        foreach (var cc in c2.GetType().GetFields().Where(p => p.FieldType.Equals(typeof(Color))))
                        {
                            colorComps.Add(new colorsLib(c2, null, cc, (Color)cc.GetValue(c2)));
                        }
                    }
                }
            }
        }

        // Update is called once per frame
        float time = 1.0f;
        Color newColor;
        void Update()
        {

            if (startTheFade)
            {
                if (FadingMethod == fadeingMethod.FadeOut && time < 0)
                {
                    if(DeactiveObjAfterFadeOut != null)
                    {
                        DeactiveObjAfterFadeOut.SetActive(false);
                    }
                    startTheFade = false;
                    return;
                }
                if (FadingMethod == fadeingMethod.FadeIn && time > 1)
                {
                    startTheFade = false;
                    return;
                }
                if (FadingMethod == fadeingMethod.PingPong && time >= 2)
                {
                    time = 0;
                }
                if (FadingMethod == fadeingMethod.FadeOut)
                    time -= Time.deltaTime * speed;
                else if (FadingMethod == fadeingMethod.FadeIn)
                    time += Time.deltaTime * speed;
                else
                    time += Time.deltaTime * speed;

                foreach (colorsLib c in colorComps)
                {
                    newColor = Color32.Lerp(FadeToColor, c.defColor, Mathf.Clamp01(time));
                    if (FadingMethod == fadeingMethod.PingPong)
                        newColor = Color32.Lerp(FadeToColor, c.defColor, Mathf.PingPong(time, 1));
                    if (c.prop != null)
                    {
                        c.prop.SetValue(c.comp, newColor, null);
                    }
                    else if (c.field != null)
                    {

                        c.field.SetValue(c.comp, newColor);
                        //c.comp.SendMessage ("Refresh");
                    }
                }
            }

        }
        public void pause()
        {
            startTheFade = false;
        }
        public void play()
        {
            startTheFade = true;
        }
        public void FadeOut()
        {
            time = 1;
            FadingMethod = fadeingMethod.FadeOut;
            startTheFade = true;
        }
        public void FadeIn()
        {
            if (DeactiveObjAfterFadeOut != null)
            {
                DeactiveObjAfterFadeOut.SetActive(true);
            }
            time = 0;
            FadingMethod = fadeingMethod.FadeIn;
            startTheFade = true;
        }
        public void PingPong()
        {
            time = 0;
            FadingMethod = fadeingMethod.PingPong;
            startTheFade = true;
        }
        public bool IsFadeOut()
        {
            return time <= 0 && !startTheFade;
        }
        public bool IsFadeIn()
        {
            return time >= 1 && !startTheFade;
        }
    }

    public class colorsLib
    {
        public Component comp;
        public PropertyInfo prop;
        public FieldInfo field;
        public Color defColor;
        public Color fadeingColor;
        public colorsLib(Component comp, PropertyInfo prop, FieldInfo field, Color defColor)
        {
            this.comp = comp;
            this.prop = prop;
            this.field = field;
            this.defColor = defColor;
        }
    }
}