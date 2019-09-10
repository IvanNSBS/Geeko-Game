using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public enum MinotaurState
{
    Normal,
    Stressed,
    Rage
}

public enum MinotaurAttack
{
    Poke,
    Axe,
    Spin,
    Dash,
    FloorHit,
}
public class MinotaurController : EnemyController
{
    public Animator minotaurAnimator;
    public float rage = 0;
    public MinotaurState minotaurState = MinotaurState.Normal;
    public MinotaurAttack[] attacks = {MinotaurAttack.Poke,MinotaurAttack.Axe,MinotaurAttack.Spin,
                                        MinotaurAttack.Dash,MinotaurAttack.FloorHit};
    private bool _attacking=false;
    private MinotaurAttack _curAttack;

    public MinotaurAttack ChooseAttack()
    {
        int lottery = 0;
        int poke=0;
        int axe=0;
        int spin=0;
        int dash=0;
        
        switch (minotaurState)
        {
            case(MinotaurState.Normal):
                poke = 50; 
                axe = 100;
                break;
            case(MinotaurState.Stressed):
                poke = 25;
                axe = 50;
                spin = 100;
                break;
            case(MinotaurState.Rage):
                poke = 10;
                axe = 20;
                spin = 50;
                dash = 100;
                break;
        }
        
        lottery = Random.Range(0, 100);

        if (lottery < poke)
        {
            return attacks[0];
        } 
        else if (lottery < axe)
        {
            return attacks[1];
        }
        else if (lottery < spin)
        {
            return attacks[2];
        }else if (lottery < dash)
        {
            return attacks[3];
        }
        Debug.Log("Bug in the lottery, number not in the range expected");
        return attacks[lottery];
    }
    
    public override void MeleeAttacks()
    {
        if (!_attacking)
        {
            _curAttack = ChooseAttack();
            _attacking = true;
        }
        else
        {
            switch (_curAttack)
            {
                case(MinotaurAttack.Poke):
                    Poke();
                    break;
                case(MinotaurAttack.Axe):
                    AxeAttack();
                    break;
                case(MinotaurAttack.FloorHit):
                    FloorHit();
                    break;
                case(MinotaurAttack.Spin):
                    Spin();
                    break;
                case(MinotaurAttack.Dash):
                    Dash();
                    break;
            }
        }
    }

    private void AxeAttack()
    {
        throw new System.NotImplementedException();
    }

    private void Spin()
    {
        throw new System.NotImplementedException();
    }

    private void FloorHit()
    {
        throw new System.NotImplementedException();
    }

    private void Poke()
    {
        throw new System.NotImplementedException();
    }
    
    public void updateRage()
    {
        rage = GetCurrentHealth() / getMaximumHealth();
    }
}
