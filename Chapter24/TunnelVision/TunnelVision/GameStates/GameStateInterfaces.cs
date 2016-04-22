using System;
using Microsoft.Xna.Framework;
using XELibrary;

namespace TunnelVision
{
    public interface ITitleIntroState : IGameState { }
    public interface IStartMenuState : IGameState { }
    public interface IOptionsMenuState : IGameState { }
    public interface IPausedState : IGameState { }
    public interface ILostGameState : IGameState { }
    public interface IWonGameState : IGameState { }
    public interface IStartLevelState : IGameState { }
    public interface IYesNoDialogState : IGameState { }
    public interface IHighScoresState : IGameState
    {
        void SaveHighScore();
        bool AlwaysDisplay { get; set;  }
    }
    public interface IPlayingState : IGameState
    {
        void StartGame();
        int CurrentLevel { get; }
        int Score { get; }
    }
    public interface IFadingState : IGameState
    {
        Color Color { get; set; }
    }
}
