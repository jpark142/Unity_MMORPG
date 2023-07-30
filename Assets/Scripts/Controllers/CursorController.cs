using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    Texture2D _attackCursor;
    Texture2D _handCursor;

    enum CursorType
    {
        None,
        Attack,
        Hand
    }

    CursorType _cursorType = CursorType.None;

    void Start()
    {
        _attackCursor = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
        _handCursor = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");

    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, _mask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(_attackCursor, new Vector2(_attackCursor.width / 5, 0), CursorMode.Auto);
                    _cursorType = CursorType.Attack;
                }
            }
            else
            {
                if (_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_handCursor, new Vector2(_handCursor.width / 3, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }

            }
        }
    }
}
