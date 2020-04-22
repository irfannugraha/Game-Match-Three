using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileEvent : MonoBehaviour
{
    public abstract void On_Match();
    public abstract bool Achievement_Completed();

}
