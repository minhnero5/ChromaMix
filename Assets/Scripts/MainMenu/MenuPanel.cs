

using UnityEngine;

namespace MainMenu
{
    public class MenuPanel : MonoBehaviour
    {

        public void OnClickPlay()
        {
            UIManager.Instance.GameModePanel.Show();
        }

        public void OnClickExit()
        {

        }

        public void OnClickPrivacy(string link)
        {
            Application.OpenURL(link);
        }

    }
}