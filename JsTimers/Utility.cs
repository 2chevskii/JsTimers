using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JsTimers
{
    static class Utility
    {
        const BindingFlags DEFAULT_BINDING_FLAGS  = BindingFlags.Public | BindingFlags.NonPublic;
        const BindingFlags STATIC_BINDING_FLAGS   = BindingFlags.Static | DEFAULT_BINDING_FLAGS;
        const BindingFlags INSTANCE_BINDING_FLAGS = BindingFlags.Instance | DEFAULT_BINDING_FLAGS;

        public static MethodInfo GetStaticMethod(Type type, string methodName)
        {
            return GetMethod(type, methodName, STATIC_BINDING_FLAGS);
        }

        public static MethodInfo GetInstanceMethod(object instance, string methodName)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance), "Object instance cannot be null");
            }

            return GetMethod(instance.GetType(), methodName, INSTANCE_BINDING_FLAGS);
        }

        public static long MillisecondsToTicks(int milliseconds)
        {
            return milliseconds * TimeSpan.TicksPerMillisecond;
        }

        public static int SecondsToMilliseconds(float seconds)
        {
            return (int)(seconds * 1000f);
        }

        public static Action BuildCallbackFromInfo(MethodInfo methodInfo, object obj, object[] args)
        {
            if (methodInfo.IsStatic)
            {
                return () => {
                    methodInfo.Invoke(null, args);
                };
            }

            return () => {
                methodInfo.Invoke(obj, args);
            };
        }

        static MethodInfo GetMethod(Type type, string methodName, BindingFlags bindingFlags)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentException($"'{methodName ?? "null"}' is not a valid method identifier");
            }

            var methodInfo = type.GetMethod(methodName, bindingFlags);

            if (methodInfo is null)
            {
                throw new InvalidOperationException($"Method '{methodName}' was not found in type {type.Name}");
            }

            return methodInfo;
        }
    }
}
