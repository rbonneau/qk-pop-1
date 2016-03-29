using System;
using UnityEngine;
using Debug = FFP.Debug;

class Hideout : MonoBehaviour
{
    Collider OuterCollider;
    Collider HidingTrigger;
    Rigidbody RBody;

    void Start()
    {
        OuterCollider = GetComponentInParent<Collider>();
        if(OuterCollider == null)
        {
            Debug.Warning("level", "Hiding Place " + gameObject.name + " has no outer collider.");
        }

        HidingTrigger = GetComponent<Collider>();
        if (HidingTrigger == null)
        {
            Debug.Warning("level", "Hiding Place " + gameObject.name + " has no inner trigger.");
        }
        else
        {
            if (!HidingTrigger.isTrigger)
            {
                HidingTrigger.isTrigger = true;
            }

            RBody = GetComponent<Rigidbody>();
            if (RBody == null)
            {
                RBody = gameObject.AddComponent<Rigidbody>();
            }
            RBody.isKinematic = true;
            RBody.useGravity = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        QK_Character_Movement player = QK_Character_Movement.Instance.GetComponentInHeirarchy<QK_Character_Movement>(other.gameObject);
        if(player != null)
        {
            QK_Character_Movement.Instance.isHidden = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        QK_Character_Movement player = QK_Character_Movement.Instance.GetComponentInHeirarchy<QK_Character_Movement>(other.gameObject);
        if (player != null)
        {
            QK_Character_Movement.Instance.isHidden = false;
        }
    }
}
