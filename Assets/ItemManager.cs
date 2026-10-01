using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class ItemManager : MonoBehaviour
{
   
    public TextMeshProUGUI undoText, coinText;

    public int undoCount, coin;

    private void OnEnable()
    {
        LoadItem(ItemType.COIN);
        LoadItem(ItemType.UNDO);
    }

    public void SaveItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.COIN:
                PlayerPrefs.SetInt(ItemType.COIN.ToString(), coin);              
                Debug.Log("Save Coin " + coin);
                break;         
            case ItemType.UNDO:
                PlayerPrefs.SetInt(ItemType.UNDO.ToString(), undoCount);           
                Debug.Log("Save Rotate "+ undoCount);
                break;
        }      
    }

    public void LoadItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.COIN:
                coin = PlayerPrefs.GetInt(ItemType.COIN.ToString(), 0);
                coinText.text = coin.ToString();
                break;        
            case ItemType.UNDO:
                undoCount = PlayerPrefs.GetInt(ItemType.UNDO.ToString(), 5);
                undoText.text = undoCount.ToString();
                break;
        }
    }

    public void BuyItem( ItemType itemType,int value)
    {       
        switch (itemType)
        {
            case ItemType.COIN:
                coin += value;
                SaveItem(ItemType.COIN);
                break;         
            case ItemType.UNDO:
                undoCount+= value;
                SaveItem(ItemType.UNDO);
                break;
        }
    }

    public bool CanBuy(int price)
    {
        if(price > coin)
        {
            return false;
        }
        coin -= price;
        SaveItem(ItemType.COIN);
        return true;
    }

    public bool CanUseItem()
    {
        if(undoCount > 0)
        {
            undoCount--;
            SaveItem(ItemType.COIN);
            return true;
        }
        return false;
    }
}

public enum ItemType{
    COIN,
    UNDO,   
}
