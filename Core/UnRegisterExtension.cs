using System.Collections.Generic;
using UnityEngine;

namespace Corbel.Extension
{

public static class UnRegisterExtension
{
    public static IUnRegister UnRegisterOntDestroy(this IUnRegister unRegister, GameObject gameObject)
    {
        var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

        if (!trigger)
        {
            trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
        }

        trigger.AddUnRegister(unRegister);

        return unRegister;
    }

    public static IUnRegister UnRegisterOntDestroy<T>(this IUnRegister self, T component)
        where T : Component
    {
        return self.UnRegisterOntDestroy(component.gameObject);
    }

    public static IUnRegister UnRegisterOnDisable(this IUnRegister unRegister, GameObject gameObject)
    {
        var trigger = gameObject.GetComponent<UnRegisterOnDisableTrigger>();

        if (!trigger)
        {
            trigger = gameObject.AddComponent<UnRegisterOnDisableTrigger>();
        }

        trigger.AddUnRegister(unRegister);

        return unRegister;
    }

    public static IUnRegister UnRegisterOnDisable<T>(this IUnRegister self, T component)
        where T : Component
    {
        return self.UnRegisterOnDisable(component.gameObject);
    }
}
}

