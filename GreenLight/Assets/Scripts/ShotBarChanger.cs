using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBarChanger : MonoBehaviour {

    public static Color[] colors = new Color[11];
    public static int currentColor;
    public UnityEngine.UI.Image buttonColor;

    private void Start()
    {
        colors[0] = Color.yellow;
        colors[1] = Color.white;
        colors[2] = Color.red;
        colors[3] = Color.magenta;
        colors[4] = Color.grey;
        colors[5] = Color.green;
        colors[6] = Color.gray;
        colors[7] = Color.cyan;
        colors[8] = Color.blue;
        colors[9] = Color.clear;
        colors[10] = Color.black;
        currentColor = PlayerPrefs.GetInt("barColor");
        _Player.barColor = colors[currentColor];
        buttonColor.color = colors[currentColor];
    }

    public void ChangeColor()
    {
        if (currentColor == 10)
            currentColor = 0;
        else
            currentColor++;
        buttonColor.color = colors[currentColor];
        PlayerPrefs.SetInt("barColor", currentColor);
        _Player.barColor = colors[currentColor];
    }
}
