
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationScene : MonoBehaviour
{
    public void ClickStart()
    {
        SceneManager.LoadScene(1);
    }

    public void ClickEnd()
    {
        SceneManager.LoadScene(0);
    }
}
