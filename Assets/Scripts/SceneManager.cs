using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CI.HttpClient;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using BF.Utility;
using System.IO;
using UnityEngine.Networking;
using BF.Treetailor;

public class SceneManager : MonoBehaviour
{
    public enum TreeTypes
    {
        [SourceURL("http://filedn.com/lN9qaElxsvQVFv0XvCyMc0R/TreeTailor/balsam", false)]
        blasam =1,
        canaanFir,
        concolorFir,
        dougals,
        fraserFir,
        grand,
        nobel,
        blasam_snow,
		canaanFir_snow,
		concolorFir_snow,
		dougals_snow,
		fraserFir_snow,
		grand_snow,
		nobel_snow,
    }
    public enum TreeSizes
    {
        [zPosSets(10, 8.0f)]
        f3 = 3,
        [zPosSets(12.5f, 10.0f)]
        f4 =4,
        [zPosSets(15, 12.5f)]
        f5 = 5,
        [zPosSets(18, 16.0f)]
        f6 = 6,
        [zPosSets(21, 15.0f)]
        f7 =7,
        [zPosSets(26.5f, 22.0f)]
        f9 =9,
        [zPosSets(35, 29.5f)]
        f11 = 11,
        [zPosSets(58, 34.5f)]
        f20 = 20
    }
    public Transform itemGrid;
    public GameObject itemPrefab;
    public GameObject buttonPrefab;
    public GameObject buttonPanel;
    public Image ProgressBar;
    public GameObject LoadingPanel;
    public GameObject ErrorPanel;
    public Text ErrorMessage;
    public GameObject theTree;
    public float DragItemszPos = 12.5f;
    public TreeTypes SelectedTreeType;
    public TreeSizes SelectedTreeSize;
    public GameObject Under9ftLightset;
    public GameObject Night9And11ftLightset;
    public GameObject Twenty20ftLightset;
    public ProductInfo[] products;
    //public HttpClient httpClient = new HttpClient();

    private List<GameObject> categoryButtons = new List<GameObject>();

    //[DllImport("__Internal")]
    //private static extern void Hello();

