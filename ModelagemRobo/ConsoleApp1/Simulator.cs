using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Utilities;

namespace ConsoleApp1
{
    class Simulator : Microsoft.Xna.Framework.Game
    {

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _batch;
        private KeyboardState _oldKeyState;
        private SpriteFont _font;

        private readonly World _world;
        private Texture2D _backgroundImage;

        private Body _ballBody;
        private Texture2D _ballSprite;
        private Vector2 _ballOrigin;

        private Vector2[] _borderBodySizes;
        private Body[] _borderBodies;
        private Texture2D _borderSprite;
        private Vector2 _borderOrigin;

        private Body[] _robotsBodiesA;
        private Body[] _robotsBodiesB;
        private Texture2D _robotSprite;
        private Vector2 _robotOrigin;

        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;

        private Vector2D _fieldSize, _windowSize;
        private Environment _env;
        private float _winScale;
        public Simulator()
        {
            _fieldSize = Vector2D.Create(1.7f, 1.3f);
            _winScale = 1000.0f / (float)_fieldSize.y;
            _windowSize = _fieldSize.Mult(_winScale);
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = (int)_windowSize.x+200; // 1.7m
            _graphics.PreferredBackBufferHeight = (int)_windowSize.y; // 1.3m
            IsMouseVisible = true;

            Content.RootDirectory = "Content";

            //Create a world with gravity.
            _world = new World(Vector2.Zero);
            _env = new Environment();
            _robotsBodiesA = new Body[_env.teamA.robots.Length];
            _robotsBodiesB = new Body[_env.teamB.robots.Length];
            _borderBodies = new Body[8];
            _borderBodySizes = new Vector2[8];
        }
        protected override void LoadContent()
        {
            // Initialize camera controls
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(_windowSize.x / 2f, _windowSize.y / 2f);
            _batch = new SpriteBatch(_graphics.GraphicsDevice);

            _font = Content.Load<SpriteFont>("font");

            // Load sprites
            using (System.IO.FileStream fileStream = new System.IO.FileStream("Content/RobotSprite.png", System.IO.FileMode.Open))
            {
                _robotSprite = Texture2D.FromStream(GraphicsDevice, fileStream);
            }
            using (System.IO.FileStream fileStream = new System.IO.FileStream("Content/BallSprite.png", System.IO.FileMode.Open))
            {
                _ballSprite = Texture2D.FromStream(GraphicsDevice, fileStream);
            }
            using (System.IO.FileStream fileStream = new System.IO.FileStream("Content/BorderSprite.png", System.IO.FileMode.Open))
            {
                _borderSprite = Texture2D.FromStream(GraphicsDevice, fileStream);
            }
            using (System.IO.FileStream fileStream = new System.IO.FileStream("Content/background.png", System.IO.FileMode.Open))
            {
                _backgroundImage = Texture2D.FromStream(GraphicsDevice, fileStream);
            }

            _ballOrigin = new Vector2(_ballSprite.Width / 2f, _ballSprite.Height / 2f);
            _robotOrigin = new Vector2(_robotSprite.Width / 2f, _robotSprite.Height / 2f);
            _borderOrigin = new Vector2(_borderSprite.Width / 2f, _borderSprite.Height / 2f);

            // 1 meters equals 615 pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(_winScale);

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = ConvertUnits.ToSimUnits(_screenCenter);
            _ballBody = BodyFactory.CreateCircle(_world, (float)Team.ball_diameter / 2f, 0.55f, circlePosition, BodyType.Dynamic);

            // Give it some bounce and friction
            _ballBody.Restitution = 0.8f;
            _ballBody.Friction = 0.20f;
            Random rand = new Random();
            for (int i = 0; i < _robotsBodiesA.Length; i++)
            {
                float y_noise = (-0.5f + rand.Next(50) / 50f) * 0.01f;
                Vector2 robotPosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2((float)((i + 0.8) * _fieldSize.x / (2 * 4)), y_noise);
                _robotsBodiesA[i] = BodyFactory.CreateRectangle(_world, (float)Team.wheels_distance, (float)Team.wheels_distance, 1f, robotPosition, 0, BodyType.Dynamic);
                _robotsBodiesA[i].Restitution = 0.9f;
                _robotsBodiesA[i].Friction = 0.03f;
            }
            for (int i = 0; i < _robotsBodiesB.Length; i++)
            {
                float y_noise = (-0.5f + rand.Next(50) / 50f) * 0.1f;
                Vector2 robotPosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(-(float)((i + 0.8) * _fieldSize.x / (2 * 4)), y_noise);
                _robotsBodiesB[i] = BodyFactory.CreateRectangle(_world, (float)Team.wheels_distance, (float)Team.wheels_distance, 1f, robotPosition, 0, BodyType.Dynamic);
                _robotsBodiesB[i].Restitution = 0.9f;
                _robotsBodiesB[i].Friction = 0.03f;
            }
            Vector2 borderPosition;
            borderPosition = new Vector2(_fieldSize.x / 2f, -0.05f);
            _borderBodies[0] = BodyFactory.CreateRectangle(_world, _fieldSize.x + 0.1f, 0.1f, 1f, borderPosition, 0, BodyType.Static);//TOP
            _borderBodySizes[0] = new Vector2(_fieldSize.x + 0.1f, 0.1f);
            borderPosition = new Vector2(_fieldSize.x / 2f, _fieldSize.y + 0.05f);
            _borderBodies[1] = BodyFactory.CreateRectangle(_world, _fieldSize.x + 0.1f, 0.1f, 1f, borderPosition, 0, BodyType.Static);//BOTTOM
            _borderBodySizes[1] = new Vector2(_fieldSize.x + 0.1f, 0.1f);
            borderPosition = new Vector2(0.05f, 0.45f / 2f);
            _borderBodies[2] = BodyFactory.CreateRectangle(_world, 0.1f, 0.45f, 1f, borderPosition, 0, BodyType.Static);//LEFT-TOP
            _borderBodySizes[2] = new Vector2(0.1f, 0.45f);
            borderPosition = new Vector2(0.05f, _fieldSize.y - 0.45f / 2f);
            _borderBodies[3] = BodyFactory.CreateRectangle(_world, 0.1f, 0.45f, 1f, borderPosition, 0, BodyType.Static);//LEFT-BOTTOM
            _borderBodySizes[3] = new Vector2(0.1f, 0.45f);
            borderPosition = new Vector2(_fieldSize.x - 0.05f, 0.45f / 2f);
            _borderBodies[4] = BodyFactory.CreateRectangle(_world, 0.1f, 0.45f, 1f, borderPosition, 0, BodyType.Static);//RIGHT-TOP
            _borderBodySizes[4] = new Vector2(0.1f, 0.45f);
            borderPosition = new Vector2(_fieldSize.x - 0.05f, _fieldSize.y - 0.45f / 2f);
            _borderBodies[5] = BodyFactory.CreateRectangle(_world, 0.1f, 0.45f, 1f, borderPosition, 0, BodyType.Static);//RIGHT-BOTTOM
            _borderBodySizes[5] = new Vector2(0.1f, 0.45f);
            borderPosition = new Vector2(-0.05f, _fieldSize.y / 2f);
            _borderBodies[6] = BodyFactory.CreateRectangle(_world, 0.1f, _fieldSize.y, 1f, borderPosition, 0, BodyType.Static);//LEFT-LEFT
            _borderBodySizes[6] = new Vector2(0.1f, _fieldSize.y);
            borderPosition = new Vector2(_fieldSize.x + 0.05f, _fieldSize.y / 2f);
            _borderBodies[7] = BodyFactory.CreateRectangle(_world, 0.1f, _fieldSize.y, 1f, borderPosition, 0, BodyType.Static);//RIGHT-RIGHT
            _borderBodySizes[7] = new Vector2(0.1f, _fieldSize.y);
            foreach (var body in _borderBodies)
            {
                body.Friction = 0.15f;
            }
        }
        protected override void Update(GameTime gameTime)
        {
            //HandleKeyboard();
            for (int i = 0; i < _robotsBodiesA.Length; i++)
            {
                Vector2D orientation = Vector2D.Create(Math.Cos(_robotsBodiesA[i].Rotation), Math.Sin(_robotsBodiesA[i].Rotation)).Mult(_env.teamA.robots[i].linear_velocity());
                _robotsBodiesA[i].LinearVelocity = new Vector2((float)orientation.x, (float)orientation.y);
                _robotsBodiesA[i].AngularVelocity = (float)(_env.teamA.robots[i].angular_velocity());
                _env.teamA.robots[i].pos.x = _robotsBodiesA[i].Position.X;
                _env.teamA.robots[i].pos.y = _robotsBodiesA[i].Position.Y;
                _env.teamA.robots[i].orientation = _robotsBodiesA[i].Rotation;
            }
            for (int i = 0; i < _robotsBodiesB.Length; i++)
            {
                Vector2D orientation = Vector2D.Create(Math.Cos(_robotsBodiesB[i].Rotation), Math.Sin(_robotsBodiesB[i].Rotation)).Mult(_env.teamB.robots[i].linear_velocity());
                _robotsBodiesB[i].LinearVelocity = new Vector2((float)orientation.x, (float)orientation.y);
                _robotsBodiesB[i].AngularVelocity = (float)(_env.teamB.robots[i].angular_velocity());
                _env.teamB.robots[i].pos.x = _robotsBodiesB[i].Position.X;
                _env.teamB.robots[i].pos.y = _robotsBodiesB[i].Position.Y;
                _env.teamB.robots[i].orientation = _robotsBodiesB[i].Rotation;
            }
            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            _env.ballPos.x = _ballBody.Position.X;
            _env.ballPos.y = _ballBody.Position.Y;

            base.Update(gameTime);
        }
        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();
            /**
            // Move camera
            if (state.IsKeyDown(Keys.Left))
                _cameraPosition.X += 1.5f;

            if (state.IsKeyDown(Keys.Right))
                _cameraPosition.X -= 1.5f;

            if (state.IsKeyDown(Keys.Up))
                _cameraPosition.Y += 1.5f;

            if (state.IsKeyDown(Keys.Down))
                _cameraPosition.Y -= 1.5f;

            _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));

            // We make it possible to rotate the circle body
            if (state.IsKeyDown(Keys.A))
                _ballBody.ApplyTorque(-10);

            if (state.IsKeyDown(Keys.D) && _oldKeyState.IsKeyUp(Keys.D))
                _ballBody.ApplyLinearImpulse(new Vector2(0, 0.001f));

            if (state.IsKeyDown(Keys.Space) && _oldKeyState.IsKeyUp(Keys.Space))
                _ballBody.ApplyLinearImpulse(new Vector2(0, -0.001f));
*/
            if (state.IsKeyDown(Keys.Escape))
                Exit();

