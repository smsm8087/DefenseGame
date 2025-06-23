using UnityEditor;
using UnityEngine;
using System.IO;

namespace Editor
{
    public class ExportCsvForServer
    {
        [MenuItem("Tools/Export CSV for Server")]
        public static void ExportCsv()
        {
            string sourceFolder = "Assets/DataExcels/";

            // GUI 로 서버 프로젝트 폴더 선택
            string destFolder = EditorUtility.OpenFolderPanel("서버 Data 폴더 선택", "", "");

            if (string.IsNullOrEmpty(destFolder))
            {
                Debug.LogWarning("Export 취소됨: 대상 폴더 선택 안됨");
                return;
            }

            var files = Directory.GetFiles(sourceFolder, "*.csv", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var destPath = Path.Combine(destFolder, fileName);
                File.Copy(file, destPath, true);
                Debug.Log($"Exported: {fileName}");
            }

            Debug.Log($"<color=green>[CSV Export]</color> 서버로 복사 완료! → {destFolder}");
        }
    }
}