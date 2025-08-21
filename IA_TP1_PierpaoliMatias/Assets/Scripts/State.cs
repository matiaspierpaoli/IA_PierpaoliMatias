using System;

public abstract class State
{
    public Action<Enum> OnFlag;

    public virtual Type[] OnEnterParameterTypes => Array.Empty<Type>();
    public virtual Type[] OnTickParameterTypes => Array.Empty<Type>();
    public virtual Type[] OnExitParameterTypes => Array.Empty<Type>();

    public virtual BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        return null;
    }
    
    public virtual BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        return null;
    }
    
    public virtual BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return null;
    }

}

