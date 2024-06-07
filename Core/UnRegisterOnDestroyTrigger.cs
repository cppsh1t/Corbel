using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Corbel.Extension
{

public class UnRegisterOnDestroyTrigger : MonoBehaviour
{
    private readonly HashSet<IUnRegister> mUnRegisters = new();

    public void AddUnRegister(IUnRegister unRegister)
    {
        mUnRegisters.Add(unRegister);
    }

    public void RemoveUnRegister(IUnRegister unRegister)
    {
        mUnRegisters.Remove(unRegister);
    }

    private void OnDestroy()
    {
        foreach (var unRegister in mUnRegisters)
        {
            unRegister.UnRegister();
        }

        mUnRegisters.Clear();
    }
}

}
