using System;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_ATOMS_GENERATE_DOCS && UNITY_EDITOR
using System.Collections.ObjectModel;
#endif

namespace UnityAtoms
{
    /// <summary>
    /// None generic base class for Events. Inherits from `BaseAtom` and `ISerializationCallbackReceiver`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    public abstract class AtomEventBase : BaseAtom, ISerializationCallbackReceiver
    {
#if !UNITY_ATOMS_GENERATE_DOCS && UNITY_EDITOR
        private static readonly Dictionary<int, ObservableCollection<Delegate>> s_actionsById = new();
        private static readonly Dictionary<int, ObservableCollection<Delegate>> s_typedActionsById = new();

        public static ObservableCollection<Delegate> GetActions(int id)
        {
            return GetOrCreateActions(s_actionsById, id);
        }

        public static ObservableCollection<Delegate> GetTypedActions(int id)
        {
            return GetOrCreateActions(s_typedActionsById, id);
        }

        private static ObservableCollection<Delegate> GetOrCreateActions(IDictionary<int, ObservableCollection<Delegate>> dict, int id)
        {
            if (!dict.ContainsKey(id))
            {
                dict.Add(id, new ObservableCollection<Delegate>());
            }
            else if (dict[id] == null)
            {
                dict[id] = new ObservableCollection<Delegate>();
            }
            return dict[id];
        }

        private static void UpdateActions(int instanceId, Action a)
        {
            var c = GetActions(instanceId);
            c.Clear();
            if (a == null) return;
            foreach (var d in a.GetInvocationList())
            {
                c.Add(d);
            }
        }

        protected static void UpdateTypedActions<T>(int instanceId, Action<T> a)
        {
            var c = GetTypedActions(instanceId);
            c.Clear();
            if (a == null) return;
            foreach (var d in a.GetInvocationList())
            {
                c.Add(d);
            }
        }
#endif
        
        /// <summary>
        /// Event without value.
        /// </summary>
        public event Action OnEventNoValue;

        public virtual void Raise()
        {
#if !UNITY_ATOMS_GENERATE_DOCS && UNITY_EDITOR
            StackTraces.AddStackTrace(GetInstanceID(), StackTraceEntry.Create());
#endif
            OnEventNoValue?.Invoke();
        }

        /// <summary>
        /// Register handler to be called when the Event triggers.
        /// </summary>
        /// <param name="del">The handler.</param>
        public void Register(Action del)
        {
            OnEventNoValue += del;
#if !UNITY_ATOMS_GENERATE_DOCS && UNITY_EDITOR
            UpdateActions(GetInstanceID(), OnEventNoValue);
#endif
        }

        /// <summary>
        /// Unregister handler that was registered using the `Register` method.
        /// </summary>
        /// <param name="del">The handler.</param>
        public void Unregister(Action del)
        {
            OnEventNoValue -= del;
#if !UNITY_ATOMS_GENERATE_DOCS && UNITY_EDITOR
            UpdateActions(GetInstanceID(), OnEventNoValue);
#endif
        }

        /// <summary>
        /// Unregister all handlers that were registered using the `Register` method.
        /// </summary>
        public virtual void UnregisterAll()
        {
            OnEventNoValue = null;
#if !UNITY_ATOMS_GENERATE_DOCS && UNITY_EDITOR
            UpdateActions(GetInstanceID(), OnEventNoValue);
#endif
        }

        /// <summary>
        /// Register a Listener that in turn trigger all its associated handlers when the Event triggers.
        /// </summary>
        /// <param name="listener">The Listener to register.</param>
        public void RegisterListener(IAtomListener listener)
        {
            OnEventNoValue += listener.OnEventRaised;
#if !UNITY_ATOMS_GENERATE_DOCS && UNITY_EDITOR
            UpdateActions(GetInstanceID(), OnEventNoValue);
#endif
        }

        /// <summary>
        /// Unregister a listener that was registered using the `RegisterListener` method.
        /// </summary>
        /// <param name="listener">The Listener to unregister.</param>
        public void UnregisterListener(IAtomListener listener)
        {
            OnEventNoValue -= listener.OnEventRaised;
#if !UNITY_ATOMS_GENERATE_DOCS && UNITY_EDITOR
            UpdateActions(GetInstanceID(), OnEventNoValue);
#endif
        }

        public void OnBeforeSerialize() { }

        public virtual void OnAfterDeserialize()
        {
            // Clear all delegates when exiting play mode
            if (OnEventNoValue != null)
            {
                foreach (var d in OnEventNoValue.GetInvocationList())
                {
                    OnEventNoValue -= (Action)d;
                }
#if !UNITY_ATOMS_GENERATE_DOCS && UNITY_EDITOR
                UpdateActions(GetInstanceID(), OnEventNoValue);
#endif
            }
        }

    }
}
