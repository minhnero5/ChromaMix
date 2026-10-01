using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class SubCheck : MonoBehaviour
{
    public Text price;
    public GameObject btnAds;
    public void OnProductFetched(Product product)
    {
        if (product.availableToPurchase)
        {
            btnAds.SetActive(true);
            if (price != null)
            {
                price.text = product.metadata.localizedPriceString;
            }
        }
        else
        {
            btnAds.SetActive(false);
        }
    }
}
