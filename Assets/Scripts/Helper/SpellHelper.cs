public static class SpellHelper
{
    public static bool IsValidTarget(AttackableUnit attacker, AttackableUnit target, TargetFilter filter)
    {
        if (target.IsDead && !filter.HasFlag(TargetFilter.Dead)) { return false; }
        if (target == attacker && !filter.HasFlag(TargetFilter.Self)) { return false; }

        if (target != attacker && filter == TargetFilter.Self) { return false; }

        if (target != attacker && (target.ClanID != attacker.ClanID || attacker.ClanID == GameHelper.FREE_CLAN_ID) && !filter.HasFlag(TargetFilter.Enemy))
        {
            return false;
        }
        if (target != attacker && (target.ClanID == attacker.ClanID && target.ClanID != GameHelper.FREE_CLAN_ID) && !filter.HasFlag(TargetFilter.Ally))
        {
            return false;
        }
        if (target.Team == TeamId.Neutral && attacker.Team != TeamId.Neutral && !filter.HasFlag(TargetFilter.Neutral))
        {
            return false;
        }

        return true;
    }
}