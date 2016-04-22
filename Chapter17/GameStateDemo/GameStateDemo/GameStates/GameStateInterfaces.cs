using System;
using Microsoft.Xna.Framework.Graphics;
using XELibrary;
using Microsoft.Xna.Framework;

namespace GameStateDemo
{
    public interface ITitleIntroState : IGameState { }
    public interface IStartMenuState : IGameState { }
    public interface IOptionsMenuState : IGameState { }
    public interface IPlayingState : IGameState { }
    public interface IPausedState : IGameState { }
    public interface ILostGameState : IGameState { }
    public interface IWonGameState : IGameState { }
    public interface IStartLevelState : IGameState { }
    public interface IYesNoDialogState : IGameState { }
    public interface IFadingState : IGameState
    {
        Color Color { get; set; }
    }
}
