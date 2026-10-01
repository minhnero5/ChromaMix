using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
   
    public TextMeshProUGUI inappOn, adsOn,coinOn;
    private Color32 colordefault = new Color32(255, 255, 255, 255);
    private Color32 colorchange= new Color32(255, 255, 0, 255);
    public TextMeshProUGUI inappOff, adsOff, coinOff;

    public Button inappBtn, adsBtn,coinBtn;
    public TextMeshProUGUI inappBtntxt, adsBtntxt, coinBtntxt;
    public GameObject inappPanel, adsPanel,coinPanel;

    public void ShowShop()
    {
        gameObject.SetActive(true);
    }

    public void HideShop()
    {
        gameObject.SetActive(false);
    }
    public void ShowInappPanel()
    {
        SetAllSpriteOff();
        CloseAllPanel();
        // inappBtn.GetComponent<Image>().sprite = inappOn;
        inappBtntxt.color = colorchange;
        inappPanel.SetActive(true);
    }

    public void ShowAdsPanel()
    {
        SetAllSpriteOff();
        CloseAllPanel();
        //adsBtn.GetComponent<Image>().sprite = adsOn;    
        adsBtntxt.color = colorchange;
        adsPanel.SetActive(true);
    }

    public void ShowCoinPanel()
    {
        SetAllSpriteOff();
        CloseAllPanel();
        //coinBtn.GetComponent<Image>().sprite = coinOn;
        coinBtntxt.color = colorchange;
        coinPanel.SetActive(true);
    }

    void CloseAllPanel()
    {
        inappPanel.SetActive(false);
        adsPanel.SetActive(false);
        coinPanel.SetActive(false);
    }

    void SetAllSpriteOff()
    {
        //inappBtn.GetComponent<Image>().sprite = inappOff;
        //adsBtn.GetComponent<Image>().sprite = adsOff;
        //coinBtn.GetComponent<Image>().sprite = coinOff;
        inappBtntxt.color = colordefault;
        adsBtntxt.color = colordefault;
        coinBtntxt.color = colordefault;
    }
}
