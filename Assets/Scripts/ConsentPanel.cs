using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsentPanel : ShowHidable
{

    [SerializeField] private Button _privatePolicyBtn;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void OnClickYes()
    {
      
        Hide();
    }

    public void OnClickPrivacy()
    {
        
    }

    public void OnClickNo()
    {
        
        Hide();
    }
}
