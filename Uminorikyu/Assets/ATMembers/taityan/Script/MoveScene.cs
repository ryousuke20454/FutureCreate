using UnityEngine;
using UnityEngine.SceneManagement; // �Y��Ȃ��I�I
public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string GameSceneName)
    {
        SceneManager.LoadScene(GameSceneName);
    }
}