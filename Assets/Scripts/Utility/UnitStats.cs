using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats
{
    private static float speed = 5;
    public static float TailGatorSpeed { get { return speed * 0.5f; } }
    public static float SeekerHawkSpeed { get { return speed * 0.2f; } }
    public static float HiveRaptorNormalSpeed { get { return speed * 1.3f; } }
    public static float HiveRaptorChaseSpeed { get { return speed * 3.5f; } }
    public static float MirrageMantaSpeed { get { return speed; } }
    public static float BlinkWolfNormalSpeed { get { return speed * 1.5f; } }
    public static float BlinkWolfChaseSpeed { get { return speed * 2.8f; } }
    public static float WebWeaverSpeed { get { return speed; } }
    public static float HomingMissileSpeed { get { return speed * 2.5f; } }
    public static float BulletSpeed { get { return speed * 2f; } }
}
