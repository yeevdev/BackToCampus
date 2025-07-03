using System.Collections.Generic;
using UnityEngine;
public class CollisionManager : MonoBehaviour
{
    private List<CollisionBox> collisionBoxes = new();

    public void Add(CollisionBox box)
    {
        collisionBoxes.Add(box);
        // for (int i = 0; i < collisionBoxes.Count; i++)
        // collisionBoxes[i].Log();
    }

    public void Remove(CollisionBox box)
    {
        collisionBoxes.Remove(box);
        // for (int i = 0; i < collisionBoxes.Count; i++)
        // collisionBoxes[i].Log();
    }
    
    public bool CheckCollision(CollisionBox box)
    {
        bool collision = false;
        for (int i = 0; i < collisionBoxes.Count; i++)
            collision |= box.DoesCollideWith(collisionBoxes[i]);
        return collision;
    }
}