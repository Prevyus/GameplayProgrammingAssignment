using UnityEngine;

namespace Toolbox
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionFieldName;

        public ShowIfAttribute(string conditionFieldName)
        {
            ConditionFieldName = conditionFieldName;
        }
    }
}
