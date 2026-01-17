using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private Image damageFlash;
    [SerializeField] private float damageTime;

    [Header("Components of HUD")]
    [SerializeField] private TextMeshProUGUI countammo;
    [SerializeField] private TextMeshProUGUI countGranade;
    [SerializeField] private TextMeshProUGUI countLife;

    private Coroutine disappearCouroutine;
    private GameObject player;
    private WeaponController weaponController;
    private GranadeController granadeController;
    private PlayerContoller playerContoller;

    public static HudController Instance;

    private void Awake()
    {
        Instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        weaponController=player.GetComponent<WeaponController>();
        granadeController = player.GetComponent<GranadeController>();
        playerContoller=player.GetComponent<PlayerContoller>();
    }

    private void Start()
    {
        if (!weaponController.InfiniteAmmo)
        {
            countammo.text = weaponController.CurrentAmmo.ToString("00") + "/" + weaponController.AmmoReserve.ToString("000");
        }
        else
        {
            countammo.text = "Infinite";
        }
        countGranade.text = granadeController.CurrentGranade.ToString("0");
        countLife.text = playerContoller.CurrentLives.ToString("00");

    }
    private void Update()
    {
        if (!weaponController.InfiniteAmmo)
        {
            countammo.text = weaponController.CurrentAmmo.ToString("00") + "/" + weaponController.AmmoReserve.ToString("000");
        }
        countGranade.text = granadeController.CurrentGranade.ToString("0");
        countLife.text = playerContoller.CurrentLives.ToString("00");
    }

    /// <summary>
    /// Turn on Damage Flash when the Player receive a shoot
    /// </summary>
    public void ShowDamageFlash()
    {
        //if there is a coroutine running stop it
        if (disappearCouroutine != null)
            StopCoroutine(disappearCouroutine);

        //restart the image Color        
        damageFlash.color = Color.white;


        //Start coroutine
        disappearCouroutine = StartCoroutine(DamageDissapear());
    }

    private IEnumerator DamageDissapear()
    {
        //Alpha variable reset to 1
        float alpha = 1.0f;

        while (alpha > 0.0f)
        {
            alpha -= (1.0f / damageTime) * Time.deltaTime;
            damageFlash.color = new Color(1.0f, 1.0f, 1.0f, alpha);
            yield return null;
        }
    }
}
