using UnityEngine;

public abstract class MovementController : MonoBehaviour
{
    public virtual void SetInputs(bool[] inputs, Quaternion playerRotation) { }
}
