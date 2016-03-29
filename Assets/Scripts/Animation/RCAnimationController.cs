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
        if (redcoat.moveSpeed > 0)
        {
            animController.SetBool("Walk", true);
            SetAllBoolParams("Walk", false);
        }
        else if(state is SearchingState)
        {
            animController.SetBool("Turn", true);
            SetAllBoolParams("Turn", false);
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
