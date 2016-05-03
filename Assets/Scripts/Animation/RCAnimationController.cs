using UnityEngine;
using System.Collections;
using Debug = FFP.Debug;

/*
 * Redcoat animation controller
 */

public class RCAnimationController : MonoBehaviour
{
    Animator animController;
    AnimatorControllerParameter[] animParams;
    StatePatternEnemy redcoat;
    IEnemyState state;
    Rigidbody rb;


    void Start()
    {
        if(GetComponent<StatePatternEnemy>() == null)
        {
            Debug.Warning("level", "Redcoat " + gameObject.name + " has no state pattern to animate on.");
            return;
        }

        animController = GetComponent<Animator>();
        animParams = animController.parameters;
        redcoat = GetComponent<StatePatternEnemy>();
        state = redcoat.currentState;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (redcoat != null)
        {
            DetermineAnimatorParams();
        }
    }

    void DetermineAnimatorParams()
    {
        state = redcoat.currentState;
        if (state is PatrolState && redcoat.idle == false || state is WalkState && redcoat.idle == false)
        {
            if (redcoat.navMeshAgent.speed > 0)
            {
                animController.SetBool("Walk", true);
                SetAllBoolParams("Walk", false);
            }
            else
            {
                animController.SetBool("Idle", true);
                SetAllBoolParams("Idle", false);
            }
        }
        else if (state is SearchingState)
        {
            if (redcoat.navMeshAgent.speed > 0)
            {
                animController.SetBool("Chase", true);
                SetAllBoolParams("Chase", false);
            }
            else
            {
                animController.SetBool("Turn", true);
                SetAllBoolParams("Turn", false);
            }
        }
        else if (state is ChaseState)
        {
            animController.SetBool("Chase", true);
            SetAllBoolParams("Chase", false);
        }
        else
        {
            animController.SetBool("Idle", true);
            SetAllBoolParams("Idle", false);
        }
    }

    void SetAllBoolParams(string exempt, bool flag)
    {
        foreach(AnimatorControllerParameter param in animParams)
        {
            if(param.type == AnimatorControllerParameterType.Bool && param.name != exempt)
            {
                animController.SetBool(param.name, flag);
            }
        }
    }
}
