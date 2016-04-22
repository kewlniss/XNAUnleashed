using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace XELibrary
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SoundManager : Microsoft.Xna.Framework.GameComponent
    {
        public bool RepeatPlayList = true;
        private AudioEngine engine;
        private WaveBank waveBank;
        private SoundBank soundBank;

        private Dictionary<string, Cue> cues = new Dictionary<string, Cue>();
        private Dictionary<string, AudioCategory> categories =
            new Dictionary<string, AudioCategory>();

        private string[] playList;
        private int currentSong;
        private Cue currentlyPlaying;

        public SoundManager(Game game, string xactProjectName)
            : this(game, xactProjectName, xactProjectName)
        { }

        public SoundManager(Game game, string xactProjectName,
                            string xactFileName)
            : this(game, xactProjectName, xactFileName,
                game.Content.RootDirectory + @"\Sounds\")
        { }

        public SoundManager(Game game, string xactProjectName,
                            string xactFileName, string contentPath)
            : base(game)
        {
            xactFileName = xactFileName.Replace(".xap", "");

            engine = new AudioEngine(contentPath + xactFileName + ".xgs");
            waveBank = new WaveBank(engine, contentPath + "Wave Bank.xwb");
            soundBank = new SoundBank(engine, contentPath + "Sound Bank.xsb");
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            engine.Update();

            if (currentlyPlaying != null) //are we playing a list?
            {
                //check current cue to see if it is playing
                //if not, go to next cue in list
                if (!currentlyPlaying.IsPlaying)
                {
                    currentSong++;

                    if (currentSong == playList.Length)
                    {
                        if (RepeatPlayList)
                            currentSong = 0;
                        else
                            StopPlayList();
                    }

                    //may have been set to null, if we finished our list
                    if (currentlyPlaying != null)
                    {
                        currentlyPlaying = soundBank.GetCue(
                            playList[currentSong]);
                        currentlyPlaying.Play();
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            soundBank.Dispose();
            waveBank.Dispose();
            engine.Dispose();

            playList = null;
            currentlyPlaying = null;
            cues = null;
            soundBank = null;
            waveBank = null;
            engine = null;

            base.Dispose(disposing);
        }

        public void SetGlobalVariable(string name, float amount)
        {
            engine.SetGlobalVariable(name, amount);
        }

        private void CheckCategory(string categoryName)
        {
            if (!categories.ContainsKey(categoryName))
                categories.Add(categoryName, engine.GetCategory(categoryName));
        }

        public void SetVolume(string categoryName, float volumeAmount)
        {
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

        public void StartPlayList(string[] playList)
        {
            StartPlayList(playList, 0);
        }

        public void StartPlayList(string[] playList, int startIndex)
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
            currentlyPlaying = soundBank.GetCue(playList[currentSong]);
            currentlyPlaying.Play();
        }

        public void StopPlayList()
        {
            if (currentlyPlaying != null)
            {
                currentlyPlaying.Stop(AudioStopOptions.Immediate);
                currentlyPlaying = null;
            }
        }
    }
}