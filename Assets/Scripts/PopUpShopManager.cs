using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpShopManager : MonoBehaviour
{
    public static PopUpShopManager instance;
    [SerializeField] private GameObject boughtPopup;
    [SerializeField] private TextMeshProUGUI buyTxtTrue;
    [SerializeField] private TextMeshProUGUI buyTxtFalse;
    [SerializeField] private GameObject imgUndo;
    private void Awake()
    {
        instance = this;
    }
    public void Popup(string content,int index)
    {   
        boughtPopup.SetActive(true);
        if(index == 1)
        {
            buyTxtTrue.gameObject.SetActive(true);
            buyTxtFalse.gameObject.SetActive(false);
            buyTxtTrue.text = content;
        }
        else
        {
            buyTxtTrue.gameObject.SetActive(false);
            buyTxtFalse.gameObject.SetActive(true);
            buyTxtFalse.text = content;
        }

    }
    public void ExitPopup()
    {
        boughtPopup.SetActive(false);
    }
    public void SetActiveImageUndo(bool isActive)
    {
        imgUndo.SetActive(isActive);
    }
}
