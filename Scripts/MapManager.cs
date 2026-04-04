using UnityEngine;

public class MapManager : MonoBehaviour
{

    public enum MapButton { Next, Back }

    public GameObject[] maps;
    private int currentMap = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maps[currentMap].SetActive(true);
    }

    private void Awake()
    {
        currentMap = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleMapChange(MapButton option) {

        maps[currentMap].SetActive(false);
        if (option == MapButton.Next) { 
        
            currentMap++;

            if (currentMap >= maps.Length) { 
            
                currentMap = maps.Length - 1;

            }

        } else if (option == MapButton.Back)
        {

            currentMap--;

            if (currentMap < 0) { 
                currentMap = 0;
            }

        }

        maps[currentMap].SetActive(true);

    
    }
}
