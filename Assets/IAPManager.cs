using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
    [SerializeField]
    ItemManager itemManager;
    [SerializeField]
    RemoveAds_Custom rmAds;
    public void BuyCoin(int value)
    {
        itemManager.BuyItem(ItemType.COIN, value);
        itemManager.LoadItem(ItemType.COIN);
    }

    public void BuyUndoItemx5(int price)
    {
        if (itemManager.CanBuy(price))
        {
            PopUpShopManager.instance.SetActiveImageUndo(true);
            itemManager.BuyItem(ItemType.UNDO, 5);
            itemManager.LoadItem(ItemType.UNDO);
            itemManager.LoadItem(ItemType.COIN);
            PopUpShopManager.instance.Popup("+5 Undo", 1);
        }
        else
        {
            PopUpShopManager.instance.SetActiveImageUndo(false);
            PopUpShopManager.instance.Popup("Not Enough Coin", 0);
        }

    }

    public void BuyUndoItemx10(int price)
    {
        if (itemManager.CanBuy(price))
        {
            PopUpShopManager.instance.SetActiveImageUndo(true);
            itemManager.BuyItem(ItemType.UNDO, 10);
            itemManager.LoadItem(ItemType.UNDO);
            itemManager.LoadItem(ItemType.COIN);
            PopUpShopManager.instance.Popup("+10 Undo", 1);
        }
        else
        {
            PopUpShopManager.instance.SetActiveImageUndo(false);
            PopUpShopManager.instance.Popup("Not Enough Coin", 0);
        }
    }
    public void BuyRemoveAds(int days)
    {
        rmAds.SetRemoveAdsExpirationDate(days);
    }
 
}
