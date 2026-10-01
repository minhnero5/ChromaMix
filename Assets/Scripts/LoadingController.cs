using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class LoadingController : Singleton2<LoadingController>
{

    // Use this for initialization
    public float loadingAmount = 0;
    public GameObject loadingBar;
    public Image loadingForceground;
    public GameObject downloadingAssetBar;
    public Image downloadingAssetForceground;
    public Text downloadingAssetTitle;
    public Text downloadingProgress;
    public const string LINK_MASTER_ASSETBUNDLE1 = "";
    public void Awake()
    {
        StartCoroutine(GetMasterJson());
    }
    IEnumerator GetMasterJson()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(LINK_MASTER_ASSETBUNDLE1))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                StartCoroutine(GetMasterJson());
            }
            else
            {
                // Show results as text
                string json = www.downloadHandler.text;
                AssetBundleManager.Instance.GetBaseDataAndCheckMAster(json, () =>
                {
                    //loadingBar.SetActive(true);
                    downloadingAssetBar.SetActive(false);
                    IncreaseProgress();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Splash");
                    AdsManager.Instance.LoadAdsWhenLoadID();

                });
            }
        }
        //WWW www = WWW.LoadFromCacheOrDownload(LINK_MASTER_ASSETBUNDLE, 0);
        //yield return www;
        //if (!string.IsNullOrEmpty(www.error))
        //{
        //    Debug.Log("Error: " + www.error);
        //    DialogController.ShowConfirmAlertDialog(I2.Loc.ScriptLocalization.Get("AlertDialog/Connect fail"), www.error, true, false, () =>
        //    {
        //        StartCoroutine(DownloadAssetBundlesMaster());
        //    });
        //}
        //else
        //{
        //    AssetBundle assetBundle = www.assetBundle;
        //    TextAsset textAsset = (TextAsset)assetBundle.LoadAsset("Master_JSON");
        //    AssetBundleManager.Instance.GetBaseDataAndCheckMAster(textAsset.text, () =>
        //    {
        //        loadingBar.SetActive(true);
        //        downloadingAssetBar.SetActive(false);
        //        IncreaseProgress();
        //        LoadData();

        //    });
        //}
    }
    void Start()
    {
        loadingForceground.fillAmount = 0;
        loadingBar.SetActive(false);
        downloadingAssetBar.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (loadingForceground.fillAmount < loadingAmount)
        {
            loadingForceground.fillAmount += Time.deltaTime * 0.6f;
        }
        if (loadingForceground.fillAmount > loadingAmount)
        {
            loadingForceground.fillAmount = loadingAmount;
        }
    }

    bool[] separateLoaded = new bool[15];
    void IncreaseProgress(int totalTask = -1)
    {
        if (totalTask == -1)
        {
            totalTask = 32;
        }
        float val = (float)(1 / (float)totalTask);
        loadingAmount += val;
    }


    void Go()
    {
        bool isSeperateLoadDone = true;

        foreach (bool isDone in separateLoaded)
        {
            if (!isDone)
            {
                isSeperateLoadDone = false;
            }
        }
        if (isSeperateLoadDone)
        {
            CancelInvoke("Go");
        }
        else
        {
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Splash");

    }
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
#if UNITY_EDITOR
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>(); // scene.GetRootGameObjects();
        foreach (var go in allGameObjects)
        {
            AssetBundleManager.FixShaders(go);
        }
#endif
    }
}
