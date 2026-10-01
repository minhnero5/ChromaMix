
using System;
using Game;
using MyGame;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;

public class GamePlayPanel : ShowHidable
{
    [SerializeField] private Button _undoBtn;
    [SerializeField] private Text _lvlTxt;
    public TextMeshProUGUI undoCountTxt;

    private void Start()
    {
        _lvlTxt.text = $"Level {LevelManager.Instance.Level.no}";
        int undoItem = PlayerPrefs.GetInt(ItemType.UNDO.ToString(), 5);
        undoCountTxt.text = undoItem.ToString();
    }

    public void OnClickUndo()
    {
        int undoItem = PlayerPrefs.GetInt(ItemType.UNDO.ToString(), 5);
        if(undoItem > 0) {
            LevelManager.Instance.OnClickUndo();
            undoItem--;
            PlayerPrefs.SetInt(ItemType.UNDO.ToString(), undoItem);
        }
        undoCountTxt.text = undoItem.ToString();
    }

    public void OnClickRestart()
    {
        GameManager.LoadGame(new LoadGameData
        {
            Level = LevelManager.Instance.Level,
            GameMode = LevelManager.Instance.GameMode,
        },false);
    }

    public void OnClickSkip()
    {
       

        SharedUIManager.PopUpPanel.ShowAsConfirmation("Skip","Watch Video to skip this level", success =>
        {
            if (!success)
            {
                return;
            }
            else
            {
                AdsManager.Instance.ShowRewardedAd(UIManager.Instance.LoadNextLevel);
                // LevelManager.Instance.Level.no +=1;
               // ResourceManager.CompleteLevel(LevelManager.Instance.GameMode, LevelManager.Instance.Level.no);
            }
        });

    }

    public void OnClickMenu()
    {
        SharedUIManager.PopUpPanel.ShowAsConfirmation("Exit?","Are you sure want to exit the game?", success =>
        {
            if (!success)
                return;

            GameManager.LoadScene("MainMenu");
        });
    }

    private void Update()
    {
        _undoBtn.interactable = LevelManager.Instance.HaveUndo;
    }
}