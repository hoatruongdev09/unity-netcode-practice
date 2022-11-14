public class ObserverHub
{
    public static OnTakeDamageEvent takeDamageEvent;
}

public delegate void OnTakeDamageEvent(IDamageInfo damageInfo);