using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreGameStart : StateMachineBehaviour {

  
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //begin loading any and all files needed before switching to main menu

        //setup audio sources
        GameMaster gm = animator.gameObject.GetComponent<GameMaster>();
        
        gm.music = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
        gm.music = GameObject.Find("SFX").GetComponent<AudioSource>();
        gm.music = GameObject.Find("VoiceActing").GetComponent<AudioSource>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //check to see if loading is done, if so move on to main menu

        animator.SetTrigger("GotoMainMenu");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
