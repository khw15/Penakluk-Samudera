using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    [SerializeField]
    private float MoveSpeed = 1f;

    [SerializeField]
    private float Offset;

    private Vector2 StartPosition;

    private float NewXPosition;

    void Start()
    {
        StartPosition = transform.position;
    }

    void Update()
    {
        NewXPosition = Mathf.Repeat(Time.time * -MoveSpeed, Offset);

        transform.position = StartPosition + Vector2.right * NewXPosition;
    }
}
