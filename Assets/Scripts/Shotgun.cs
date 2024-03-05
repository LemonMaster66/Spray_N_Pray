using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    public override void ShootStart()
    {
        base.ShootStart();
        Debug.Log("Unique Shotgun Code");
    }

}
