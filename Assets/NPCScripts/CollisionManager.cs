using System.Collections.Generic;
using UnityEngine;
public class CollisionManager : MonoBehaviour
{
    private List<CollisionBox> collisionBoxes = new();

    public bool CheckCollision(CollisionBox box)
    {
        bool collision = false;
        for (int i = 0; i < collisionBoxes.Count; i++)
            collision |= box.DoesCollideWith(collisionBoxes[i]);
        return collision;
    }
}