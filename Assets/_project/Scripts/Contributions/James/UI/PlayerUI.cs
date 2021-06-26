using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////////////////////////////////////
//
//  Script Name: PkayerUI.cs
//  Creator: James Bradbury
//  Description: A test script that expands the timer to render the ui for the player timer
//  
/////////////////////////////////////////////////////////////


public class PlayerUI : CountdownTimer
{

    [SerializeField] GameObject PotatoObject, PotatoMask;
    [SerializeField] float MaskSizeStart, MaskSizeEnd, vibrateMagnitude;
    
    SpriteRenderer PotatoSprite;
    RectTransform PotatoOrigin, MaskOrigin;
    Vector2 PotatoPosition;

    [SerializeField] Color colorStart, colorEnd;
    

    void Start() // Grabs the sprites for the potato and fuse 
    {
        PotatoSprite    = PotatoObject.GetComponent<SpriteRenderer>();
        PotatoOrigin    = PotatoObject.GetComponent<RectTransform>();
        MaskOrigin      = PotatoMask.GetComponent<RectTransform>();
        PotatoPosition  = PotatoOrigin.anchoredPosition;
    }

    void LateUpdate()
    {
        if(GetCurrentTime() > GetMinTime())
        {
            float LerpAmount = Mathf.InverseLerp(GetMaxTime(), GetMinTime(), GetCurrentTime());
            LerpAmount = LerpAmount * LerpAmount;
            LerpPotato(LerpAmount);
        }
        else
        {
            Debug.Log("Boom");
            Destroy(gameObject);
        }
    } // If the potato hasn't exploded, lerp the mask to make it look like the fuse is shortening

    void LerpPotato(float Step)
    {
        PotatoSprite.color = Color.Lerp(colorStart, colorEnd, Step);

        float MaskMagnitude = Mathf.Lerp(MaskSizeStart, MaskSizeEnd, Step);
        MaskOrigin.localScale = MaskMagnitude * Vector3.one;

        float OffsetMagnitude = vibrateMagnitude * Step;

        float DisplacementX =  (Time.deltaTime % 0.01f) * 100;
        float DisplacementY = (Time.deltaTime % 0.001f) * 1000;

        Vector2 Displacement = new Vector2(Mathf.Lerp(-OffsetMagnitude, OffsetMagnitude, DisplacementX), Mathf.Lerp(-OffsetMagnitude, OffsetMagnitude, DisplacementY));

        PotatoOrigin.anchoredPosition = Displacement + PotatoPosition;

    } // lerps the potato and shakes the potato around to make it appear animated


}
