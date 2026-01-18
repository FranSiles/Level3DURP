using UnityEngine;
using UnityEngine.SceneManagement;

public class Cinematic : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("Level1");
    }
}
