using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseContoller
{
    Stat _stat;

    [SerializeField]
    float _scanRange = 10;
    [SerializeField]
    float _attackRange = 2;

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;
        _stat = gameObject.GetComponent<Stat>();

        if(gameObject.GetComponent<UI_HPBar>() == null )
            Managers.UI.Make3DUI<UI_HPBar>(transform);
    }

    protected override void UpdateIdle()
    {
        GameObject player = Managers.Game.GetPlayer();
        if (player == null)
            return;

        float distance = (player.transform.position - transform.position).magnitude;
        if(distance <= _scanRange )
        {
            _target = player;
            State = Define.State.Moving;
            return;
        }
    }

    protected override void UpdateMoving()
    {
        // �÷��̾ �� �����Ÿ����� ������ ����
        if (_target != null)
        {
            _destPos = _target.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance < _attackRange)
            {
                var nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);
                State = Define.State.Skill;
                return;
            }
        }

        // �̵�
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            var nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);
            nma.speed = _stat.MoveSpeed;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }
    protected override void UpdateSkill()
    {
        if (_target != null)
        {
            Vector3 dir = _target.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    void OnHitEvent()
    {

        if(_target != null )
        {
            // ü��
            Stat targetStat = _target.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);

            if(targetStat.Hp > 0)
            {
                float distance = (_target.transform.position - transform.position).magnitude;
                if (distance <= _attackRange)
                    State = Define.State.Skill;
                else
                    State = Define.State.Moving;
            }
            else
            {
                State = Define.State.Idle;
            }
        }
        else
        {
            State = Define.State.Idle;
        }
    }

}
