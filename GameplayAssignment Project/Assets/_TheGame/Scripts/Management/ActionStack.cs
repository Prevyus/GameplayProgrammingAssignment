using System.Collections.Generic;
using UnityEngine;

namespace Custom
{
    public class ActionStack : MonoBehaviour
    { // ACTION STACK, WHICH IS USED TO CONTROL THE GAME FLOW IN THE GAME SCENE
        public interface IAction
        {
            void OnBegin(bool firstTime);
            void OnUpdate();
            void OnInterrupted();
            void OnFinished();
            bool IsDone();
        }

        public abstract class Action : IAction
        {
            public virtual void OnBegin(bool firstTime) { }
            public virtual void OnUpdate() { }
            public virtual void OnInterrupted() { }
            public virtual void OnFinished() { }
            public virtual bool IsDone() { return true; }
        }

        private List<IAction> stack = new List<IAction>();
        private HashSet<IAction> firstTimeActions = new HashSet<IAction>();
        public IAction current;

        public bool IsEmpty => current == null && stack.Count == 0;

        public void Push(IAction action)
        {
            if (action == null) return;

            if (current != null) current.OnInterrupted();
            stack.RemoveAll(a => a == action);
            stack.Insert(0, action);

            if (current != null && current != action)
                current = null;
        }

        public void PushOnce(IAction action)
        { // PUSHES AN ACTION ONLY IF ITS A DIFFERENT TYPE THAN THE CURRENT ACTION
            if (action == null) return;

            if (current != null && current.GetType() == action.GetType()) return;

            if (current != null) current.OnInterrupted();
            stack.RemoveAll(a => a == action);
            stack.Insert(0, action);

            if (current != null && current != action)
                current = null;
        }

        void Update()
        {
            UpdateStack();
        }

        void UpdateStack()
        {
            if (IsEmpty) return;

            while (current == null && stack.Count > 0)
            {
                current = stack[0];

                bool firstTime = !firstTimeActions.Contains(current);
                firstTimeActions.Add(current);

                current.OnBegin(firstTime);

                if (stack.Count > 0 && current != stack[0])
                {
                    current = null;
                    return;
                }
            }

            if (current == null) return;

            current.OnUpdate();

            if (stack.Count > 0 && current == stack[0] && current.IsDone())
            {
                stack.RemoveAt(0);
                current.OnFinished();
                firstTimeActions.Remove(current);
                current = null;
            }
            else if (stack.Count == 0 || current != stack[0])
            {
                current = null;
            }
        }
    }
}