using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Saving.Note;

public static class FileSystem
{
    public static NoteFile ReadNoteFile<NoteFile>(string fileName)
    {
        return JsonUtility.FromJson<NoteFile>(GetJsonString(fileName));
    }

    public static void SaveNoteFile<NoteFile>(NoteFile toSave, string fileName)
    {
        string content = JsonUtility.ToJson(toSave, true);

        WriteFile(GetPath(fileName), content);
    }
    private static void WriteFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
        {
            writer.WriteLine(content);
        }
    }

    private static string GetPath(string fileName)
    {
        return Application.dataPath + "/Notes/" + fileName + ".note";
    }

    private static string GetJsonString(string fileName)
    {
        if (File.Exists(GetPath(fileName)))
        {
            string content = "";

            using (StreamReader sr = new StreamReader(GetPath(fileName), System.Text.Encoding.UTF8))
            {
                //need to manually build string because "prettyPrint" is ruining string with escape sequences
                while (!sr.EndOfStream)
                {
                    content += sr.ReadLine();
                }

                return content;
            }
        }
        return "";
    }
}

