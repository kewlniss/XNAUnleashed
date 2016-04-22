using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using XELibrary;

namespace TunnelVision
{
    public partial class TunnelVisionCamera : Camera
    {
        private float spinLeft = 0;
        private float spinRight = 0;
        private float spinDown = 0;
        private float spinUp = 0;

        private float spinLeftChange = 0;
        private float spinRightChange = 0;
        private float spinDownChange = 0;
        private float spinUpChange = 0;

        public TunnelVisionCamera(Game game) : base(game) { }

        public override void Update(GameTime gameTime)
        {
            if (!UpdateInput)
                return;

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (input.KeyboardState.IsKeyDown(Keys.Left))
                spinLeftChange += .1f;
            else
                spinLeftChange -= .1f;
            spinLeftChange = MathHelper.Clamp(spinLeftChange, 0, 1);
            spinLeft = spinLeftChange;
            if (input.GamePads[playerIndex].ThumbSticks.Left.X < 0)
                spinLeft = -input.GamePads[playerIndex].ThumbSticks.Left.X;
            if (spinLeft > 0)
                cameraYaw += (Utility.PowerCurve(spinLeft) * SpinRate *
                    timeDelta);

            if (input.KeyboardState.IsKeyDown(Keys.Right))
                spinRightChange += .1f;
            else
                spinRightChange -= .1f;
            spinRightChange = MathHelper.Clamp(spinRightChange, 0, 1);
            spinRight = spinRightChange;
            if (input.GamePads[playerIndex].ThumbSticks.Left.X > 0)
                spinRight = input.GamePads[playerIndex].ThumbSticks.Left.X;
            if (spinRight > 0)
                cameraYaw -= (Utility.PowerCurve(spinRight) * SpinRate *
                    timeDelta);

            if (input.KeyboardState.IsKeyDown(Keys.Down))
                spinDownChange += .1f;
            else
                spinDownChange -= .1f;
            spinDownChange = MathHelper.Clamp(spinDownChange, 0, 1);
            spinDown = spinDownChange;
            if (input.GamePads[playerIndex].ThumbSticks.Left.Y < 0)
                spinDown = -input.GamePads[playerIndex].ThumbSticks.Left.Y;
            if (spinDown > 0)
                cameraPitch -= (Utility.PowerCurve(spinDown) * SpinRate *
                    timeDelta);

            if (input.KeyboardState.IsKeyDown(Keys.Up))
                spinUpChange += .1f;
            else
                spinUpChange -= .1f;
            spinUpChange = MathHelper.Clamp(spinUpChange, 0, 1);
            spinUp = spinUpChange;
            if (input.GamePads[playerIndex].ThumbSticks.Left.Y > 0)
                spinUp = input.GamePads[playerIndex].ThumbSticks.Left.Y;
            if (spinUp > 0)
                cameraPitch += (Utility.PowerCurve(spinUp) * SpinRate *
                    timeDelta);

            //reset camera angle if needed
            if (cameraYaw > 80)
                cameraYaw = 80;
            else if (cameraYaw < -80)
                cameraYaw = -80;

            //keep camera from rotating a full 90 degrees in either direction
            if (cameraPitch > 89)
                cameraPitch = 89;
            if (cameraPitch < -89)
                cameraPitch = -89;

            Matrix rotationMatrix;
            Vector3 transformedReference;

            Matrix.CreateRotationY(MathHelper.ToRadians(cameraYaw),
                out rotationMatrix);

            //add in pitch to the rotation
            rotationMatrix = Matrix.CreateRotationX(
                MathHelper.ToRadians(cameraPitch)) * rotationMatrix;

            // Create a vector pointing the direction the camera is facing.
            Vector3.Transform(ref cameraReference, ref rotationMatrix,
                out transformedReference);
            // Calculate the position the camera is looking at.
            Vector3.Add(ref cameraPosition, ref transformedReference,
                out cameraTarget);

            Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget,
                ref cameraUpVector, out view);
        }
    }
}
