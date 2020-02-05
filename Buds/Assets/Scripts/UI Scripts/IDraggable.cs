using UnityEngine;


public interface IDraggable
{
    void Drop(GameObject onto);

    void Lift(GameObject from);
}
