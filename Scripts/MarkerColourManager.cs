using UnityEngine;
using UnityEngine.Events;
using static DotColour;

public class MarkerColourManager : MonoBehaviour, IColourManager
{
    public Material markerMat;
    public Material markerIndicatorMaterial;
    public GameObject currentMarker;


    public Color markerStartingColour;

    public float red;
    public float green;
    public float blue;

    public UnityEvent<float> redSlider;
    public UnityEvent<float> greenSlider;
    public UnityEvent<float> blueSlider;


    public float Red
    {
        get => red;
        set
        {

            red = value;
            UpdateColour();

        }
    }
    public float Green
    {
        get => green;
        set
        {

            green = value;
            UpdateColour();
        }
    }
    public float Blue
    {
        get => blue;
        set
        {

            blue = value;
            UpdateColour();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame

    void Awake()
    {
        if (markerMat != null) { 

            markerMat.color = markerStartingColour;

            red = markerMat.color.r;
            green = markerMat.color.g;
            blue = markerMat.color.b;        

        }

    }

    // Update is called once per frame
    void Update()
    {

    }


    void UpdateColour() {

        if (markerMat != null)
        {

            markerMat.color = new Color(red, green, blue);

            markerIndicatorMaterial.color = new Color(red, green, blue, 1.0f);
        }
        else
        {

            markerIndicatorMaterial.color = new Color(0f, 0f, 0f, 0f);

        }

    }

    public void UpdateCurrentMarker(GameObject marker) {

        markerMat = marker.GetComponent<Renderer>().material;
        currentMarker= marker;

        red = markerMat.color.r;
        green = markerMat.color.g;
        blue = markerMat.color.b;

        UpdateColour();

        redSlider.Invoke(red);
        greenSlider.Invoke(green);
        blueSlider.Invoke(blue);
    
    }

    public void ChangeRed(float redHue)
    {

        red = redHue;

    }

    public void ChangeBlue(float blueHue)
    {

        blue = blueHue;

    }
    public void ChangeGreen(float greenHue)
    {

        green = greenHue;

    }


}