            _oldKeyState = state;
        }
        float Conv(float v)
        {
            return ConvertUnits.ToDisplayUnits(v);
        }
        int Convi(float v)
        {
            return (int)ConvertUnits.ToDisplayUnits(v);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            _batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
            _batch.Draw(_backgroundImage, new Rectangle(0, 0, (int)_windowSize.x, (int)_windowSize.y), Color.White);

            Color[] pixel = { Color.White };
            Texture2D white_pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            white_pixel.SetData(pixel);
            _batch.Draw(white_pixel, new Rectangle() { X = Convi(_fieldSize.x) / 2 - 1, Y = 0, Width = 3, Height = Convi(_fieldSize.y) }, Color.White);
            _batch.Draw(white_pixel, new Rectangle() { X = Convi(0.1f), Y = Convi(0.3f) - 1, Width = Convi(0.15f), Height = 3 }, Color.White);
            _batch.Draw(white_pixel, new Rectangle() { X = Convi(0.1f), Y = Convi(_fieldSize.y - 0.3f) - 2, Width = Convi(0.15f), Height = 3 }, Color.White);
            _batch.Draw(white_pixel, new Rectangle() { X = Convi(0.25f) - 1, Y = Convi(0.3f) - 1, Width = 3, Height = Convi(0.7f) }, Color.White);
            _batch.Draw(white_pixel, new Rectangle() { X = Convi(_fieldSize.x - 0.2f), Y = Convi(0.3f) - 1, Width = Convi(0.15f), Height = 3 }, Color.White);
            _batch.Draw(white_pixel, new Rectangle() { X = Convi(_fieldSize.x - 0.2f), Y = Convi(_fieldSize.y - 0.3f) - 2, Width = Convi(0.15f), Height = 3 }, Color.White);
            _batch.Draw(white_pixel, new Rectangle() { X = Convi(_fieldSize.x - 0.2f) - 1, Y = Convi(0.3f) - 1, Width = 3, Height = Convi(0.7f) }, Color.White);


            _batch.Draw(_ballSprite, ConvertUnits.ToDisplayUnits(_ballBody.Position), null, Color.White, _ballBody.Rotation, _ballOrigin, ((float)Team.ball_diameter * _winScale) / (float)_ballSprite.Height, SpriteEffects.None, 0f);
            //_batch.Draw(_groundSprite, ConvertUnits.ToDisplayUnits(_groundBody.Position), null, Color.White, 0f, _groundOrigin, 1f, SpriteEffects.None, 0f);
            for (int i = 0; i < _robotsBodiesA.Length; i++)
            {
                _batch.Draw(_robotSprite, ConvertUnits.ToDisplayUnits(_robotsBodiesA[i].Position), null, Color.Blue, _robotsBodiesA[i].Rotation, _robotOrigin, ((float)Team.wheels_distance * _winScale) / (float)_robotSprite.Height, SpriteEffects.None, 0f);
                _batch.Draw(_robotSprite, ConvertUnits.ToDisplayUnits(_robotsBodiesB[i].Position), null, Color.Red,  _robotsBodiesB[i].Rotation, _robotOrigin, ((float)Team.wheels_distance * _winScale) / (float)_robotSprite.Height, SpriteEffects.None, 0f);
            }
            for (int i = 2; i < _borderBodies.Length - 2; i++)
            {
                Vector2D center = Vector2D.Create(ConvertUnits.ToDisplayUnits(_borderBodies[i].Position));
                Vector2D size = Vector2D.Create(ConvertUnits.ToDisplayUnits(_borderBodySizes[i]));
                Vector2D left_top = center.Sub(size.Mult(.5f));
                Rectangle destRect = new Rectangle(new Point((int)left_top.x, (int)left_top.y), new Point((int)size.x, (int)size.y));
                _batch.Draw(_borderSprite, destRect, Color.White);
            }
            _batch.End();
            base.Draw(gameTime);
        }

    }
}
