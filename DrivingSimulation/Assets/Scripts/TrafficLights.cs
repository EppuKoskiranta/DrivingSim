using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TrafficLights : MonoBehaviour
{
    public enum LightState
    {
        RED,
        YELLOW,
        YELLOWRED,
        GREEN,
    }


    private LightState state;



    public GameObject greenLight;
    public GameObject yellowLight;
    public GameObject redLight;

    private Color redColor;
    private Color yellowColor;
    private Color greenColor;

    private void Awake()
    {
        redColor = redLight.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
        yellowColor = yellowLight.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
        greenColor = greenLight.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");


        Red();
    }


    public LightState GetState()
    {
        return state;
    }


    public IEnumerator TurnGreen()
    {
        if (state == LightState.RED)
        {
            YellowRed();
            yield return new WaitForSecondsRealtime(0.8f);
            Green();
        }
    }

    public IEnumerator TurnRed()
    {
        if (state == LightState.GREEN)
        {
            Yellow();
            yield return new WaitForSecondsRealtime(0.8f);
            Red();
        }
    }


    public void Red()
    {
        state = LightState.RED;
        if (greenLight != null && yellowLight != null && redLight != null)
        {
            redLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(redColor.r, redColor.g, redColor.b, redColor.a) * 1f);
            yellowLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(yellowColor.r, yellowColor.g, yellowColor.b, yellowColor.a) * -1);
            greenLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(greenColor.r, greenColor.g, greenColor.b, greenColor.a) * -1);
        }
        
    }

    public void YellowRed()
    {
        state = LightState.YELLOWRED;
        if (greenLight != null && yellowLight != null && redLight != null)
        {
            redLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(redColor.r, redColor.g, redColor.b, redColor.a) * 1f);
            yellowLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(yellowColor.r, yellowColor.g, yellowColor.b, yellowColor.a) * 1);
            greenLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(greenColor.r, greenColor.g, greenColor.b, greenColor.a) * -1);
        }
    }

    public void Yellow()
    {
        state = LightState.YELLOW;
        if (greenLight != null && yellowLight != null && redLight != null)
        {
            redLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(redColor.r, redColor.g, redColor.b, redColor.a) * -1f);
            yellowLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(yellowColor.r, yellowColor.g, yellowColor.b, yellowColor.a) * 1);
            greenLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(greenColor.r, greenColor.g, greenColor.b, greenColor.a) * -1);
        }
    }

    public void Green()
    {
        state = LightState.GREEN;
        if (greenLight != null && yellowLight != null && redLight != null)
        {
            redLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(redColor.r, redColor.g, redColor.b, redColor.a) * -1f);
            yellowLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(yellowColor.r, yellowColor.g, yellowColor.b, yellowColor.a) * -1);
            greenLight.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", new Vector4(greenColor.r, greenColor.g, greenColor.b, greenColor.a) * 1);
        }

    }
}
