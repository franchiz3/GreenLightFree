using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpShot  {

    protected int min,max;
    public string displayName;
    public int longMin, longMax, midMin, midMax;
    public int pullRate;
    public int midGreen;
    public bool custom;
    public float pureGreen;


    public JumpShot(string displayName, int min, int max, bool custom, int rate, float green, int midGreen)
    {
        this.displayName = displayName;
        pureGreen = green;
        this.custom = false;
        pullRate = 75;
        this.max = max;
        this.min = min;
        this.midGreen = midGreen;
    }

    public JumpShot(string displayName, int midMin, int midMax, int longMin, int longMax)
    {
       // Debug.Log(midMin + " " + midMax + " " + longMin + " " +longMax );
        this.displayName = displayName;
        custom = true;
        this.midMax = midMax;
        this.midMin = midMin;
        this.longMax = longMax;
        this.longMin = longMin;
        if (_Player.longRange)
        {
            midGreen = (longMin + longMax) / 2;
            pureGreen = 7;
        }
        else
        {
            midGreen = (midMin + midMax) / 2;
            pureGreen = 9;
        }
           
    }

    //need to add variations for size and position<--- not necessary only rating 
    #region setsgets
    public int GetMin() { return min; }
    public int GetMax() { return max; }
    #endregion
}
