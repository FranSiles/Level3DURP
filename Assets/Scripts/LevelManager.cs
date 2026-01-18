using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [Header("Panels of endgame")]
    [SerializeField] private GameObject panelWin;
    [SerializeField] private GameObject panelLose;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject configurationPanel;

    [Header("Score")]
    [SerializeField] private int scorePerLife = 250;
    [SerializeField] private int scorePerEnemy = 50;

    [Header("Text for panels")]
    [SerializeField] private TextMeshProUGUI pointswin;
    [SerializeField] private TextMeshProUGUI enemies;

    private int enemiesOnGame;
    private int CurrentEnemies;
    private int scoreLife;
    private int scoreEnemies;
    private int totalScore;
    private GameObject player;
    private void Awake()
    {
        Instance = this;
        enemiesOnGame = GameObject.FindGameObjectsWithTag("Enemy").Length;
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("Enemigos en el mapa: "+ enemiesOnGame);
    }
    private void Update()
    {
        if (player.GetComponent<PlayerContoller>().CurrentLives<=0)
        {
            ActivateLosePanel();
        }
    }
    public void ActivateWinPanel()
    {
        PauseGameObjects();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        panelWin.SetActive(true);
        scoreLife = scorePerLife * player.GetComponent<PlayerContoller>().CurrentLives;
        Debug.Log("Score por la vida: " + scoreLife);
        CurrentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int enemyKills = enemiesOnGame - CurrentEnemies;
        scoreEnemies = scorePerEnemy * enemyKills;
        Debug.Log("Score por enemigos: " + scoreEnemies);
        Debug.Log("enemigos:"+enemyKills);
        totalScore=scoreLife+scoreEnemies;
        pointswin.text = "Score: " + totalScore.ToString("0000");
        enemies.text="Enemies:"+enemyKills.ToString("00")+"/"+enemiesOnGame.ToString("00");
        

    }
    private void ActivateLosePanel()
    {
        PauseGameObjects();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        panelLose.SetActive(true);
    }
    public void ActivatePausePanel()
    {
        pausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PauseGameObjects();
    }
    public void DesactivatePauseLevel()
    {
        pausePanel.SetActive(false);
        ResumeGameObjects();
    }
    public void Restart()
    {
        ResumeGameObjects();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        pausePanel.SetActive(false);
    }
    public void Continue()
    {
        ResumeGameObjects();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pausePanel.SetActive(false);
    }
    public void Configuration()
    {
        pausePanel.SetActive(false);
        configurationPanel.SetActive(true);
    }
    public void back()
    {
        configurationPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
    public void nextLevel()
    {
        CheckMaxLevel();
        //total number of levels
        int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Substring(5));
        CheckRecordPoints(currentLevel);
        switch (currentLevel)
        {
            case 1:
                SceneManager.LoadScene("Level2");
                break;
            case 2:
                SceneManager.LoadScene("Level3");
                break;
        }
    }

    private void CheckRecordPoints(int level)
    {
        int highScore = 0;
        switch (level)
        {
            case 1:
                highScore = PlayerPrefs.GetInt(GameConstants.HIGHSCORELevel1_KEY, 0000);//Put value 0 if not exist a value
                break;
            case 2:
                highScore = PlayerPrefs.GetInt(GameConstants.HIGHSCORELevel2_KEY, 0000);//Put value 0 if not exist a value
                break;
            case 3:
                highScore = PlayerPrefs.GetInt(GameConstants.HIGHSCORELevel3_KEY, 0000);//Put value 0 if not exist a value
                break;
        }
        if (totalScore > highScore)
        {
            switch (level)
            {
                case 1:
                    PlayerPrefs.SetInt(GameConstants.HIGHSCORELevel1_KEY, totalScore);
                    break;
                case 2:
                    PlayerPrefs.SetInt(GameConstants.HIGHSCORELevel2_KEY, totalScore);
                    break;
                case 3:
                    PlayerPrefs.SetInt(GameConstants.HIGHSCORELevel3_KEY, totalScore);
                    break;
            }

        }
    }
    public void MainMenu()
    {
        int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Substring(5));
        CheckMaxLevel();
        CheckRecordPoints(currentLevel);
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Check Max Level
    /// </summary>
    void CheckMaxLevel()
    {
        //ask for the current level
        int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Substring(5));
        //Save the next Level for the first time
        if (!PlayerPrefs.HasKey(GameConstants.MAXLEVEL_KEY))
        {
            PlayerPrefs.SetInt(GameConstants.MAXLEVEL_KEY, currentLevel + 1);
        }
        else
        {
            if (PlayerPrefs.GetInt(GameConstants.MAXLEVEL_KEY) < currentLevel + 1)
            {
                PlayerPrefs.SetInt(GameConstants.MAXLEVEL_KEY, currentLevel + 1);
            }

        }
    }

    private void PauseGameObjects()
    {
        // Pausar jugador
        if (player != null)
        {
            var playerController = player.GetComponent<PlayerMovement>();
            if (playerController != null)
                playerController.enabled = false;

            var weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
                weaponController.enabled = false;
            var playerInput = player.GetComponent<PlayerInput>();
            if(playerInput!=null)
                playerInput.enabled = false;
        }

        // Pausar enemigos
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            var ai = enemy.GetComponent<EnemyController>();
            var animation = enemy.GetComponent<Animator>();
            var navmesh=enemy.GetComponent<NavMeshAgent>();
            if (ai != null)
            {
                ai.enabled = false;
                animation.enabled = false;
                navmesh.enabled = false;
            }  
        }

        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }
    private void ResumeGameObjects()
    {
        // Reactivar jugador
        if (player != null)
        {
            var playerController = player.GetComponent<PlayerMovement>();
            if (playerController != null)
                playerController.enabled = true;

            var weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
                weaponController.enabled = true;
            var playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
                playerInput.enabled = true;
        }

        // Reactivar enemigos
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            var ai = enemy.GetComponent<EnemyController>(); // Tu script de enemigos
            var animation =enemy.GetComponent<Animator>();
            var navmesh = enemy.GetComponent<NavMeshAgent>();
            if (ai != null)
            {
                ai.enabled = true;
                animation.enabled = true;
                navmesh.enabled = true;
            }
                
        }

        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
    }

}