    void Start()
    {
        Application.runInBackground = true;
        theTree = GameObject.Find("TheTree");
        //loadAssets();
    }
    public void loadAssets()
    {
        StartCoroutine(LoadingAllAssetsAsync());
    }
    // Use this for initialization
    IEnumerator LoadingAllAssetsAsync()
    {
        LoadingPanel.SetActive(true);
        ColorFader cf = LoadingPanel.GetComponent<ColorFader>();
        if (cf != null)
        {
            // cf.FadeIn();
        }

        ProgressBar.fillAmount = 0;
        int loadingSteps = 4;   //1 for loading the settings coockies, 2 for loading the shoping cart and list of products ,3 loading the tree, 4 for generating the tabs

        System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
        System.Security.Cryptography.DESCryptoServiceProvider b = new System.Security.Cryptography.DESCryptoServiceProvider();


        loadingSteps = 30; //to make it look good while loading the coockies and products 
        ProgressBar.fillAmount = 1.0f * 1.0f / loadingSteps;
        if (PlayerPrefs.HasKey("user"))
        {
            string username = PlayerPrefs.GetString("user");
            Debug.Log(username);
        }
        else {
            PlayerPrefs.SetString("user", "Vazric");
        }

        // loading products ...
        ProgressBar.fillAmount = 2.0f * 1.0f / loadingSteps;

        //in this point we loaded the product
        loadingSteps = products.Length + 4;
        //loading the tree

        //get the size and set the position using size attribiyte of zPosSets
        DragItemszPos = SelectedTreeSize.GetAttribute<zPosSets>().DragItemZPos;
        int treesize = (int)SelectedTreeSize;

        // enabling the lights for 3 to 7ft , then 9 and 11fit , then 20ft
        Under9ftLightset.SetActive((treesize < 9));
        Night9And11ftLightset.SetActive((treesize > 7 && treesize < 20));
        Twenty20ftLightset.SetActive(treesize > 11);

        theTree.transform.position = new Vector3(theTree.transform.position.x,theTree.transform.position.y, SelectedTreeSize.GetAttribute<zPosSets>().TreeZPos);
        theTree.SetActive(false);

        //loading the tree prefab 
        string treePrefabSRC = SelectedTreeType.GetAttribute<SourceURL>().GetURLForSize(SelectedTreeSize);

        Debug.Log(treePrefabSRC);

        var www = UnityWebRequestAssetBundle.GetAssetBundle(treePrefabSRC);
        yield return www.Send();
        if (www.isNetworkError)
        {
            if (!string.IsNullOrEmpty(www.error))
            {
                ErrorPanel.SetActive(true);
                ErrorMessage.text = www.error + ". Please check your connection and try to reload the page again.";
            }
            Debug.LogError(www.error);
            yield break;
        }
        Debug.Log(www.downloadHandler.ToString());
        var bundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
        www.Dispose();
        var assetNames = bundle.GetAllAssetNames();
        foreach(var c in assetNames)
        {
            Debug.Log(c);
        }
       // var objects = ;
      //  GameObject test = (GameObject)bundle.LoadAsset("balsam_3ft.prefab");



        GameObject myTree = Instantiate(bundle.mainAsset as GameObject);
        myTree.transform.SetParent(theTree.transform);
        myTree.transform.position = Vector3.zero;
        myTree.transform.localPosition = Vector3.zero;
        myTree.transform.localRotation = Quaternion.Euler((Vector3.zero));
        myTree.transform.rotation = Quaternion.Euler(Vector3.zero);
       // myTree.GetComponent<MeshRenderer>().materials[0] = null;
        //myTree.GetComponent<MeshRenderer>().materials[0] = Resources.Load("Materials/balsamTree01") as Material;
        /*
        Debug.Log("loading AssetBundle!");
        //var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "myassetBundle"));
        WWW www = WWW.LoadFromCacheOrDownload(treePrefabSRC, 1);
        yield return www;
        AssetBundle bundle = www.assetBundle;

      //  var t = bundle.LoadAllAssets();

        GameObject test = (GameObject)bundle.LoadAsset("balsam_5ft");

        Instantiate(test);
        test.transform.SetParent(theTree.transform);
        test.transform.position = Vector3.zero;
       */
        //LoadTheTree(treePrefabSRC);
        ProgressBar.fillAmount = 2.0f * 1.0f / loadingSteps;

        for (int index = 0; index < products.Length; index++)
        {
            Debug.Log("Loading " + index.ToString());
            yield return products[index].LoadImage();
            if (!string.IsNullOrEmpty(products[index].Error))
            {
                ErrorPanel.SetActive(true);
                ErrorMessage.text = products[index].Error + ". Please check your connection and try to reload the page again.";
            }
            ProgressBar.fillAmount = (3.0f + index) * 1.0f / loadingSteps;
        }

        var type = typeof(ProductInfo.ProductType);

        var values = System.Enum.GetValues(typeof(ProductInfo.ProductType)).Cast<ProductInfo.ProductType>().Select(e => new
        {
            pType = e,
            pAttr = (ProductCategoryAttribute)(type.GetMember(e.ToString()).First().GetCustomAttributes(typeof(ProductCategoryAttribute), false)[0]),
            numOfProd = products.Count(p => p.productType == e)
        }).ToList();

        categoryButtons.Clear();
        ProgressBar.fillAmount = 1;
        foreach (var cat in values)
        {
            if (!cat.pAttr.isHidden)
            {
                GameObject button = (GameObject)Instantiate(buttonPrefab);
                button.name = "Cat_" + cat.pType.ToString();
                button.transform.SetParent(buttonPanel.transform, false);//Setting button parent
                button.transform.GetChild(0).GetComponent<Text>().text = cat.pAttr.Description + " (" + cat.numOfProd.ToString() + ")";//Changing text
                if (cat.numOfProd > 0)
                {
                    categoryButtons.Add(button);
                    button.GetComponent<Button>().onClick.AddListener(new UnityAction(delegate
                    {
                        ListItemsByType(cat.pType);
                    }));
                }
                else {
                    button.GetComponent<Button>().interactable = false;
                }
            }
        }

        Debug.Log("Loading complete!");

        if (cf != null)
        {
            cf.FadeOut();
        }
        else
        {
            LoadingPanel.SetActive(false);
        }
        theTree.SetActive(true);
    }

    // Update is called once per frame
    //void Update ()
    //{

    //}

