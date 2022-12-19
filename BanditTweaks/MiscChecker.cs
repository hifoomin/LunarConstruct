using System;

namespace HBT
{
    public abstract class MiscBase<T> : MiscBase where T : MiscBase<T>
    {
        public static T instance { get; set; }

        public MiscBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}