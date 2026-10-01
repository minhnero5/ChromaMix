using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AssetBundleManager : MonoBehaviour
{
    private static AssetBundleManager _instance;
    public static AssetBundleManager Instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = FindObjectOfType<AssetBundleManager>();
            }

            if (_instance == null)
            {
                GameObject singleton = new GameObject();
                _instance = singleton.AddComponent<AssetBundleManager>();
                DontDestroyOnLoad(singleton);
            }
            return _instance;
        }
    }

    const string KEYBASEDATAONSERVER = "AssetBundles_New";
    public bool useLocalAssets = false;

    public AdsManager ads;
    [System.Serializable]
    public class AssetBundles
    {
        public AdmobID admob;
        public List<AssetBundleInfo> listAssetBundle = new List<AssetBundleInfo>();
    }
    [System.Serializable]
    public class AssetBundleInfo
    {
        public string name;
        public string linkToAssetBundleOnServer;
        public int version;
        public float fileSize;
        public string type;
        public string platform = "Android";
    }

    public AssetBundles assetBundles = new AssetBundles();
    public List<AssetBundleInfo> listAssetBundleNeedDownload = new List<AssetBundleInfo>();
    public List<AssetBundleInfo> listAssetBundleNeedLoad = new List<AssetBundleInfo>();

    public AssetBundleInfo downloadingAssetBundle = new AssetBundleInfo();
    public AssetBundleInfo loadingAssetBundle = new AssetBundleInfo();

    public float currentDownloadingProgress = 0;
    public int currentIndexDownloading = 0;
    public int currentIndexLoading = 0;

    bool isDownloadingAssetBundles = false;
    System.Action onComplete;
    System.Action onFail;

    public Dictionary<string, AssetBundle> listLoadedAssetBundle = new Dictionary<string, AssetBundle>();


    public float totalSizeNeedToDownload = 0f;
    public void GetBaseDataAndCheckMAster(string freLoad, System.Action onComplete = null, System.Action onFail = null)
    {
        //Debug.Log(freLoad);
        assetBundles = JsonConvert.DeserializeObject<AssetBundles>(freLoad);
        ads.admobID.appId = assetBundles.admob.appId;
        ads.admobID.banner = assetBundles.admob.banner;
        ads.admobID.interstitial = assetBundles.admob.interstitial;
        ads.admobID.rewarded = assetBundles.admob.rewarded;
        CheckAndDownloadAssetBundles(onComplete, onFail);
       
        //if (onComplete != null)
        //{
        //    onComplete();
        //}
    }
    public void CheckAndDownloadAssetBundles(System.Action onComplete = null, System.Action onFail = null)
    {
        if (!useLocalAssets)
        {
            LoadingController.Instance.downloadingAssetBar.SetActive(true);
            //LoadingController.Instance.loginButtons.SetActive(false);
            LoadingController.Instance.loadingBar.SetActive(false);
            LoadingController.Instance.downloadingAssetTitle.gameObject.SetActive(false);
            listLoadedAssetBundle.Clear();
            this.onComplete = onComplete;
            this.onFail = onFail;
            listAssetBundleNeedDownload.Clear();

            if (assetBundles.listAssetBundle.Count > 0)
            {
                foreach (AssetBundleInfo assetBundleInfo in assetBundles.listAssetBundle)
                {
                    string link = assetBundleInfo.linkToAssetBundleOnServer;
                    int version = assetBundleInfo.version;
                    
                    Hash128 hash = Hash128.Parse(assetBundleInfo.version.ToString());
                    string platform = assetBundleInfo.platform;
#if UNITY_ANDROID
        if (platform == "iOS")
        {
            continue;
        }
#elif UNITY_IOS
        if (platform == "Android")
        {
            continue;
        }
#endif
                    // if (!Caching.IsVersionCached(link, version))
                  
                    if (!Caching.IsVersionCached(link, hash))
                    {
                        totalSizeNeedToDownload += assetBundleInfo.fileSize;
                        listAssetBundleNeedDownload.Add(assetBundleInfo);
                    }
                    else
                    {
                        listAssetBundleNeedLoad.Add(assetBundleInfo);
                    }
                }
            }

#if UNITY_IOS
        if (listAssetBundleNeedDownload.Count > 0)
        {
            string content = I2.Loc.ScriptLocalization.Get("DownloadResourcesContent");
            content = content.Replace("<totalSize>", totalSizeNeedToDownload.ToString());
            DialogController.ShowConfirmAlertDialog(I2.Loc.ScriptLocalization.Get("DownloadResources"), content, true, true, () =>
            {
                if (listAssetBundleNeedLoad.Count > 0)
                {
                    currentIndexLoading = 0;
                    StartCoroutine(LoadAssetBundles());
                }
                else
                {
                    currentIndexDownloading = 0;
                    StartCoroutine(DownloadAssetBundles());
                }
            }, () =>
            {
                Application.Quit();
            });
        }
        else
        {
            if (listAssetBundleNeedLoad.Count > 0)
            {
                currentIndexLoading = 0;
                StartCoroutine(LoadAssetBundles());
            }
        }
#else
            if (listAssetBundleNeedLoad.Count > 0)
            {
                currentIndexLoading = 0;
                StartCoroutine(LoadAssetBundles());
            }
            else
            {
                currentIndexDownloading = 0;
                StartCoroutine(DownloadAssetBundles());
            }
#endif
        }
        else
        {
            LoadingController.Instance.downloadingAssetBar.SetActive(false);
            //LoadingController.Instance.loginButtons.SetActive(false);
            LoadingController.Instance.loadingBar.SetActive(true);
            LoadingController.Instance.downloadingAssetTitle.gameObject.SetActive(false);
            if(onComplete != null)
            {
                onComplete();
            }
        }


    }

    void Update()
    {
        if (isDownloadingAssetBundles)
        {
            LoadingController.Instance.downloadingProgress.text = currentDownloadingProgress * downloadingAssetBundle.fileSize + " kb/" + downloadingAssetBundle.fileSize + " kb";
            LoadingController.Instance.downloadingAssetForceground.fillAmount = currentDownloadingProgress;
        }
    }

    IEnumerator LoadAssetBundles()
    {
        loadingAssetBundle = listAssetBundleNeedLoad[currentIndexLoading];
        isDownloadingAssetBundles = false;
       LoadingController.Instance.downloadingProgress.text = "CheckingAssetBundles";
        // WWW www = WWW.LoadFromCacheOrDownload(loadingAssetBundle.linkToAssetBundleOnServer, loadingAssetBundle.version);
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(loadingAssetBundle.linkToAssetBundleOnServer, (uint)loadingAssetBundle.version,0);

        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Error");
        }
        else
        {
            //            Debug.Log("Done");
            if (loadingAssetBundle.type == "Scene")
            {
                try
                {
                    // var assetBundle = www.assetBundle;
                    var assetBundle = DownloadHandlerAssetBundle.GetContent(www);
                    string[] scenePaths = assetBundle.GetAllScenePaths();
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
                }
                catch
                {

                }
            }
            //listLoadedAssetBundle.Add(loadingAssetBundle.name, www.assetBundle);
            // listLoadedAssetBundle.Add(loadingAssetBundle.name, DownloadHandlerAssetBundle.GetContent(www));
            listLoadedAssetBundle.Add(loadingAssetBundle.name, DownloadHandlerAssetBundle.GetContent(www));
            www.Dispose();
            if (currentIndexLoading < listAssetBundleNeedLoad.Count - 1)
            {
                currentIndexLoading++;
                StartCoroutine(LoadAssetBundles());
            }
            else
            {
                if (listAssetBundleNeedDownload.Count > 0)
                {
                    currentIndexDownloading = 0;
                    StartCoroutine(DownloadAssetBundles());
                }
                else
                {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                }

            }
        }
    }


    IEnumerator DownloadAssetBundles()
    {
        downloadingAssetBundle = listAssetBundleNeedDownload[currentIndexDownloading];
        currentDownloadingProgress = 0;
        LoadingController.Instance.downloadingAssetTitle.gameObject.SetActive(true);
        LoadingController.Instance.downloadingAssetTitle.text = "Downloading...." + " (" + (currentIndexDownloading + 1) + "/" + listAssetBundleNeedDownload.Count + ")";
        isDownloadingAssetBundles = true;
        Debug.Log("Start Download Bundle: " + downloadingAssetBundle.linkToAssetBundleOnServer + " - " + downloadingAssetBundle.version);
        //WWW www = WWW.LoadFromCacheOrDownload(downloadingAssetBundle.linkToAssetBundleOnServer, downloadingAssetBundle.version);
     
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(downloadingAssetBundle.linkToAssetBundleOnServer, (uint)downloadingAssetBundle.version,0);
        //while (!www.isDone)
        //{
        //    currentDownloadingProgress = www.progress;
        //    yield return new WaitForSeconds(.1f);
        //}

        StartCoroutine(ShowProgress(www));

        yield return www.SendWebRequest();
        
        if (!string.IsNullOrEmpty(www.error) || www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            Debug.Log("Done");
            if (downloadingAssetBundle.type == "Scene")
            {
                try
                {
                    Debug.Log("Downloaded Scene: " + downloadingAssetBundle.name + "-" + downloadingAssetBundle.linkToAssetBundleOnServer + "-" + downloadingAssetBundle.version);
                    var assetBundle = DownloadHandlerAssetBundle.GetContent(www);
                    string[] scenePaths = assetBundle.GetAllScenePaths();
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
                }
                catch
                {
                    Debug.Log(www.error);
                    //DialogController.ShowConfirmAlertDialog(I2.Loc.ScriptLocalization.Get("AlertDialog/Connect fail"), www.error, true, false, () =>
                    //{
                    //    Master.Instance.ReLogin(false);
                    //});
                }
                yield return new WaitForSeconds(1);
            }
            else
            {
                //var bundle = DownloadHandlerAssetBundle.GetContent(www);
                //Debug.Log("다른 번들 "+bundle.name);
            }

  
            //Debug.Log("GetContent 이름 = " + DownloadHandlerAssetBundle.GetContent(www).name);

            listLoadedAssetBundle.Add(downloadingAssetBundle.name, DownloadHandlerAssetBundle.GetContent(www));
       
            
            www.Dispose();
            if (currentIndexDownloading < listAssetBundleNeedDownload.Count - 1)
            {
                currentIndexDownloading++;
                StartCoroutine(DownloadAssetBundles());
            }
            else
            {
                isDownloadingAssetBundles = false;
                //Debug.Log("OnComplete:" + )
                if (onComplete != null)
                {
                    onComplete();
                }
            }
        }
    }

    private IEnumerator ShowProgress(UnityWebRequest www)
    {
        Debug.LogError("ShowProgress www: " + www.url);
        while (!www.isDone)
        {
            currentDownloadingProgress = www.downloadProgress;
            yield return new WaitForSeconds(.1f);
        }
    }

    public Object GetAssetBundle(string assetRootName, string pathToAsset)
    {
        Debug.LogWarning(pathToAsset);
        if (!useLocalAssets)
        {
            Object asset = listLoadedAssetBundle[assetRootName].LoadAsset(pathToAsset);
#if UNITY_EDITOR
            if (asset is GameObject)
            {
                FixShaders(asset as GameObject);
            }
#endif
            return asset;
        }
        else
        {
            return Resources.Load(pathToAsset);
        }
    }
    
    public static void FixShaders(GameObject prefab)
        {
        var renderers = prefab.GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in renderers)
        {
            ReplaceShader(renderer.sharedMaterials);
        }

        var tmps = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var tmp in tmps)
        {
            ReplaceShader(tmp.material);
            ReplaceShader(tmp.materialForRendering);
        }

        var spritesRenderers = prefab.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var spriteRenderer in spritesRenderers)
        {
            ReplaceShader(spriteRenderer.sharedMaterials);
        }

        var images = prefab.GetComponentsInChildren<Image>(true);
        foreach (var image in images)
        {
            ReplaceShader(image.material);
        }

        var particleSystemRenderers = prefab.GetComponentsInChildren<ParticleSystemRenderer>(true);
        foreach (var particleSystemRenderer in particleSystemRenderers)
        {
            ReplaceShader(particleSystemRenderer.sharedMaterials);
        }

        var particles = prefab.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var particle in particles)
        {
            var renderer = particle.GetComponent<Renderer>();
            if (renderer != null) ReplaceShader(renderer.sharedMaterials);
        }

        //var uiParticleSystems = prefab.GetComponentsInChildren<UIParticleSystem>(true);
        //foreach (var uiParticleSystem in uiParticleSystems)
        //{
        //    var material = uiParticleSystem.material;
        //    if (material != null) ReplaceShader(material);
        //}
    }

    public static void ReplaceShader(Material[] materials)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            ReplaceShader(materials[i]);
        }
    }

    public static void ReplaceShader(Material material)
    {
        if (material == null) return;

        var shaderName = material.shader.name;
        var shader = Shader.Find(shaderName);

        if (shader != null) material.shader = shader;
    }
}
