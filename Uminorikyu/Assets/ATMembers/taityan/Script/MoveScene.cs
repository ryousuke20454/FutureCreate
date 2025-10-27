using UnityEngine;
using UnityEngine.SceneManagement; // ñYÇÍÇ»Ç¢ÅIÅI
public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string GameSceneName)
    {
        SceneManager.LoadScene(GameSceneName);
    }
}