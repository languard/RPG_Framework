using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreMainMenu : StateMachineBehaviour {

    bool doOnce = true;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Ensure all other scenese are unloaded, and load the MainMenu        
        //for(int i=SceneManager.sceneCount - 1; i>0; i++)
        while(SceneManager.sceneCount > 1)
        {
            if (SceneManager.GetSceneAt(0).name == "Core") continue;
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
        }
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(doOnce)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));
            doOnce = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //remove menu from scenes
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
