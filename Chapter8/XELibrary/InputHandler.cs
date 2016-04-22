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
        KeyboardHandler KeyboardState { get; }

        GamePadState[] GamePads { get; }

        ButtonHandler ButtonHandler { get; }

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
        private KeyboardHandler keyboard;
        private ButtonHandler gamePadHandler = new ButtonHandler();

#if !XBOX360
        private MouseState mouseState;
        private MouseState prevMouseState;
#endif

        public InputHandler(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IInputHandler), this);

            //initialize our member fields
            keyboard = new KeyboardHandler();

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
            keyboard.Update();

            gamePadHandler.Update();

            if (keyboard.IsKeyDown(Keys.Escape))
                Game.Exit();

            if (gamePadHandler.WasButtonPressed(0, Buttons.Back))
                Game.Exit();


#if !XBOX360
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
#endif

            base.Update(gameTime);
        }

        #region IInputHandler Members

        public KeyboardHandler KeyboardState
        {
            get { return (keyboard); }
        }

        public ButtonHandler ButtonHandler
        {
            get { return (gamePadHandler); }
        }

        public GamePadState[] GamePads
        {
            get { return (gamePadHandler.GamePads); }
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

    public class ButtonHandler
    {
        private GamePadState[] prevGamePadsState = new GamePadState[4];
        private GamePadState[] gamePadsState = new GamePadState[4];

        public GamePadState[] GamePads
        {
            get
            {
                return (gamePadsState);
            }
        }

        public ButtonHandler()
        {
            prevGamePadsState[0] = GamePad.GetState(PlayerIndex.One);
            prevGamePadsState[1] = GamePad.GetState(PlayerIndex.Two);
            prevGamePadsState[2] = GamePad.GetState(PlayerIndex.Three);
            prevGamePadsState[3] = GamePad.GetState(PlayerIndex.Four);
        }

        public void Update()
        {
            //set our previous state to our new state
            prevGamePadsState[0] = gamePadsState[0];
            prevGamePadsState[1] = gamePadsState[1];
            prevGamePadsState[2] = gamePadsState[2];
            prevGamePadsState[3] = gamePadsState[3];

            //get our new state
            gamePadsState[0] = GamePad.GetState(PlayerIndex.One);
            gamePadsState[1] = GamePad.GetState(PlayerIndex.Two);
            gamePadsState[2] = GamePad.GetState(PlayerIndex.Three);
            gamePadsState[3] = GamePad.GetState(PlayerIndex.Four);
        }

        public bool WasButtonPressed(int playerIndex, Buttons button)
        {
            return (gamePadsState[playerIndex].IsButtonDown(button) &&
                prevGamePadsState[playerIndex].IsButtonUp(button)); 
        }
    }

    public class KeyboardHandler
    {
        private KeyboardState prevKeyboardState;
        private KeyboardState keyboardState;

        public KeyboardHandler()
        {
            prevKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return (keyboardState.IsKeyDown(key));
        }

        public bool IsHoldingKey(Keys key)
        {
            return (keyboardState.IsKeyDown(key) &&
                prevKeyboardState.IsKeyDown(key));
        }

        public bool WasKeyPressed(Keys key)
        {
            return (keyboardState.IsKeyDown(key) &&
                prevKeyboardState.IsKeyUp(key));
        }

        public bool HasReleasedKey(Keys key)
        {
            return (keyboardState.IsKeyUp(key) &&
                prevKeyboardState.IsKeyDown(key));
        }

        public void Update()
        {
            //set our previous state to our new state
            prevKeyboardState = keyboardState;

            //get our new state
            keyboardState = Keyboard.GetState();
        }
    }
}