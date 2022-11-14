using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ManualBoneManager : ABoneManager, IBoneManager
{
    [SerializeField] private List<BoneData> externalBones;
    public override Transform GetBoneByName(string name)
    {
        var bone = externalBones.Find(bone => bone.name == name);
        return bone?.bone ?? null;
    }
}