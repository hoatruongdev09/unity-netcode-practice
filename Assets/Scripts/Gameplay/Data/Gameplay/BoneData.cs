using UnityEngine;

[System.Serializable]
public class BoneData
{
    public string name;
    public Transform bone;

    public BoneData(string name, Transform bone)
    {
        this.name = name;
        this.bone = bone;
    }
}