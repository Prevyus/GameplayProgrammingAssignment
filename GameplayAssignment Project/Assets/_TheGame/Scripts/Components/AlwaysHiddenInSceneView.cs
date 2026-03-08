using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class AlwaysHiddenInSceneView : MonoBehaviour
{// THIS IS JUST A TOOL SO THAT I DONT HAVE OBJECTS I DONT WANNA SEE IN THE SCENE VIEW OR ACCIDENTALLY TOUCH THEM

#if UNITY_EDITOR
    public bool hide = true;
    public bool untouchable = true;

    void OnEnable()
    {
        var svm = SceneVisibilityManager.instance;
        if (hide) svm.Hide(gameObject, true);
        if (untouchable) svm.DisablePicking(gameObject, true);
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }

    void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingEditMode)
        {
            var svm = SceneVisibilityManager.instance;
            if (hide) svm.Hide(gameObject, true);
            if (untouchable) svm.DisablePicking(gameObject, true);
        }
    }
#endif
}
