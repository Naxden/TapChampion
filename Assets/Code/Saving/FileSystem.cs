using UnityEngine;
using System.IO;
using Saving.Note;
using UnityEditor;
using System;
using UnityEngine.Networking;
using System.Collections;

namespace Saving.SavingSystem
{
    public static class FileSystem
    {
        const string MUSIC_BEGIN = "#Music\n", MUSIC_END = "#EndMusic";
        const string NOTE_MAP_BEGIN = "#NoteMap\n", NOTE_MAP_END = "#EndNoteMap";
        const string SPRITE_BEGIN = "#Sprite\n", SPRITE_END = "#EndSprite";


        public static void WriteNoteFile(string path, NoteFile toSave)
        {
            string content = JsonUtility.ToJson(toSave, true);

            WriteFile(path, content);
        }

        private static void WriteFile(string path, string content)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);

            using StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            writer.WriteLine(content);
        }

        private static void WriteBinaryFile(string path, byte[] content)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);

            using BinaryWriter writer = new BinaryWriter(fileStream, System.Text.Encoding.UTF8);
            writer.Write(content);
        }

        public static IEnumerator GetAudioClipRoutine(string songName, AudioSource audioOutput)
        {
            using var www = UnityWebRequestMultimedia.GetAudioClip("file://" + GetPath($"/Songs/{songName}/{songName}.mp3"), AudioType.MPEG);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                audioOutput.clip = DownloadHandlerAudioClip.GetContent(www);
            }
        }

        public static Sprite GetSprite(string spriteName)
        {
            string path = GetPath($"/Songs/{spriteName}/{spriteName}.png");
            byte[] buffor = ReadBinaryFile(path);

            if (buffor != null)
            {
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(buffor);

                Sprite sprite = Sprite.Create(texture2D,
                                              new Rect(0.0f, 0.0f, texture2D.width, texture2D.height),
                                              new Vector2(0.5f, 0.5f), 100.0f);

                return sprite;
            }

            Debug.LogWarning($"GetSprite: Couldn't load sprite at {path}");
            return null;
        }

        public static NoteFile GetNoteFile(string noteFileName)
        {
            string path = GetPath($"/Songs/{noteFileName}/{noteFileName}.note");

            if (File.Exists(path))
            {
                return StringToNoteFile(ReadFile(path));
            }

            Debug.LogWarning($"GetSprite: Couldn't load sprite at {path}");
            return null;
        }

        private static string GetPath(string fileName)
        {
            return Directory.GetCurrentDirectory() + fileName;
        }

        private static string ReadFile(string path)
        {
            if (File.Exists(path))
            {
                using StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8);
                return sr.ReadToEnd();
            }

            Debug.LogWarning($"ReadFile: Failed to read file at {path}");
            return null;
        }

        private static byte[] ReadBinaryFile(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            Debug.LogWarning($"ReadBinaryFile: Couldn't find file at {path}");
            return null;
        }

        private static string GetPartOfString(string content, string beginFlag, string endFlag)
        {
            int beginIndex = content.IndexOf(beginFlag) + beginFlag.Length;
            int endIndex = content.IndexOf(endFlag);

            if (beginIndex < endIndex)
                return content.Substring(beginIndex, endIndex - beginIndex);

            Debug.LogWarning($"GetPartOfString: Couldn't find that flags {beginFlag} : {endFlag} in passed content");
            return null;
        }

        public static string[] GetFileList(string directoryPath)
        {
            return Directory.GetFiles(directoryPath);
        }

        public static void ImportTapchFile()
        {
            string file = EditorUtility.OpenFilePanel("Select File to import", GetPath("/Downloads"), "tapch");
            if (file.Length == 0)
            {
                Debug.Log("ImportTapchFile: File import fail");
                return;
            }

            string content = ReadFile(file);
            NoteFile noteFile = StringToNoteFile(content);
            string title = noteFile.title;
            string directoryPath = SetSongDirectory(title);

            WriteNoteFile($"{directoryPath}/{title}.note", noteFile);

            WriteSpriteFile($"{directoryPath}/{title}.png", content);

            WriteMusicFile($"{directoryPath}/{title}.mp3", content);
        }

        public static void ExportTapchFile()
        {

        }

        private static NoteFile StringToNoteFile(string content)
        {
            NoteFile noteFile = JsonUtility.FromJson<NoteFile>(content);

            if (noteFile != null)
                return noteFile;

            Debug.LogWarning("StringToNoteFile: Couldn't convert to NoteFile properly");
            return null;
        }

        private static string NoteFileToString(NoteFile noteFile)
        {
            if (noteFile != null)
                return JsonUtility.ToJson(noteFile, true);

            Debug.LogWarning("NoteFileToString: Couldn't convert to JsonString properly");
            return null;
        }

        private static void WriteSpriteFile(string path, string content)
        {
            byte[] buffor = Convert.FromBase64String(
                GetPartOfString(content, SPRITE_BEGIN, SPRITE_END));

            WriteBinaryFile(path, buffor);
        }

        public static void WriteMusicFile(string path, string content)
        {
            byte[] buffor = Convert.FromBase64String(
                GetPartOfString(content, MUSIC_BEGIN, MUSIC_END));

            WriteBinaryFile(path, buffor);
        }

        private static string SetSongDirectory(string songTitle)
        {
            string targetPath = GetPath("/Songs/") + songTitle;

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
            return targetPath;
        }
    }
}

