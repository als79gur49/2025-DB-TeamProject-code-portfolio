using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField]
    private Button loadScenebutton;
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private float delayTime = 0.1f;

    private void Awake()
    {
        loadScenebutton?.onClick.AddListener(()=>Invoke("LoadScene", delayTime));
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
