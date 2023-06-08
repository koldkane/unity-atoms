#if UNITY_2019_1_OR_NEWER && !UNITY_ATOMS_GENERATE_DOCS
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace UnityAtoms.Editor
{
    public static class AtomRegisterEditor
    {
        public static VisualElement RenderRegister(VisualElement parent, int instanceId)
        {
            if (!AtomPreferences.IsDebugModeEnabled) return null;

            var actions = AtomEventBase.GetActions(instanceId);
            var typedActions = AtomEventBase.GetTypedActions(instanceId);

            var foldout = GetOrCreate<Foldout>(parent, "REGISTER_FOLDOUT", (element) =>
            {
                element.style.marginTop = 4;
                element.style.marginBottom = 4;
                element.text = "Register";
            });

            var wrapper = GetOrCreate<VisualElement>(foldout, "REGISTER_ROOT", (element) =>
            {
                element.style.flexDirection = FlexDirection.Column;
            });

            RenderRegisterOverview(wrapper, actions, typedActions);

            actions.CollectionChanged += (_, _) =>
            {
                RenderRegisterOverview(wrapper, actions, typedActions);
            };


            typedActions.CollectionChanged += (_, _) =>
            {
                RenderRegisterOverview(wrapper, actions, typedActions);
            };

            return wrapper;
        }
        
        private static void RenderRegisterOverview(VisualElement parent,
            ObservableCollection<Delegate> actions, ObservableCollection<Delegate> typedActions)
        {
            var stackTracesOverview = GetOrCreate<ScrollView>(parent, "REGISTER_OVERVIEW_SCROLL_VIEW", (scrollView) =>
            {
                scrollView.style.maxHeight = 100;
                scrollView.style.height = 100;
                scrollView.style.backgroundColor = GetBodyColor();
                scrollView.showVertical = true;
            });
            var overviewRowContainer = GetOrCreate<VisualElement>(stackTracesOverview, "REGISTER_OVERVIEW_ROW_CONTAINER");
            overviewRowContainer.style.flexDirection = FlexDirection.Column;

            overviewRowContainer.Clear();
            
            foreach (var d in actions)
            {
                AddRow(overviewRowContainer, d);
                
            }

            foreach (var d in typedActions)
            {
                AddRow(overviewRowContainer, d);
            }
        }
        
        private static void AddRow(VisualElement container, Delegate d)
        {
            var info = d.GetMethodInfo();
            var hostType = d.GetMethodInfo().DeclaringType;
            var isObject = d.Target is UnityEngine.Object;
            var o = d.Target as UnityEngine.Object;
            var row = new Label()
            {
                text = isObject ? $"{info} in {o.name} of {hostType}" : $"{info} of {hostType}",
                style =
                {
                    paddingTop = 4,
                    paddingBottom = 4,
                    paddingLeft = 4,
                    paddingRight = 4,
                }
            };

            if (isObject)
            {
                row.RegisterCallback<MouseDownEvent>((_) =>
                {
                    Selection.activeObject = o;
                });
            }
            
            container.Add(row);
        }

        private static T GetOrCreate<T>(VisualElement parent, string name, Action<T> initializer = null) where T : VisualElement, new()
        {
            var element = (T)parent.Query<VisualElement>(name: name).First() ?? new T() { name = name };
            if (initializer != null)
            {
                initializer(element);
            }
            if (!parent.Contains(element))
                parent.Add(element);
            return element;
        }
        
        private static Color GetBodyColor()
        {
            var proColor = new Color(83f / 255f, 83f / 255f, 83f / 255f);
            var basicColor = new Color(174f / 255f, 174f / 255f, 174f / 255f);
            return EditorGUIUtility.isProSkin ? proColor : basicColor;
        }
    }
}
#endif
