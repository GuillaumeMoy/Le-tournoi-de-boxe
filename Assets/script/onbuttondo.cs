using UnityEngine;

public class onbuttondo : MonoBehaviour
{
    public GameObject menu;

    public int Count = 0;

    void Start()
    {
        Count = 1;
        Debug.Log(Count);

        // Make sure the menu starts hidden
        menu.SetActive(false);
    }

    public void AdditionBoutton()
    {
        Count++;
        Debug.Log(Count);

        // Open the menu
        menu.SetActive(true);
    }

    public void CloseMenu()
    {
        // Close the menu
        menu.SetActive(false);
    }
}