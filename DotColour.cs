using UnityEngine;
using UnityEngine.Events;
public class DotColour : MonoBehaviour, IColourManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Material closeDotsPanel;
    public Material farDotsPanel;

    public Color closeDotStartingColour;
    public Color farDotStartingColour;

    public float red;
    public float green;
    public float blue;

    public enum Dots {Close, Far, None}
    private Dots currentDot = Dots.Close;

    public UnityEvent<float> redSlider;
    public UnityEvent<float> greenSlider;
    public UnityEvent<float> blueSlider;

    public UnityEvent<Color, Color> scannerColours;

    public float Red { 
        get => red; 
        set { 
        
            red = value;
            UpdateColour();
        
        } }
    public float Green
    {
        get => green;
        set
        {

            green = value;
            UpdateColour();
        }
    }
    public float Blue { 
        get => blue;
        set { 
        
            blue = value;
            UpdateColour();
        }
    }

    void Awake()
    {
        closeDotsPanel.color = closeDotStartingColour;
        farDotsPanel.color = farDotStartingColour;

        red = closeDotsPanel.color.r;
        green = closeDotsPanel.color.g;
        blue = closeDotsPanel.color.b;
    }

    // Update is called once per frame
    void Update()
    {

    }


    void UpdateColour() {

        Color newColour = new Color(red, green, blue);

        if (currentDot == Dots.Close)
        {

            closeDotsPanel.color = newColour;
            scannerColours.Invoke(closeDotsPanel.color, farDotsPanel.color);


        }
        else if (currentDot == Dots.Far)
        {

            farDotsPanel.color = newColour;
            scannerColours.Invoke(closeDotsPanel.color, farDotsPanel.color);


        }


    }

    public void ChangeWhichDot(Dots closeOrFar) {

        currentDot= closeOrFar;

        if (currentDot == Dots.Close)
        {

            red = closeDotsPanel.color.r;
            green = closeDotsPanel.color.g;
            blue = closeDotsPanel.color.b;

        }
        else if (currentDot == Dots.Far) { 
        
            red = farDotsPanel.color.r;
            green = farDotsPanel.color.g;
            blue = farDotsPanel.color.b;
        
        }


        redSlider.Invoke(red);
        greenSlider.Invoke(green);
        blueSlider.Invoke(blue);


    }

    public void ChangeRed(float redHue) { 
    
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
