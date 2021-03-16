using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineShaderRenderer : MonoBehaviour
{

    [SerializeField] float scaleFactor;
    [SerializeField] Color Color;
    [SerializeField] Material outlineColor;
    private Renderer outlineRenderer;
    GameObject outlineObject;

    // Start is called before the first frame update
    void Start()
    {
        outlineRenderer = CreateOutline(outlineColor, scaleFactor, Color);
    //    outlineRenderer.enabled = true;
        Destroy(this);
    }

    Renderer CreateOutline(Material outlineMat, float scaleFactor, Color color)
    {

        outlineObject = new GameObject(gameObject.name + " outline");

        outlineObject.transform.parent = gameObject.transform;
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localEulerAngles = Vector3.zero;
        outlineObject.transform.localScale = Vector3.one ;   


        // outlineObject.transform.localScale = transform.localScale;
        MeshFilter mesh = outlineObject.AddComponent<MeshFilter>();
        mesh.mesh = GetComponent<MeshFilter>().mesh;

        MeshRenderer rend = outlineObject.AddComponent<MeshRenderer>();
        rend.material = outlineMat;
        rend.material.SetColor("Color_B76D8C5B", color);
        rend.material.SetFloat("Vector1_E75A2F97", scaleFactor );
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //outlineObject.GetComponent<OutlineShaderRenderer>().enabled = false;
        //outlineObject.GetComponent<Collider>().enabled = false;
        //rend.enabled = false;

        return rend;
    }

    private void Update()
    {
 //       outlineObject.transform.position = transform.position;
  //      outlineObject.transform.eulerAngles = transform.eulerAngles * 0.5f;
   //     outlineObject.transform.localScale = transform.localScale;

    }

    //private void OnMouseEnter()
    //{
    //    outlineRenderer.enabled = true;
    //}

    //private void OnMouseOver()
    //{
    //    transform.Rotate(Vector3.up, 1f, Space.World);
    //}

    //private void OnMouseExit()
    //{
    //    outlineRenderer.enabled = false;
    //}
}
