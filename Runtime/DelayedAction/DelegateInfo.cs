using System;
using UnityEngine;

namespace GameFramework
{
    public struct DelegateInfo : IComparable<DelegateInfo>
    {
        private static int autoId;

        private Delegate action;
        private object[] target;

        public DelegateInfo(Delegate action, float invocationTime, params object[] param)
        {
            Id = autoId++;
            this.action = action;
            InvocationTime = invocationTime;
            target = param;
        }

        public int Id { get; private set; }

        public float InvocationTime { get; private set; }

        public void Invoke()
        {
            try
            {
                action.DynamicInvoke(target);
            }
            catch (Exception ex)
            {
                GameLogger.LogError($"DynamicInvoke is thrown exception : {ex} {this}");
            }
        }

        public int CompareTo(DelegateInfo other)
        {
            return InvocationTime.CompareTo(other.InvocationTime);
        }

        public override string ToString()
        {
            if (action == null || action.Method.DeclaringType == null)
            {
                return $"Null delegate for {Id}";
            }

            string message = $"{Id} call after {InvocationTime - Time.unscaledTime:F1} {action.Method.DeclaringType.Name}.{action.Method.Name}(";
            string step = "";
            foreach (object obj in target)
            {
                message += step + obj;
                step = ", ";
            }

            message += ")";
            return message;
        }
    }
}