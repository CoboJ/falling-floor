using System.Collections;
using UnityEngine;
using Doozy.Engine.Progress;
using UnityEngine.SceneManagement;

public class LogoScript : MonoBehaviour
{
    private Progressor progressor;
    private bool isClosing;

    [SerializeField] private string sceneToLoad;
    [SerializeField] private float waitToClose;

    void Awake() 
    {
        progressor = GetComponent<Progressor>();    
    }

    void Start()
    {
        progressor.SetProgress(1);
    }
    
    public void Progress(float progress)
    {
        if(progress == 1 && !isClosing)
        {
            isClosing = true;
            StartCoroutine(CloseLogo());
        }
        
        if(progress == 0 && isClosing)
        {
            isClosing = false;
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        }
    }

    IEnumerator CloseLogo()
    {
        yield return new WaitForSeconds(waitToClose);

        progressor.SetProgress(0);
    }
}
