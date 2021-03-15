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
        outlineRenderer.enabled = true;
    }

    Renderer CreateOutline(Material outlineMat, float scaleFactor, Color color)
    {

        outlineObject = Instantiate(gameObject, transform.position, transform.rotation);
        outlineObject.transform.localScale = transform.localScale;

        Renderer rend = outlineObject.GetComponent<Renderer>();
        rend.material = outlineMat;
        rend.material.SetColor("Color_B76D8C5B", color);
        rend.material.SetFloat("Vector1_E75A2F97", scaleFactor );
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        outlineObject.GetComponent<OutlineShaderRenderer>().enabled = false;
        outlineObject.GetComponent<Collider>().enabled = false;
        rend.enabled = false;

        return rend;
    }

    private void Update()
    {
        outlineObject.transform.position = transform.position;
        outlineObject.transform.eulerAngles = transform.eulerAngles * 0.5f;
        outlineObject.transform.localScale = transform.localScale;

    }

    //private void OnMouseEnter()
    //{
    //    outlineRenderer.enabled = true;
    //}

    private void OnMouseOver()
    {
        transform.Rotate(Vector3.up, 1f, Space.World);
    }

    //private void OnMouseExit()
    //{
    //    outlineRenderer.enabled = false;
    //}
}
