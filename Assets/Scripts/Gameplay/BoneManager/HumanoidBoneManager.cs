using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidBoneManager : ManualBoneManager
{
    private Dictionary<string, BoneData> bones = new Dictionary<string, BoneData>();
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null) { return; }
        foreach (var boneName in Enum.GetValues(typeof(HumanBodyBones)))
        {
            if ((HumanBodyBones)boneName == HumanBodyBones.LastBone) { continue; }
            bones.Add(boneName.ToString(), new BoneData(boneName.ToString(), animator.GetBoneTransform((HumanBodyBones)boneName)));
        }
    }


    public override Transform GetBoneByName(string name)
    {
        var availableBone = base.GetBoneByName(name);
        if (availableBone) { return availableBone; }
        if (bones.TryGetValue(name, out var bone))
        {
            return bone.bone;
        }
        return null;
    }
}