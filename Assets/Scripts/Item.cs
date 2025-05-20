using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] ItemSO data;

    public int GetPoint()
    {
        return data.point;
    }
}
