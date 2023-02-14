using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using UnityEngine.Networking;
using System.Collections;
using SimpleFileBrowser;

namespace Saving
{
    public static class FileManager
    {
        const string MUSIC_BEGIN = "#Music\n", MUSIC_END = "#EndMusic";
        const string NOTE_MAP_BEGIN = "#NoteMap\n", NOTE_MAP_END = "#EndNoteMap";
        const string SPRITE_BEGIN = "#Sprite\n", SPRITE_END = "#EndSprite";

        public enum FileExtension {TAPCH, MUSIC, IMAGE};

        public static bool loadDialogInitialized = false;

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

        public static IEnumerator GetAudioClipRoutine(string path, AudioSource audioOutput)
        {
            using var www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG);
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

        public static Sprite GetSprite(string path)
        {
            byte[] buffer = ReadBinaryFile(path);

            if (buffer != null)
            {
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(buffer);

                Sprite sprite = Sprite.Create(texture2D,
                                              new Rect(0.0f, 0.0f, texture2D.width, texture2D.height),
                                              new Vector2(0.5f, 0.5f), 100.0f);

                return sprite;
            }

            Debug.LogWarning($"GetSprite: Couldn't load sprite at {path}");
            return null;
        }

        public static NoteFile GetNoteFile(string path)
        {
            if (File.Exists(path))
            {
                return StringToNoteFile(ReadFile(path));
            }

            Debug.LogWarning($"GetSprite: Couldn't load sprite at {path}");
            return null;
        }

        public static string GetPath(string fileName)
        {
            return Directory.GetCurrentDirectory() + '/' + fileName;
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

        public static string GetPartOfString(string content, string beginFlag, string endFlag)
        {
            int beginIndex = content.LastIndexOf(beginFlag) + beginFlag.Length;
            int endIndex = content.IndexOf(endFlag);

            if (beginIndex < endIndex)
                return content.Substring(beginIndex, endIndex - beginIndex);

            Debug.LogWarning($"GetPartOfString: Couldn't find that flags {beginFlag} : {endFlag} in passed content");
            return null;
        }

        public static bool DoesSongExist(string songName)
        {
            return Directory.Exists(GetPath("Songs/") + songName);
        } 

        public static void FileDialogInitialize()
        {
            FileBrowser.SetFilters(true, new FileBrowser.Filter("TapChapmpion", ".tapch"),
                                         new FileBrowser.Filter("Images", ".jpg", ".png"),
                                         new FileBrowser.Filter("Music", ".mp3"));
            FileBrowser.AddQuickLink("Downloads", GetPath("Downloads"));
            FileBrowser.AddQuickLink("Songs", GetPath("Songs"));
            loadDialogInitialized = true;
        }

        public static void ShowLoadDialog(FileBrowser.OnSuccess onSuccess, FileBrowser.OnCancel onCancel,
                                          string label, FileExtension extension)
        {
            if (onSuccess == null)
            {
                Debug.LogError("ShowLoadDialog: onSucces delegate is empty");
                return;
            }
            if (onCancel == null)
            {
                Debug.LogError("ShowLoadDialog: onCancel delegate is empty");
                return;
            }
            if (!loadDialogInitialized)
            {
                FileDialogInitialize();
            }

            SetDefaultFilter(extension);
            FileBrowser.ShowLoadDialog(onSuccess, onCancel, FileBrowser.PickMode.Files, 
                                        false, GetPath("Downloads"), null, label);
        }
        public static void SetDefaultFilter(FileExtension fileExtension)
        {
            switch ((int)fileExtension)
            {
                case 0:
                    FileBrowser.SetDefaultFilter(".tapch");
                    break;
                case 1:
                    FileBrowser.SetDefaultFilter(".mp3");
                    break;
                case 2:
                    FileBrowser.SetDefaultFilter(".png");
                    break;
                default:
                    break;
            }
        }

        public static void ImportTapchFile()
        {
            string filePath = "";
            FileManager.ShowLoadDialog((paths) => filePath = paths[0], 
                                        () => filePath = "",
                                        "Load TapCh file",
                                        FileExtension.TAPCH);
                                        
            if (filePath.Length == 0)
            {
                Debug.Log("ImportTapchFile: File import fail");
                return;
            }

            string content = ReadFile(filePath);
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

        public static void RecordSong(string songPath, string imagePath, NoteFile noteFile)
        {

        }

        public static UserSettings GetUserSettings()
        {
            UserSettings userSettings;
            string content = ReadFile(GetPath("userSettings.json"));

            if (content == null)
            {
                Debug.LogWarning("GetDifficulty: Couldn't open userSettings");
                return null;
            }

            userSettings = JsonUtility.FromJson<UserSettings>(content);

            if (userSettings == null)
            {
                Debug.LogWarning("GetUserSettings: Failed to convert from .json");
            }

            return userSettings;
        }

        public static void WriteUserSettings(UserSettings userSettings)
        {
            string content;

            content = JsonUtility.ToJson(userSettings, true);
            
            if (content == null)
            {
                Debug.LogWarning("WriteUserSettings: Failed to convert to json String");
            }

            string path = GetPath("userSettings.json");
            WriteFile(path, content);
        }

        private static NoteFile StringToNoteFile(string content)
        {
            NoteFile noteFile = JsonUtility.FromJson<NoteFile>(content);

            if (noteFile == null)
            {
                Debug.LogWarning("StringToNoteFile: Couldn't convert to NoteFile properly");
                return null;
            }

            return noteFile;
        }

        private static string NoteFileToString(NoteFile noteFile)
        {
            if (noteFile == null)
            {
                Debug.LogWarning("NoteFileToString: Couldn't convert to JsonString properly");
                return null;
            }

            return JsonUtility.ToJson(noteFile, true);
        }

        private static void WriteSpriteFile(string path, string content)
        {
            byte[] buffer = Convert.FromBase64String(
                GetPartOfString(content, SPRITE_BEGIN, SPRITE_END));

            WriteBinaryFile(path, buffer);
        }

        public static void WriteMusicFile(string path, string content)
        {
            byte[] buffer = Convert.FromBase64String(
                GetPartOfString(content, MUSIC_BEGIN, MUSIC_END));

            WriteBinaryFile(path, buffer);
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

