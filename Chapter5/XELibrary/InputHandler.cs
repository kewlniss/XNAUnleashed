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
    public interface IInputHandler
    {
        KeyboardState KeyboardState { get; }

        GamePadState[] GamePads { get; }

#if !XBOX360
        MouseState MouseState { get; }
        MouseState PreviousMouseState { get; }
#endif
    };

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputHandler : Microsoft.Xna.Framework.GameComponent, IInputHandler
    {

        private KeyboardState keyboardState;

        private GamePadState[] gamePads = new GamePadState[4];

#if !XBOX360
        private MouseState mouseState;
        private MouseState prevMouseState;
#endif

        public InputHandler(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IInputHandler), this);

#if !XBOX360
            Game.IsMouseVisible = true;
            prevMouseState = Mouse.GetState();
#endif
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

#if !XBOX360
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
#endif

            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                Game.Exit();

            gamePads[0] = GamePad.GetState(PlayerIndex.One);
            gamePads[1] = GamePad.GetState(PlayerIndex.Two);
            gamePads[2] = GamePad.GetState(PlayerIndex.Three);
            gamePads[3] = GamePad.GetState(PlayerIndex.Four);

            if (gamePads[0].IsButtonDown(Buttons.Back))
                Game.Exit();

            base.Update(gameTime);
        }

        #region IInputHandler Members

        public KeyboardState KeyboardState
        {
            get { return (keyboardState); }
        }

        public GamePadState[] GamePads
        {
            get { return (gamePads); }
        }

#if !XBOX360
        public MouseState MouseState
        {
            get { return (mouseState); }
        }

        public MouseState PreviousMouseState
        {
            get { return (prevMouseState); }
        }
#endif

        #endregion

    }
}