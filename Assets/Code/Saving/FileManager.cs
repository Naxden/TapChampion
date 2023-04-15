using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections;
using SimpleFileBrowser;
using UnityEditor;
using JetBrains.Annotations;

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

        private static void WriteFile(string path, in string content)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);

            using StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            writer.WriteLine(content);
        }

        private static void WriteBinaryFile(string path, in byte[] content)
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

            if (buffer == null)
            {
                Debug.LogWarning($"GetSprite: Couldn't load sprite at {path}");
                return null;
            }
            
            Texture2D texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(buffer);

            Sprite sprite = Sprite.Create(texture2D,
                                            new Rect(0.0f, 0.0f, texture2D.width, texture2D.height),
                                            new Vector2(0.5f, 0.5f), 100.0f);

            return sprite;
        }

        public static NoteFile GetNoteFile(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"GetNoteFile: Couldn't load NoteFile at {path}");
                return null;
            }

            return StringToNoteFile(ReadFile(path));
        }

        public static string GetPath(string fileName)
        {
            return Directory.GetCurrentDirectory() + '/' + fileName;
        }

        public static string[] GetAllSongs()
        {
            string songDirPath = GetPath("Songs");
            return Directory.GetDirectories(songDirPath);
        }

        public static int GetSongsCount()
        {
            return GetAllSongs().Length;
        }

        private static string ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"ReadFile: Failed to read file at {path}");
                return null;
            }

            using StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8);
            return sr.ReadToEnd();
        }

        private static byte[] ReadBinaryFile(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"ReadBinaryFile: Couldn't find file at {path}");
                return null;
            }

            return File.ReadAllBytes(path);
        }

        public static string GetPartOfString(string content, string beginFlag, string endFlag)
        {
            int beginIndex = content.LastIndexOf(beginFlag) + beginFlag.Length;
            int endIndex = endFlag == "\0" ? content.Length : content.IndexOf(endFlag);

            if (beginIndex >= endIndex)
            {
                Debug.LogWarning($"{beginIndex} {endIndex}");
                Debug.LogWarning($"GetPartOfString: Couldn't find that flags " +
                                 $"{beginFlag} : {endFlag} in passed content");
                return null;
            }

            return content.Substring(beginIndex, endIndex - beginIndex);
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

        public static bool ImportTapchFile(string filePath)
        {
            string content = ReadFile(filePath);

            NoteFile noteFile = StringToNoteFile(GetPartOfString(content, NOTE_MAP_BEGIN, NOTE_MAP_END));
            string title = noteFile.title;

            if (DoesSongExist(title))
            {
                Debug.LogWarning($"ImportTapchFile: Song with name {title} already exists");
                return false;
            }
            string directoryPath = CreateSongDirectory(title);

            WriteNoteFile($"{directoryPath}/{title}.note", noteFile);

            WriteMusicFile($"{directoryPath}/{title}.mp3", content, true);

            WriteSpriteFile($"{directoryPath}/{title}.png", content, true);

            return true;
        }

        public static void ExportTapchFile(string songTitle)
        {
            string songPath = GetSongDirectory(songTitle);

            NoteFile noteFile = GetNoteFile($"{songPath}/{songTitle}.note");

            for (int i = 0; i < Enum.GetNames(typeof(Difficulty)).Length; i++)
            {
                noteFile.highScores[i] = 0;
                noteFile.accuracies[i] = 0f;
            }
            
            string content = NOTE_MAP_BEGIN;
            content += NoteFileToString(noteFile);
            content += NOTE_MAP_END;

            string songMusicFile = Convert.ToBase64String(ReadBinaryFile($"{songPath}/{songTitle}.mp3"));
            content += MUSIC_BEGIN;
            content += songMusicFile;
            content += MUSIC_END;

            string songImageFile = Convert.ToBase64String(ReadBinaryFile($"{songPath}/{songTitle}.png"));
            content += SPRITE_BEGIN;
            content += songImageFile;
            content += SPRITE_END;

            string destinationPath = GetPath($"Downloads/{songTitle}.tapch");
            WriteFile(destinationPath, content);
        }

        public static void RecordNewSong(string songTitle, NoteFile noteFile, string imagePath, string musicPath)
        {
            CreateSongDirectory(songTitle);

            RecordSong(songTitle, noteFile, imagePath, musicPath);
        }

        public static void RecordSong(string songTitle, NoteFile noteFile, string imagePath, string musicPath)
        {
            string songDirectory = GetSongDirectory(songTitle);

            if (imagePath != null)
            {
                byte[] bytes = ReadBinaryFile(imagePath);

                WriteBinaryFile($"{songDirectory}/{songTitle}.png", bytes);
            }

            if (musicPath != null)
            {
                byte[] bytes = ReadBinaryFile(musicPath);

                WriteBinaryFile($"{songDirectory}/{songTitle}.mp3", bytes);
            }

            if (noteFile == null)
            {
                Debug.LogWarning("RecordSong: noteFile was empty");
                return;
            }

            WriteNoteFile($"{songDirectory}/{songTitle}.note", noteFile);
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
            string content = JsonUtility.ToJson(userSettings, true);
            
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

        private static void WriteSpriteFile(string path, string content, bool importing)
        {
            byte[] buffer = Convert.FromBase64String( importing ?
                GetPartOfString(content, SPRITE_BEGIN, SPRITE_END) :
                content);

            WriteBinaryFile(path, buffer);
        }

        public static void WriteMusicFile(string path, string content, bool importing)
        {
            byte[] buffer = Convert.FromBase64String( importing ?
                GetPartOfString(content, MUSIC_BEGIN, MUSIC_END) :
                content);

            WriteBinaryFile(path, buffer);
        }

        private static string CreateSongDirectory(string songTitle)
        {
            string targetPath = GetPath("/Songs/") + songTitle;

            if (DoesSongExist(songTitle))
            {
                Debug.LogWarning($"CreateSongDirectory: Song with name {songTitle} already exists");
                return null;
            }
            
            Directory.CreateDirectory(targetPath);
            return targetPath;
        }

        private static string GetSongDirectory(string songTitle)
        {
            string targetPath = GetPath("/Songs/") + songTitle;

            return targetPath;
        }

        public static void DeleteSongDirectory(string songTitle)
        {
            string targetPath = GetPath("/Songs/") + songTitle;

            if (!Directory.Exists(targetPath))
                return;
            
            Directory.Delete(targetPath, true);
        }
    }
}

