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
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        protected IInputHandler input;

        private Matrix projection;
        private Matrix view;

        protected Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
        private Vector3 cameraTarget = Vector3.Zero;
        private Vector3 cameraUpVector = Vector3.Up;

        private Vector3 cameraReference = new Vector3(0.0f, 0.0f, -1.0f);

        private float cameraYaw = 0.0f;
        private float cameraPitch = 0.0f;

        private const float spinRate = 120.0f;
        private const float moveRate = 120.0f;

        protected Vector3 movement = Vector3.Zero;

        protected int playerIndex = 0;

        private Viewport? viewport;

        public Camera(Game game)
            : base(game)
        {
            input = (IInputHandler)game.Services.GetService(typeof(IInputHandler));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();

            InitializeCamera();
        }


        private void InitializeCamera()
        {
            //Projection
            float aspectRatio = (float)Game.GraphicsDevice.Viewport.Width /
                (float)Game.GraphicsDevice.Viewport.Height;
            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio,
                0.0001f, 1000.0f, out projection);

            //View
            Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget,
                ref cameraUpVector, out view);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (input.KeyboardState.IsKeyDown(Keys.Left) ||
                (input.GamePads[playerIndex].IsButtonDown(Buttons.RightThumbstickLeft)) ||
                (input.GamePads[playerIndex].IsButtonDown(Buttons.DPadLeft)))
            {
                cameraYaw += (spinRate * timeDelta);
            }
            if (input.KeyboardState.IsKeyDown(Keys.Right) ||
                (input.GamePads[playerIndex].IsButtonDown(Buttons.RightThumbstickRight)) ||
                (input.GamePads[playerIndex].IsButtonDown(Buttons.DPadRight)))
            {
                cameraYaw -= (spinRate * timeDelta);
            }

            if (input.KeyboardState.IsKeyDown(Keys.Down) ||
                (input.GamePads[playerIndex].IsButtonDown(Buttons.RightThumbstickDown)) ||
                (input.GamePads[playerIndex].IsButtonDown(Buttons.DPadDown)))
            {
                cameraPitch -= (spinRate * timeDelta);
            }
            if (input.KeyboardState.IsKeyDown(Keys.Up) ||
                (input.GamePads[playerIndex].IsButtonDown(Buttons.RightThumbstickUp)) ||
                (input.GamePads[playerIndex].IsButtonDown(Buttons.DPadUp)))
            {
                cameraPitch += (spinRate * timeDelta);
            }

#if !XBOX360
            if ((input.PreviousMouseState.X > input.MouseState.X) &&
                (input.MouseState.LeftButton == ButtonState.Pressed))
            {
                cameraYaw += (spinRate * timeDelta);
            }
            else if ((input.PreviousMouseState.X < input.MouseState.X) &&
                (input.MouseState.LeftButton == ButtonState.Pressed))
            {
                cameraYaw -= (spinRate * timeDelta);
            }

            if ((input.PreviousMouseState.Y > input.MouseState.Y) &&
                (input.MouseState.LeftButton == ButtonState.Pressed))
            {
                cameraPitch += (spinRate * timeDelta);
            }
            else if ((input.PreviousMouseState.Y < input.MouseState.Y) &&
                (input.MouseState.LeftButton == ButtonState.Pressed))
            {
                cameraPitch -= (spinRate * timeDelta);
            }
#endif
            //reset camera angle if needed
            if (cameraYaw > 360)
                cameraYaw -= 360;
            else if (cameraYaw < 0)
                cameraYaw += 360;

            //keep camera from rotating a full 90 degrees in either direction
            if (cameraPitch > 89)
                cameraPitch = 89;
            if (cameraPitch < -89)
                cameraPitch = -89;

            //update movement (none for this base class)
            movement *= (moveRate * timeDelta);

            Matrix rotationMatrix;
            Matrix.CreateRotationY(MathHelper.ToRadians(cameraYaw), out rotationMatrix);

            if (movement != Vector3.Zero)
            {
                Vector3.Transform(ref movement, ref rotationMatrix, out movement);
                cameraPosition += movement;
            }

            //add in pitch to the rotation
            rotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(cameraPitch)) *
                rotationMatrix;

            // Create a vector pointing the direction the camera is facing.
            Vector3 transformedReference;
            Vector3.Transform(ref cameraReference, ref rotationMatrix,
                out transformedReference);
            // Calculate the position the camera is looking at.
            Vector3.Add(ref cameraPosition, ref transformedReference, out cameraTarget);

            Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector,
                out view);


            base.Update(gameTime);
        }

        public Matrix View
        {
            get { return view; }
        }

        public Matrix Projection
        {
            get { return projection; }
        }

        public PlayerIndex PlayerIndex
        {
            get { return ((PlayerIndex)playerIndex); }
            set { playerIndex = (int)value; }
        }

        public Vector3 Position
        {
            get { return (cameraPosition); }
            set { cameraPosition = value; }
        }

        public Vector3 Orientation
        {
            get { return (cameraReference); }
            set { cameraReference = value; }
        }

        public Vector3 Target
        {
            get { return (cameraTarget); }
            set { cameraTarget = value; }
        }
        public Viewport Viewport
        {
            get
            {
                if (viewport == null)
                    viewport = Game.GraphicsDevice.Viewport;

                return ((Viewport)viewport);
            }
            set
            {
                viewport = value;
                InitializeCamera();
            }
        }
    }
}