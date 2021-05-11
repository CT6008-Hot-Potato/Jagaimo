using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollText : MonoBehaviour
{
    RectTransform MyTransform;
    [SerializeField]
    Vector3 Change;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out MyTransform);   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (MyTransform == null)
            return;

        MyTransform.localPosition += Change * Time.deltaTime;
    }
}
