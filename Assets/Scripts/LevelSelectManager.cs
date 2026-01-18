using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreLevel1Text;
    [SerializeField] private TextMeshProUGUI highScoreLevel2Text;
    [SerializeField] private TextMeshProUGUI highScoreLevel3Text;


    private void Awake()
    {
        Time.timeScale = 1;
        if (PlayerPrefs.HasKey(GameConstants.HIGHSCORELevel1_KEY))
        {
            highScoreLevel1Text.text = "Score :" + PlayerPrefs.GetInt(GameConstants.HIGHSCORELevel1_KEY).ToString("0000");
        }
        if (PlayerPrefs.HasKey(GameConstants.HIGHSCORELevel2_KEY))
        {
            highScoreLevel2Text.text = "Score :" + PlayerPrefs.GetInt(GameConstants.HIGHSCORELevel2_KEY).ToString("0000");
        }
        if (PlayerPrefs.HasKey(GameConstants.HIGHSCORELevel3_KEY))
        {
            highScoreLevel3Text.text = "Score :" + PlayerPrefs.GetInt(GameConstants.HIGHSCORELevel3_KEY).ToString("0000");
        }
    }

    private void Start()
    {
        //Get children length of Button Container --> Number levels
        GameObject buttonContainer = GameObject.Find("ButtonContainer");

        //dinamicaly put on every button the listener loadLevelSelection
        for (int i = 0; i < buttonContainer.transform.childCount; i++)
        {
            int index = i;
            //if the level button is lower or equal than the maxlevel reached
            if (index + 1 <= PlayerPrefs.GetInt(GameConstants.MAXLEVEL_KEY))
            {
                buttonContainer.transform.GetChild(index).GetComponent<Button>().interactable = true;
                buttonContainer.transform.GetChild(index).GetComponent<Button>().onClick.AddListener(() => LoadLevelSelection(index + 1));
            }
            else
            {
                buttonContainer.transform.GetChild(index).GetComponent<Button>().interactable = false;
            }

        }
    }

    /// <summary>
    /// Load the scene with LevelIndex + 1 to get the actual level name
    /// </summary>
    /// <param name="index">the index of the button in the array</param>
    void LoadLevelSelection(int index)
    {
        string levelName = "Level" + index;
        if (levelName=="Level1")
        {
            SceneManager.LoadScene("CinematicLevel");
        }
        else
        {
            SceneManager.LoadScene(levelName);
        }
    }

    public void ButtonBack()
    {
        //Go to the Menu
        SceneManager.LoadScene(GameConstants.MAINMENU_LEVEL);
    }
}
