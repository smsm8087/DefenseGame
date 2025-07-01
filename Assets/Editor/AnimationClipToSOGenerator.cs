#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public static class AnimationClipToSOGenerator
{
    [MenuItem("Tools/Convert AnimationClip to JobIdleAnimationData")]
    public static void ConvertSelectedClipToSO()
    {
        if (Selection.activeObject is not AnimationClip clip)
        {
            Debug.LogError("선택된 객체가 AnimationClip이 아닙니다.");
            return;
        }

        var curveBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        var spriteBinding = curveBindings.FirstOrDefault(b => b.propertyName == "m_Sprite");

        if (spriteBinding.propertyName != "m_Sprite")
        {
            Debug.LogError("이 AnimationClip은 Sprite 기반이 아닙니다.");
            return;
        }

        var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, spriteBinding);

        var frameDataList = new List<FrameData>();
        for (int i = 0; i < keyframes.Length; i++)
        {
            var current = keyframes[i];
            var nextTime = (i + 1 < keyframes.Length) ? keyframes[i + 1].time : clip.length;
            float duration = nextTime - current.time;

            frameDataList.Add(new FrameData
            {
                sprite = current.value as Sprite,
                duration = duration
            });
        }

        // ScriptableObject 생성
        var so = ScriptableObject.CreateInstance<JobIdleAnimationData>();
        so.jobType = clip.name.ToLower(); // 이름으로 jobType 설정
        so.idleFrames = frameDataList;

        // 저장 경로
        string folderPath = "Assets/GeneratedAnimations";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string assetPath = $"{folderPath}/{clip.name}_IdleData.asset";
        AssetDatabase.CreateAsset(so, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"<color=green>ScriptableObject 생성 완료:</color> {assetPath}");
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = so;
    }
}
#endif