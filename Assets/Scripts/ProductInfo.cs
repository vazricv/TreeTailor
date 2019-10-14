using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace BF.Treetailor
{


    [System.Serializable]
    public class ProductInfo //: MonoBehaviour
    {
        public string Error = null;

        public enum ProductType
        {
            [ProductCategoryAttribute("unknown", -1, true)]
            Unknown = 0,
            [ProductCategoryAttribute("Ornaments", 101)]
            Ornament,
            [ProductCategoryAttribute("Tree-Toppers", 102)]
            TreeTopper,
            [ProductCategoryAttribute("Twinkle Lights", 103)]
            TwinkleLights,
            [ProductCategoryAttribute("Ribbons", 104)]
            Ribbons,
            [ProductCategoryAttribute("Trees", 105, true)]
            Tree,
            [ProductCategoryAttribute("Other", 106, true)]
            Other
        }

        public Sprite sprite = null;
        [SerializeField]
        public int productID = 0;
        public string productName = "";
        public string description = "";
        public string url = "";
        public ProductType productType = ProductType.Unknown;
        public string imageURL = "";
        public bool use3DObject = false;
        public string objectURL = "";
        public GameObject prefabObject = null;
        public GameObject objToUse = null;
        public int productCount = 0;

        // this is only for internal use
        // number of products draged and used in the scene
        public int productUsed = 0;

        public string DisplayName
        {
            get
            {
                return this.productName + " (" + this.productUsed.ToString() + "/" + productCount.ToString() + ")";
            }
        }

        public int ProductLeft
        {
            get { return this.productCount - this.productUsed; }
        }

        public ProductInfo()
        {
            if (!string.IsNullOrEmpty(imageURL) && sprite == null)
            {
                LoadImage();
            }
        }

        public ProductInfo(int id, string pName, ProductType pType, string pImage, GameObject item, bool p3D, string p3DModelURL, int pCount, string pURL, string pDescription)
        {
            this.productID = id;
            this.productName = pName;
            this.description = pDescription;
            this.imageURL = pImage;
            this.objectURL = p3DModelURL;
            this.prefabObject = item;
            this.productType = pType;
            this.url = pURL;
            this.use3DObject = p3D;
            this.productCount = pCount;
            LoadImage();

        }

        public IEnumerator LoadImage()
        {

            Dictionary<string, string> headers = new Dictionary<string, string>();

#if UNITY_WEBGL
            headers.Add("Access-Control-Allow-Credentials", "true");
            headers.Add("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
            headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            headers.Add("Access-Control-Allow-Origin", "*");
#endif


            Texture2D tex = new Texture2D(1, 1, TextureFormat.DXT1, false);
            WWW www = new WWW(imageURL, null, headers);
            yield return www;
            try
            {
                www.LoadImageIntoTexture(tex);

            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }

            sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            Vector3 scale = new Vector3(128.0f / tex.width, 128.0f / tex.height / 1, 1f);

            objToUse = GameObject.Instantiate(prefabObject);
            objToUse.name = "ObjID-" + productID;
            objToUse.transform.localScale = scale;
            objToUse.GetComponent<BoxCollider>().size = new Vector3(1 / scale.x, 1 / scale.y, 1);
            objToUse.GetComponent<SpriteRenderer>().sprite = sprite;
            objToUse.GetComponent<ItemManager>().productId = this.productID;

            if (!string.IsNullOrEmpty(www.error))
                Error = "Error:" + www.error;
        }


    }

    public class ProductCategoryAttribute : System.Attribute
    {
        public string Description { get; set; }

        public bool isHidden { get; set; }

        public int productCategoryID { get; set; }

        public ProductCategoryAttribute(string description, int id = -1, bool hidden = false)
        {
            Description = description;
            isHidden = hidden;
            productCategoryID = id;
        }
    }
}