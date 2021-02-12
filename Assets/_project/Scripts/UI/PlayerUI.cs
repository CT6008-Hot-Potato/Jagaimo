using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] int Mode; // 0 = debug, 1 = 1st person, 2 = 2nd person

    [SerializeField] GameObject PotatoObject, PotatoMask;
    [SerializeField] float timerStart, timerEnd, timer, MaskSizeStart, MaskSizeEnd, vibrateMagnitude;
    
    SpriteRenderer PotatoSprite;
    RectTransform PotatoOrigin, MaskOrigin;
    Vector2 PotatoPosition;

    [SerializeField] Color colorStart, colorEnd;
    

    void Start()
    {
        timer           = timerStart;
        PotatoSprite    = PotatoObject.GetComponent<SpriteRenderer>();
        PotatoOrigin    = PotatoObject.GetComponent<RectTransform>();
        MaskOrigin      = PotatoMask.GetComponent<RectTransform>();
        PotatoPosition  = PotatoOrigin.anchoredPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(timer > timerEnd)
        {
            timer -= Time.deltaTime;

            float LerpAmount = Mathf.InverseLerp(timerStart, timerEnd, timer);

            LerpAmount = LerpAmount * LerpAmount;
            LerpPotato(LerpAmount);
        }
        else
        {
            Debug.Log("Boom");
            Destroy(gameObject);
        }

        
    }

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

    }


}
