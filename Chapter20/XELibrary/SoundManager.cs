using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Media;

namespace XELibrary
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SoundManager //: Microsoft.Xna.Framework.GameComponent
    {
        public bool RepeatPlayList = true;

        private Song[] playList;
        private int currentSong;

        //private float soundVolume = 1f;

       // public SoundManager(Game game)
       //     : base(game) { }
        /*
        public override void Initialize()
        {
            //MediaPlayer.ActiveSongChanged += new EventHandler(MediaPlayer_ActiveSongChanged);

            base.Initialize();
        }
        */
        //override
        public void Update()//GameTime gameTime)
        {
            if (playList.Length > 0) //are we playing a list?
            {
                //check current cue to see if it is playing
                //if not, go to next cue in list
                if (MediaPlayer.State != MediaState.Playing)
                {
                    currentSong++;

                    if (currentSong == playList.Length)
                    {
                        if (RepeatPlayList)
                            currentSong = 0;
                        else
                            return;
                    }

                    if (MediaPlayer.State != MediaState.Playing)
                        MediaPlayer.Play(playList[currentSong]);

                }
            }

            //base.Update(gameTime);
        }

        public void StartPlayList(Song[] playList)
        {
            StartPlayList(playList, 0);
        }

        public void StartPlayList(Song[] playList, int startIndex)
        {
            if (playList.Length == 0)
                return;

            this.playList = playList;

            if (startIndex > playList.Length)
                startIndex = 0;

            StartPlayList(startIndex);
        }

        public void StartPlayList(int startIndex)
        {
            if (playList.Length == 0)
                return;

            currentSong = startIndex;
            MediaPlayer.Play(playList[currentSong]);
            MediaPlayer.IsRepeating = false;
        }

        public void StopPlayList()
        {
            MediaPlayer.Stop();
        }
    }
}


    /*
    
        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            currentSong++;

            if (currentSong == playList.Length)
            {
                if (RepeatPlayList)
                    currentSong = 0;
                else
                    return;
            }

            if (MediaPlayer.State != MediaState.Playing)
                MediaPlayer.Play(playList[currentSong]);
        }
     
    public void SetVolume(string categoryName, float volumeAmount)
    {
        if (categoryName.ToLower() == "music")
            MediaPlayer.Volume = volumeAmount;
        else
            soundVolume = volumeAmount;

        CheckCategory(categoryName);

        categories[categoryName].SetVolume(volumeAmount);
    }

    public void PauseCategory(string categoryName)
    {
        CheckCategory(categoryName);

        categories[categoryName].Pause();
    }

    public void ResumeCategory(string categoryName)
    {
        CheckCategory(categoryName);

        categories[categoryName].Resume();
    }

    public bool IsPlaying(string cueName)
    {
        if (cues.ContainsKey(cueName))
            return (cues[cueName].IsPlaying);

        return (false);
    }

    public void Play(string cueName)
    {
        Cue prevCue = null;

        if (!cues.ContainsKey(cueName))
            cues.Add(cueName, soundBank.GetCue(cueName));
        else
        {
            //store our cue if we were playing
            if (cues[cueName].IsPlaying)
                prevCue = cues[cueName];

            cues[cueName] = soundBank.GetCue(cueName);
        }

        //if we weren’t playing, set previous to our current cue name
        if (prevCue == null)
            prevCue = cues[cueName];

        try
        {
            cues[cueName].Play();
        }
        catch (InstancePlayLimitException)
        {
            //hit limit exception, set our cue to the previous
            //and let’s stop it and then start it up again ...
            cues[cueName] = prevCue;

            if (cues[cueName].IsPlaying)
                cues[cueName].Stop(AudioStopOptions.AsAuthored);

            Toggle(cueName);
        }
    }

    public void Pause(string cueName)
    {
        if (cues.ContainsKey(cueName))
            cues[cueName].Pause();
    }

    public void Resume(string cueName)
    {
        if (cues.ContainsKey(cueName))
            cues[cueName].Resume();
    }

    public void Toggle(string cueName)
    {
        if (cues.ContainsKey(cueName))
        {
            Cue cue = cues[cueName];

            if (cue.IsPaused)
            {
                cue.Resume();
            }
            else if (cue.IsPlaying)
            {
                cue.Pause();
            }
            else //played but stopped 
            {
                //need to reget cue if stopped
                Play(cueName);
            }
        }
        else //never played, need to reget cue
            Play(cueName);
    }

    public void StopAll()
    {
        foreach (Cue cue in cues.Values)
            cue.Stop(AudioStopOptions.Immediate);
    }

    public void Stop(string cueName)
    {
        if (cues.ContainsKey(cueName))
            cues[cueName].Stop(AudioStopOptions.Immediate);
        cues.Remove(cueName);
    }


     */
