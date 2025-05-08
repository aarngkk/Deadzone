using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneController sceneController;

    public void Play()
    {
        sceneController.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
