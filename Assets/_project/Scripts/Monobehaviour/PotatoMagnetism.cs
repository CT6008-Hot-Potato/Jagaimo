using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoMagnetism : MonoBehaviour
{
    #region Variables Needed

    private float MagnestismStrength;
    private float MagnestismDuration;

    Transform target;
    Rigidbody _rb;

    #endregion

    private void Awake()
    {
        _rb = _rb ?? transform.parent.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && other.TryGetComponent(out CharacterManager cManager))
        {
            if (!cManager._tracker.isTagged && MagnestismStrength > 0 && MagnestismDuration > 0)
            {
                target = other.transform;
                StartCoroutine(Co_HomePotato());
            }
        }
    }

    public void SetMagnetismStr(float newStr)
    {
        MagnestismStrength = newStr;
    }

    public void SetMagnetismDur(float newDur)
    {
        MagnestismStrength = newDur;
    }

    private IEnumerator Co_HomePotato()
    {
        for (float t = 0; t < MagnestismDuration; t += Time.deltaTime)
        {
            Vector3 targetDir = (target.position - transform.position).normalized;

            _rb.AddForce(targetDir * MagnestismStrength);

            yield return null;
        }
    }
}
