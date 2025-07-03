using System.Collections.Generic;
using UnityEngine;
public class CollisionManager : MonoBehaviour
{
    private List<CollisionBox> collisionBoxes = new();

    public void Add(CollisionBox box)
    {
        collisionBoxes.Add(box);
    }

    public void Remove(CollisionBox box)
    {
        collisionBoxes.Remove(box);
    }

    public bool CheckCollision(CollisionBox box)
    {   // box와 다른 모든 CollisionBox에 대해서 충돌하는지 검사 
        bool collision = false;
        for (int i = 0; i < collisionBoxes.Count; i++)
        {
            if (box == collisionBoxes[i])
            {
                continue;
            }
            if (box.DoesCollideWith(collisionBoxes[i]))
            {
                collision = true;
                break;
            }
        }
        return collision;
    }
}