using UnityEngine;
using Unity.Entities;

public class MoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Debug.Log("OnUpdate!!!");
        foreach (var e in GetEntities<MoveSystemComponents>())
        {
            Debug.Log(e);
        }
    }
}
