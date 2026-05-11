using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    //upDown 遷移中の上下の向き
    private float upDown = 0;
    private bool _isDrifting = false;

    public bool IsDrifting => _isDrifting;

    public void UpdateAnimation(float horizontal, float speed, float aitTime, float verticalVelocity, bool isFlying, bool isGliding, float upDownValue)
    {
        float setHorizontal = horizontal;
        //ドリフト中の左右の動き
        if (_isDrifting)
        {
            //setHorizontal = 
        }
        _animator.SetFloat("Horizontal", horizontal);
        _animator.SetFloat("Speed", speed);
        if (upDown < upDownValue)
        {
            upDown += Time.deltaTime * 2f;
            if (upDown > upDownValue)
            {
                upDown = upDownValue;
            }
        }
        else
        {
            upDown -= Time.deltaTime * 2f;
            if (upDown < upDownValue)
            {
                upDown = upDownValue;
            }
        }
        _animator.SetFloat("UpDown", upDown);
        if (isFlying)
        {
            _animator.SetBool("Fly", true);
            _animator.SetFloat("Horizontal", horizontal);
            _animator.SetFloat("Speed", 1);
            _animator.SetBool("Fall", false);
            _animator.SetBool("LeftDrift", false);
            _animator.SetBool("RightDrift", false);
            return;
        }
        else if (aitTime <= 0.2f && !isGliding)
        {
            _animator.SetBool("Fly", false);
            _animator.SetBool("Fall", false);
            return;
        }
        else if (verticalVelocity > 0)
        {
            _animator.SetBool("Fly", true);
            _animator.SetBool("Fall", false);
        }
        else
        {
            _animator.SetBool("Fly", false);
            _animator.SetBool("Fall", true);
        }
        if (isGliding)
        {
            _animator.SetBool("Drift", false);
        }
    }

    public void PlayRightDriftStart()
    {
        _animator.SetBool("RightDrift", true);
        _isDrifting = true;
    }
    public void PlayLeftDriftStart()
    {
        _animator.SetBool("LeftDrift", true);
        _isDrifting = true;
    }
    public void PlayNonDriftStart()
    {
        _animator.SetBool("NonDrift", true);
    }

    public void StopDrift()
    {
        _animator.SetBool("RightDrift", false);
        _animator.SetBool("LeftDrift", false);
        _animator.SetBool("NonDrift", false);
        _isDrifting = false;
    }

    public void PlayFireballDamage()
    {
        _animator.SetTrigger("FireballDamage");
        StopDrift();
    }
    public void PlayDamage()
    {
        _animator.SetTrigger("Damage");
        StopDrift();
    }

    public void PlayNeckBending()
    {
        _animator.SetBool("EnemyNear", true);
    }

    public void StopNeckBending()
    {
        _animator.SetBool("EnemyNear", false);
    }

    public void EnemyPosition(float direction)
    {
        _animator.SetFloat("NeckAngle", direction);
    }

    public void StartHappy()
    {
        _animator.SetTrigger("Happy");
    }

    public void AirDashAnimation()
    {
        _animator.SetTrigger("AirDash");
    }

    public void StartSpin()
    {
        _animator.SetTrigger("Spin");
        StopDrift();
    }

    public void FireballShoot()
    {
        _animator.SetTrigger("AttackFireball");
    }
}
