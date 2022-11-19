using System;
using UnityEngine;
using deVoid.Utils;

public static class GameHelper
{
    public const float ISOMETRIC_ANGLE = 45f;
    public const bool USE_ISOMETRIC_VIEW = true;
    public const int FREE_CLAN_ID = 0;
    public const int UNSET_ID = -1;
    public static readonly Vector3 UNSET_VECTOR_3 = new Vector3(-404, 404, -404);

    public static Vector3 ModifyDirectByCurrentView(Vector3 input)
    {
        if (!USE_ISOMETRIC_VIEW)
        {
            return input;
        }
        return ConvertToIsometricDirect(input);
    }
    public static Vector3 ConvertToIsometricDirect(Vector3 input)
    {
        return Quaternion.AngleAxis(ISOMETRIC_ANGLE, Vector3.up) * input;
    }
    public static float Angle(Vector3 input)
    {
        return Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg;
    }
    public static Vector3 InputDirectToWorldDirect(Vector2 input)
    {
        return new Vector3(input.x, 0, input.y);
    }
    public static Quaternion RotationFromDirection(Vector3 direction)
    {
        return Quaternion.Euler(0, Angle(direction), 0);
    }
    public static bool Chance(float percent)
    {
        return PsudoRandom() * 100f <= percent;
    }
    public static float PsudoRandom()
    {
        UnityEngine.Random.InitState(Environment.TickCount);
        return UnityEngine.Random.value;
    }

    public static ISpell CreateSpell(ISpellData spellData)
    {
        return new Spell(spellData, null);
    }

    public static float GetSqrDistance(Vector3 pos1, Vector3 pos2)
    {
        return (pos1 - pos2).sqrMagnitude;
    }

    public static void TriggerDamageSignal(IDamageInfo damageInfo)
    {
        Debug.Log($"trigger event damage signal");
        Signals.Get<TakeDamageSignal>().Dispatch(damageInfo);
    }

    public static void TriggerCharacterDieSignal(AttackableUnit killer, AttackableUnit victim)
    {
        Signals.Get<CharacterDieSignal>().Dispatch(killer, victim);
    }

    public static long GeneratePacketId()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds();
    }
}