using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using BF.Treetailor;

public class ProductItem : MonoBehaviour
{
	public Image image;
	public Text text;
	public ProductInfo product = null;
	public ItemManager itemManager = null;
    public Material enableMat = null;
    public Material disableMat = null;
	// Use this for initialization
	void Start ()
	{
		
		if (itemManager == null)
			itemManager = gameObject.GetComponent<ItemManager> ();
		if (product != null) {
			//text.text = product.productName + " (0/" + product.productCount + ")";
			text.text = product.DisplayName;
			//yield return product.LoadImage ();
			image.sprite = product.sprite;
			itemManager.dragableObjPrefab = product.objToUse;
			//itemManager.maxItemCount = product.productCount;
			itemManager.productId = product.productID;
            itemManager.productType = product.productType;

            if(disableMat == null)
                disableMat = Resources.Load<Material>(@".\Matterials\uiColorDisabled.mat") as Material;
            if(enableMat == null)
                enableMat = Resources.Load<Material>(@".\Matterials\uiColorDimmed.mat") as Material;

            if (product.productCount <= 0)
                image.material = disableMat;
        }
	}


    // Update is called once per frame
    void Update()
    {
        if (product.productCount <= 0 || product.productUsed == product.productCount)
        {
            if (Enabled == true || image.material != disableMat)
                this.Enabled = false;
        }
        else
        {
            if (Enabled == false)
                this.Enabled = true;
        }

    }
    private bool enabled = true;
    public bool Enabled
    {
        get { return enabled; }
        set { enabled = value;
            if (value == true)
            {
                if(enableMat != null)
                {
                    image.material = enableMat;
                    if (image.transform.parent != null && image.transform.parent.GetComponent<Image>() != null)
                        image.transform.parent.GetComponent<Image>().material = enableMat;
                }
            }
            else
            {
                if(disableMat != null)
                {
                    image.material = disableMat;
                    if(image.transform.parent != null && image.transform.parent.GetComponent<Image>() != null)
                    image.transform.parent.GetComponent<Image>().material = disableMat;
                }
            }
        }

    }
}
