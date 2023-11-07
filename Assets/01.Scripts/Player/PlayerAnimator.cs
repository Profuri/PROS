using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : PlayerHandler
{
    private Animator _animator;

    // private readonly int _walkLeftHash = Animator.StringToHash("WalkLeft");
    // private readonly int _walkRighthash = Animator.StringToHash("WalkRight");
    // private readonly int _idleHash = Animator.StringToHash("Idle");
    // private readonly int _jumpHash = Animator.StringToHash("");
    // private readonly int _dashHash  = Animator.StringToHash("");


    //private void SetAnimHash
    public override void Init(PlayerBrain brain)
    {
        base.Init(brain);
        brain.transform.Find("Visual").GetComponent<Animator>();
    }


    public override void BrainFixedUpdate()
    {
        
    }

    public override void BrainUpdate()
    {

    }
}
