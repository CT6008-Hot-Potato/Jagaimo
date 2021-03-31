using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBlink: MonoBehaviour
{
    Camera MyCamera;
    Vector2 Constraints;
    [SerializeField] Vector2 TransformedValue;
    public float TransitionTime;
    float Speed;
    [SerializeField] Color BackgroundColor;
    


    private void OnEnable()
    {
        if (!TryGetComponent(out MyCamera))
            return;
        Constraints = MyCamera.rect.size;
        Speed = 1 / TransitionTime;

        GameObject backgroundObject = new GameObject(name + " " + "background");
        backgroundObject.transform.SetParent(transform);
        Camera backgroundCam   = backgroundObject.AddComponent<Camera>();
        backgroundCam.rect = MyCamera.rect;
        backgroundCam.cullingMask = 0;
        backgroundCam.clearFlags = CameraClearFlags.SolidColor;
        backgroundCam.backgroundColor = BackgroundColor;
        backgroundCam.depth = MyCamera.depth - 1.0f;


        OpenLens();
    }


    public void OpenLens()
    {
        StartCoroutine(LerpCameraBlink(TransformedValue, Constraints, 0));
    }

    public void CloseLens()
    {
        StartCoroutine(LerpCameraBlink(Constraints, TransformedValue, 0));
    }



    IEnumerator LerpCameraBlink(Vector2 Start, Vector2 End, float LerpValue)
    {

        LerpValue +=  Speed * Time.deltaTime;
        if (LerpValue > 1)
            LerpValue = 1;
        
        Vector2 LerpedValue = Vector2.Lerp(Start, End, LerpValue );


        MyCamera.rect = new Rect( (1 - LerpedValue.x) * 0.5f , (1 - LerpedValue.y) *0.5f , LerpedValue.x, LerpedValue.y);


        
        yield return null;

        if (LerpValue < 1)
            StartCoroutine(LerpCameraBlink(Start, End, LerpValue));
        
    }

}
