using deVoid.Utils;
using UnityEngine;

public class TakeDamageSignal : ASignal<IDamageInfo> { }
public class CharacterDieSignal : ASignal<AttackableUnit, AttackableUnit> { }

public class RocketTriggerObject : ASignal<Rocket, InGameObject> { }
public class RocketCollisionEvent : ASignal<Rocket, Collision> { }