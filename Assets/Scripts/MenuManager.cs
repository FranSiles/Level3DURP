using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        //First time we save Level 1 as max level
        if (!PlayerPrefs.HasKey(GameConstants.MAXLEVEL_KEY))
        {
            PlayerPrefs.SetInt(GameConstants.MAXLEVEL_KEY, 1);
        }
    }

    public void SelectLevel()
    {
        //Go to the next Scene (Index Scene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void CreditsLevel()
    {
        //Go to Credits
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    public void MenuLevel()
    {
        //Go to Credits
        SceneManager.LoadScene(GameConstants.MAINMENU_LEVEL);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
