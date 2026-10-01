using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalAssetBundle : MonoBehaviour
{
    public GameObject privacyPanel;
    public List<string> assetbudleName = new List<string>();

    public List<string> scenes = new List<string>();

    int isAcceptPrivacy;
    private void Start()
    {
        //PlayerPrefs.DeleteKey("PRIVACY");
        StartGetBundle();
        isAcceptPrivacy = PlayerPrefs.GetInt("PRIVACY");
        Debug.Log(isAcceptPrivacy);
        if(isAcceptPrivacy == 0)
        {
            privacyPanel.SetActive(true);
        }
        if (isAcceptPrivacy == 1)
        {
            privacyPanel.SetActive(false);
        }
    }
    void StartGetBundle()
    {
        StartCoroutine(LoadBundleList(() => StartCoroutine(LoadScene())));
    }


    bool IsAccpetPrivacy()
    {
        if(isAcceptPrivacy == 1) return true;
        return false;
    }
    void LoadAssetBundle(string name)
    {
        var assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, name));
        string[] scenePaths = assetBundle.GetAllScenePaths();
        scenes.Add(scenePaths[0]);      
    }

    IEnumerator LoadBundleList(Action onComplete = null)
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < assetbudleName.Count; i++)
        {
            LoadAssetBundle(assetbudleName[i]);
            yield return new WaitForSeconds(1f);
        }       
        onComplete?.Invoke();
    }

    IEnumerator LoadScene()
    {
        while(!IsAccpetPrivacy())
        {
            yield return new WaitForSeconds(0.1f);
        }
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenes[0]);
        SceneManager.LoadScene(sceneName);
    }

    public void AcceptPrivacy()
    {
        isAcceptPrivacy = 1;
        PlayerPrefs.SetInt("PRIVACY", isAcceptPrivacy);
        privacyPanel.SetActive(false);
    }

    
}