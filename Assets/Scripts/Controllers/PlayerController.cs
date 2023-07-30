using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseContoller
{
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    PlayerStat _stat;

    bool _stopSkill = false;


    public override void Init()
    {
        _stat = gameObject.GetComponent<PlayerStat>();
        Managers.Input.MouseAction += OnMouseEvent;

        if (gameObject.GetComponent<UI_HPBar>() == null)
            Managers.UI.Make3DUI<UI_HPBar>(transform);
    }

    protected override void UpdateSkill()
    {
        if(_target != null)
        {
            Vector3 dir = _target.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    protected override void UpdateMoving()
    {
        // 몬스터가 내 사정거리보다 가까우면 공격
        if(_target != null)
        {
            _destPos = _target.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if(distance < 1.5f)
            {
                State = Define.State.Skill;
                return;
            }
        }

        // 이동
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
            if(Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                if(Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;

                return;
            }

            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }


    void OnHitEvent()
    {
        Debug.Log("Hit!");

        if (_target != null)
        {
            Stat targetStat = _target.GetComponent<Stat>();
            PlayerStat myStat = gameObject.GetComponent<PlayerStat>();
            int damage = Mathf.Max(0, myStat.Attack - targetStat.Defence);
            targetStat.Hp -= damage;
        }

        if (_stopSkill)
        {
            State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Skill;
        }

    }


    void OnMouseEvent(Define.MouseEvent ev)
    {
        switch(State)
        {
            case Define.State.Idle:
                OnMouseEvent_IdleRun(ev);
                break;  
            case Define.State.Moving:
                OnMouseEvent_IdleRun(ev);
                break;
            case Define.State.Skill:
                {
                    if (ev == Define.MouseEvent.PointerUp)
                        _stopSkill = true;
                }
                break;
            case Define.State.Die:
                break;

            default:
                break;


        }
        
    }

    void OnMouseEvent_IdleRun(Define.MouseEvent ev)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);
        //Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        switch (ev)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if (raycastHit)
                    {
                        _destPos = hit.point;
                        State = Define.State.Moving;
                        _stopSkill = false;

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _target = hit.collider.gameObject;
                        else
                            _target = null;
                    }
                }
                break;
            case Define.MouseEvent.Press:
                {
                    if (_target == null && raycastHit)
                        _destPos = hit.point;
                }
                break;
            case Define.MouseEvent.PointerUp:
                _stopSkill = true;
                break;
            default:
                break;

        }
    }
}
