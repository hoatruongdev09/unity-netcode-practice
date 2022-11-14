using UnityEngine;

public abstract class ABoneManager : MonoBehaviour, IBoneManager
{
    public abstract Transform GetBoneByName(string name);
}