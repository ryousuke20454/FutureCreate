using UnityEngine;
using UnityEngine.SceneManagement; // �Y��Ȃ��I�I
public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string SampleGameScene)
    {
        SceneManager.LoadScene(SampleGameScene);
    }
}