using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public GameObject MenuCanvas;
    bool MenuOpen = false;
    

    private void Start()
    {
        MenuCanvas.SetActive(false);
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
           
            SetPause();  
        }
        
    }

    public void SetPause() 
        {
        if (!MenuOpen)
        {
           
            MenuCanvas.SetActive(true);
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            

        }

        else
        {
            
            MenuCanvas.SetActive(false);
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
           
        }

        MenuOpen = !MenuOpen;

        } 


    public void Restart()
    {
        Debug.Log("Restart");
        SceneManager.LoadScene(0);
        MenuCanvas.SetActive(false);
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public void exit()
    {
        Application.Quit();
    }

}
