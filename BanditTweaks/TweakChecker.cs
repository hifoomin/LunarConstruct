using System;

namespace HBT
{
    public abstract class TweakBase<T> : TweakBase where T : TweakBase<T>
    {
        public static T instance { get; set; }

        public TweakBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}