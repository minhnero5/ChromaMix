//using D2S.Ads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAds_Custom : MonoBehaviour
{
    public static RemoveAds_Custom Instance;

    private DateTime removeAdsExpirationDate;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
        //PlayerPrefs.SetString("RemoveAdsExpirationDate", "0");
        // Kiểm tra thời gian kết thúc của việc loại bỏ quảng cáo và cập nhật nó.
        if (PlayerPrefs.HasKey("RemoveAdsExpirationDate"))
        {
            long ticks = Convert.ToInt64(PlayerPrefs.GetString("RemoveAdsExpirationDate"));
            removeAdsExpirationDate = new DateTime(ticks);
        }
    }

    public void SetRemoveAdsExpirationDate(int days)
    {
        removeAdsExpirationDate = DateTime.Now.AddDays(days);
        PlayerPrefs.SetString("RemoveAdsExpirationDate", removeAdsExpirationDate.Ticks.ToString());
        PlayerPrefs.Save();
    }

    public DateTime GetRemoveAdsExpirationDate()
    {
        return removeAdsExpirationDate;
    }

    public void HideBannerIfNecessary()
    {
        if (DateTime.Now < removeAdsExpirationDate)
        {
           // AdsController.Instance.HideBanner();
        }
    }
    public bool HasPurchasedRemoveAds()
    {
        return DateTime.Now <= removeAdsExpirationDate;
    }
}
