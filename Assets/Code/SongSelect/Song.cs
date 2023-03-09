using Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongSelect
{
    public struct Song
    {
        public NoteFile noteFile;
        public Sprite noteSprite;
        public AudioClip noteAudioClip;

        public Song(NoteFile noteFile, AudioClip audioClip, Sprite sprite)
        {
            this.noteFile = noteFile;
            noteAudioClip = audioClip;
            noteSprite = sprite;
        }
    }
}
