using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour {

    public MeshRenderer TargetObject;
    public string MaterialPath;
    public Material matToSwitch;
	// Use this for initialization
	void Start () {
        matToSwitch = Resources.Load<Material>(MaterialPath);
        TargetObject.material = matToSwitch;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