    public void ListItemsByType(int pType)
    {
        foreach(GameObject b in categoryButtons)
        {
            if(b.name == "Cat_"+ ((ProductInfo.ProductType)pType).ToString())
            {
                b.GetComponent<Image>().color = new Color(0.5f, 0.4f, 0.8f, 0.2f);
                b.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
            }
            else
            {
                b.GetComponent<Image>().color = new Color(1, 1, 1);
                b.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
            }

        }
        foreach (Transform t in itemGrid.transform)
            Destroy(t.gameObject);
        foreach (var p in products.ToList().Where(p => p.productType == (ProductInfo.ProductType)pType || pType == 0))
        {
            p.LoadImage();
            var newitem = Instantiate(itemPrefab);
            newitem.transform.SetParent(itemGrid, false);
            newitem.GetComponentInChildren<ProductItem>().product = p;
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ListItemsByType(ProductInfo.ProductType pType)
    {
        ListItemsByType((int)pType);
    }

    public int GetProductMaxCount(int productID)
    {
        return products.FirstOrDefault(p => p.productID == productID).productCount;
    }

    public int GetProductUsedCount(int productID)
    {
        return products.FirstOrDefault(p => p.productID == productID).productUsed;
    }

    public int GetProductLeft(int productID)
    {
        return products.FirstOrDefault(p => p.productID == productID).ProductLeft;
    }

    public string GetProductDisplayName(int productID)
    {
        return products.FirstOrDefault(p => p.productID == productID).DisplayName;
    }

    public ProductInfo GetProduct(int productID)
    {
        return products.FirstOrDefault(p => p.productID == productID);
    }

    public bool UseOne(int productID)
    {
        ProductInfo product = products.FirstOrDefault(p => p.productID == productID);
        if (product.productUsed < product.productCount)
        {
            product.productUsed++;
            return true;
        }
        return false;
    }

    public bool BackToBasketOne(int productID)
    {
        ProductInfo product = products.FirstOrDefault(p => p.productID == productID);
        if (product.productUsed > 0)
        {
            product.productUsed--;
            return true;
        }
        return false;
    }

    public void ObjectToggler(GameObject target)
    {
        target.SetActive(!target.activeSelf);
    }

    public IEnumerable LoadTheTree( string filePath)
    {
        Debug.Log("loading AssetBundle!");
        //var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "myassetBundle"));
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.Send();
        string result = www.downloadHandler.text;
        Debug.Log("assets file: "+result);
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(result);
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            //return;
        }

        var prefab = myLoadedAssetBundle.LoadAllAssets();
        Instantiate(prefab[0]);

        myLoadedAssetBundle.Unload(false);
    }

    public void SetTreeSize(int size)
    {
        SelectedTreeSize = (TreeSizes)size;
    }
    public void SetTreeType(int treetypeInt)
    {
        SelectedTreeType = (TreeTypes)treetypeInt;
        loadAssets();
    }
    /*
    IEnumerator DownloadAssetBundle()
    {
        yield return StartCoroutine(this.downloadAssetBundle(@"https://filedn.com/lN9qaElxsvQVFv0XvCyMc0R/TreeTailor/balsam_5ft.unitypackage", 0));

        bundle = this.getAssetBundle(@"https://filedn.com/lN9qaElxsvQVFv0XvCyMc0R/TreeTailor/balsam_5ft.unitypackage", 0);

        GameObject obj = Instantiate(bundle.LoadAsset("balsam_5ft"), Vector3.zero, Quaternion.identity) as GameObject;
        // Unload the AssetBundles compressed contents to conserve memory
        bundle.Unload(false);
    }
     */
}

public class SourceURL : System.Attribute
{
	public string URL { get; set; }
    public bool IsPrefab { get; set; }

    public SourceURL(string sourceURL, bool isPrefab)
    {
        URL = sourceURL;
        IsPrefab = isPrefab;
    }

	public string GetURLForSize(SceneManager.TreeSizes size)
	{
        if(IsPrefab)
            return URL + "_" + ((int)size).ToString() + "ft";
        else
            return URL + "_" + ((int)size).ToString()+"ft.unity3d";
	}
}


public class zPosSets : System.Attribute
{
    public float TreeZPos { get; set; }
    public float DragItemZPos { get; set; }

    public zPosSets( float treeZPos, float dragingZPos)
    {
        this.TreeZPos = treeZPos;
        this.DragItemZPos = dragingZPos;
    }
}


