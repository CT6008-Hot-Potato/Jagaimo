////////////////////////////////////////////////////////////
// File: OutlineShaderRenderer
// Author: James Bradbury
// Brief: A test script for testing menu functions
//////////////////////////////////////////////////////////// 

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


    void Start()
    {
        outlineRenderer = CreateOutline(outlineColor, scaleFactor, Color);
        Destroy(this);
    } // As the game starts, create the outline

    Renderer CreateOutline(Material outlineMat, float scaleFactor, Color color)
    {

        outlineObject = new GameObject(gameObject.name + " outline");

        outlineObject.transform.parent = gameObject.transform;
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localEulerAngles = Vector3.zero;
        outlineObject.transform.localScale = Vector3.one ;   

        MeshFilter mesh = outlineObject.AddComponent<MeshFilter>();
        if (GetComponent<MeshFilter>() != null)
        {
            mesh.mesh = GetComponent<MeshFilter>().mesh;
        }
        else if (GetComponent<SkinnedMeshRenderer>() != null)
        {
            mesh.mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        Renderer rend = outlineObject.AddComponent<MeshRenderer>();
        rend.material = outlineMat;
        rend.material.SetColor("Color_B76D8C5B", color);
        rend.material.SetFloat("Vector1_E75A2F97", scaleFactor );
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
     
        return rend;
    } //  Creates an object slightly bigger than the original object, and gives it the outlinematerial- which renders behind the original object

}
