using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MovementBehaviour
{
    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public override float MovementSpeed
    {
        get
        {
            return rb2d.linearVelocity.magnitude;
        }
        set
        {
            rb2d.linearVelocity.Normalize();
            rb2d.linearVelocity *= value;
        }
    }
}
