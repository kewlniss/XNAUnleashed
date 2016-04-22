using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace XELibrary
{
    public abstract partial class GameState : DrawableGameComponent, IGameState
    {
        protected IGameStateManager GameManager;
        protected IInputHandler Input;
        protected Rectangle TitleSafeArea;

        public GameState(Game game)
            : base(game)
        {
            GameManager = (IGameStateManager)game.Services.GetService(
                typeof(IGameStateManager));
            Input = (IInputHandler)game.Services.GetService(
                typeof(IInputHandler));
        }

        protected override void LoadContent()
        {
            TitleSafeArea = Utility.GetTitleSafeArea(GraphicsDevice, 0.85f);
        }

        internal protected virtual void StateChanged(object sender, EventArgs e)
        {
            if (GameManager.State == this.Value)
                Visible = Enabled = true;
            else
                Visible = Enabled = false;
        }

        #region IGameState Members
        public GameState Value
        {
            get { return (this); }
        }
        #endregion
    }
}
