////////////////////////////////////////////////////////////
// File: PotatoMagnetism.cs
// Author: Charles Carter
// Date Created: 20/05/21
// Brief: The script for the potato to steer towards the neareast untagged player when in range
////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using UnityEngine;

public class PotatoMagnetism : MonoBehaviour {
    #region Variables Needed

    private float MagnestismStrength;
    private float MagnestismDuration;

    private CharacterManager target;
    private Rigidbody _rb;

    private bool bCanHone = true;

    #endregion

    #region Unity Methods

    private void Awake() {
        _rb = _rb ?? transform.parent.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && other.TryGetComponent(out CharacterManager cManager)) {
            if (!cManager._tracker.isTagged && MagnestismStrength > 0 && MagnestismDuration > 0) {
                target = cManager;

                if (bCanHone) {
                    StartCoroutine(Co_HomePotato());
                }
            }
        }
    }

    #endregion

    #region Public Methods

    public void SetMagnetismStr(float newStr) {
        MagnestismStrength = newStr;
    }

    public void SetMagnetismDur(float newDur) {
        MagnestismDuration = newDur;
    }

    #endregion

    #region Private Methods

    private IEnumerator Co_HomePotato() {
        bCanHone = false;

        if (Debug.isDebugBuild) {
            Debug.Log("Potato homing onto player: " + _rb.velocity, this);
        }

        for (float t = 0; t < MagnestismDuration; t += Time.deltaTime) {
            //They were tagged by the potato
            if (target._tracker.isTagged) {
                StopCoroutine(Co_HomePotato());
                break;
            }

            Vector3 targetDir = (target.transform.position - transform.position).normalized;

            _rb.AddForce(targetDir * MagnestismStrength);

            yield return null;
        }

        bCanHone = true;
    }

    #endregion
}
