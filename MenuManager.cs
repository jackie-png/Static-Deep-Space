using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public enum MenuOption { None, Map, ScannerDot, Markers, PlayerSetting, Back }

    public GameObject[] options;

    public GameObject[] menus;

    public GameObject mainMenu;

    private GameObject currentMenu;

    private int currentMenuNum = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectOption(MenuOption optionPicked) { 
    
        switch (optionPicked)
        {
            case MenuOption.Map:
                currentMenu = menus[0];
                break;
            case MenuOption.ScannerDot:
                currentMenu = menus[1];
                break;
            case MenuOption.Markers:
                currentMenu = menus[2];
                break;
            case MenuOption.PlayerSetting:
                currentMenu = menus[3];
                break;
        }

        currentMenuNum = (int)optionPicked;
        TurnOnSelectedMenu();
    
    }
    void TurnOnSelectedMenu()
    {
        mainMenu.SetActive(false);

        for (int i = 0; i < menus.Length; i++)
        {
            bool isSelected = (i == currentMenuNum);
            menus[i].SetActive(isSelected);

            if (isSelected)
            {
                Debug.Log("current Menu " + i);
            }
        }
    }

    public void GoBackMainMenu(MenuOption option) {

        menus[currentMenuNum].SetActive(false);
        mainMenu.SetActive(true);
    
    }
}
