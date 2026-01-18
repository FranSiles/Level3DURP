using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    private int currentLives;
    [SerializeField] private int maxLives;

    public int CurrentLives { get => currentLives; set => currentLives = value; }

    private void Awake()
    {
        currentLives = maxLives;
    }

    /// <summary>
    /// When the player receive damage
    /// </summary>
    /// <param name="quantity"></param>
    public void DamagePlayer(int quantity)
    {
        currentLives -= quantity;

        //HUD Show Damage Flash
        HudController.Instance.ShowDamageFlash();

        if (currentLives <= 0)
        {
            LevelManager.Instance.ActivateLosePanel();
        }
    }

}
