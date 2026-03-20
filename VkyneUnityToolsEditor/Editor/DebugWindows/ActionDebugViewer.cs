using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System;

namespace VkyneTools.Debug
{
    public class ActionDebugViewer : EditorWindow
    {
        GameObject _subject;
        List<MonoBehaviour> _subjectBehaviors = new List<MonoBehaviour>();

        int _monoBehaviourSelection;
        int _depth;

        List<int> _fieldSelections = new List<int>();

        Vector2 _scrollLocation = new Vector2(0, 0);

        // Add menu named "My Window" to the Window menu
        [MenuItem("Debug/Event Invoke Debug")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            ActionDebugViewer window = (ActionDebugViewer)EditorWindow.GetWindow(typeof(ActionDebugViewer), false, "Event Invoke Debug Viewer");
            window.Show();
        }

        void OnGUI()
        {

            _subject = (GameObject)EditorGUILayout.ObjectField(_subject, typeof(GameObject), true);

            //Instance selection dropdown

            if (_subject != null)
            {

                _subjectBehaviors = _subject.GetComponents<MonoBehaviour>().ToList();
                if (_subjectBehaviors.Any())
                {
                    _monoBehaviourSelection = EditorGUILayout.Popup("Select a MonoBehaviour:", _monoBehaviourSelection, _subjectBehaviors.Select(x => x.GetType().ToString()).ToArray());

                    if (_monoBehaviourSelection >= _subjectBehaviors.Count)
                    {
                        _monoBehaviourSelection = _subjectBehaviors.Count - 1;
                    }

                    _depth = -1;
                    GetRecursiveFields(_subjectBehaviors[_monoBehaviourSelection]);
                }

            }
            else
            {
                _monoBehaviourSelection = 0;
            }
        }

        public void GetRecursiveFields(object subject)
        {
            if (subject == null)
            {
                EditorGUILayout.LabelField("Field hasn't been instatiated. Start Game to View.", EditorStyles.largeLabel);
                return;
            }
            _depth++;
            if (_depth == _fieldSelections.Count)
            {
                _fieldSelections.Add(0);
            }

            FieldInfo[] chosenFields = subject.GetType().GetFields(BindingFlags.Public |
                                                                   BindingFlags.NonPublic |
                                                                   BindingFlags.Instance |
                                                                   BindingFlags.GetField);
            Type subjectType = subject.GetType();
            object baseSubject = Convert.ChangeType(subject, subjectType);

            if (chosenFields.Any())
            {
                List<string> fieldNames = new List<string>() { "Base" };
                fieldNames.AddRange(chosenFields.Select(x => x.Name));
                _fieldSelections[_depth] = EditorGUILayout.Popup("Select a Field:", _fieldSelections[_depth], fieldNames.ToArray());

                if (_fieldSelections[_depth] >= fieldNames.Count)
                {
                    _fieldSelections[_depth] = fieldNames.Count - 1;
                }

                if (_fieldSelections[_depth] == 0)
                {
                    _scrollLocation = EditorGUILayout.BeginScrollView(_scrollLocation);
                    DisplaySubscribedActions(baseSubject);
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    GetRecursiveFields(chosenFields[_fieldSelections[_depth] - 1].GetValue(baseSubject));
                }

            }
            else
            {
                EditorGUILayout.LabelField($"{subjectType.ToString()} is not a travesable class/ doesn't have any fields", EditorStyles.whiteLargeLabel);
            }
        }

        public void DisplaySubscribedActions(object subject)
        {
            if (subject == null)
            {
                EditorGUILayout.LabelField("Field hasn't been instatiated. Start Game to View.", EditorStyles.largeLabel);
                return;
            }

            Type subjectType = subject.GetType();
            object baseSubject = Convert.ChangeType(subject, subjectType);

            GUILayout.Label(subjectType.ToString(), EditorStyles.boldLabel);

            //Get Feilds
            FieldInfo[] fieldValues = subjectType.GetFields();
            List<FieldInfo> fieldInfo = fieldValues.Where(field => field.FieldType == typeof(Action) || (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Action<>))).ToList();

            //Get Events
            EventInfo[] eventValues = subjectType.GetEvents();
            fieldInfo.AddRange(eventValues.Select(events => subjectType.GetField(events.Name,
                                                                                 BindingFlags.NonPublic |
                                                                                 BindingFlags.Instance |
                                                                                 BindingFlags.GetField)));
            fieldInfo.RemoveAll(x => x == null);

            List<object> eventInvokes = fieldInfo.Select(field => field?.GetValue(baseSubject))
                                                  .ToList();

            List<Type[]> genericTypeArguments = fieldInfo.Select(x => x.FieldType.GenericTypeArguments).ToList();


            if (Application.isPlaying)
            {
                for (int i = 0; i < fieldInfo.Count; i++)
                {
                    DisplayDelagateInvokes(eventInvokes[i], $"{fieldInfo[i].Name}({String.Join(", ", genericTypeArguments[i].Select(x => x.ToString()))})");
                }
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Enter Playmode to view Actions and thier subscribed delegates.", EditorStyles.largeLabel);

            }
        }

        public void DisplayDelagateInvokes(object action, string actionName)
        {
            Delegate[] invokeList = (action as Delegate)?.GetInvocationList();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(actionName, EditorStyles.boldLabel);
            if (invokeList != null)
            {
                Array.ForEach(invokeList, (x =>
                {
                    EditorGUILayout.LabelField($"└ {x.Method.Name} : {x.Method.DeclaringType.ToString()}");
                }));
                invokeList = null;
            }
            else
            {
                EditorGUILayout.LabelField("►This Action has no subscribed delegates.◄");
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
    }

}
