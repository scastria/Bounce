using System;
using System.Collections.Generic;
using Box2D.Collision.Shapes;
using Box2D.Common;
using Box2D.Dynamics;
using CocosSharp;
using Bounce.Controls;

namespace Bounce.Game
{
	public class GameLayer : CCLayerColor
	{
		private const string CURRENT_BEST = "Current Best";
		private const float CURRENT_BEST_X_PERCENT = 0.5f;
		private const float CURRENT_BEST_Y_PERCENT = 0.95f;
		private const int CURRENT_BEST_FONT_SIZE = 50;
		private const float SCORE_X_PERCENT = 0.5f;
		private const float SCORE_Y_PERCENT = 0.9f;
		private const int SCORE_FONT_SIZE = 150;
		private const int BALL_Y_MARGIN = 20;
		private const float BALL_X_PERCENT = 0.5f;
		private const float BALL_RADIUS_PERCENT = 0.33f;
		private const float WALL_FRICTION = 1f;
		private const float WALL_RECOIL = 0.1f;
		private const float BALL_FRICTION = 1f;
		private const float BALL_RECOIL = 0.4f;
		private const int FORCE_MAGNITUDE = 55;
		private const int GRAVITY = -130;
		//private const int FORCE_MAGNITUDE = 20;
		//private const int GRAVITY = -10;

		private int _score = 0;
		private int Score {
			get { return (_score); }
			set {
				_score = value;
				_scoreLN.Text = "" + value;
			}
		}

		private float _gameWidth;
		private float _gameHeight;
		private CCPhysicsSprite _ballN = null;
		private b2World _world = null;
		private Box2DDebug _debugDraw = null;
		private CCCustomCommand _renderDebugCommand = null;
		private bool _isGameOver = true;
		private b2Vec2 _gravity;
		private CCLabel _currentBestLN = null;
		private CCLabel _scoreLN = null;
		private int _currentBest = 0;
		private ContactListener _contactListener = null;
		private bool _stopGameRequested = false;

		public GameLayer(float gameWidth, float gameHeight) : base(new CCColor4B(249, 247, 233))
		{
			_gameWidth = gameWidth;
			_gameHeight = gameHeight;

			InitPhysics();

			AddScoring();

			AddBall();

			//StartDebugging();

			Schedule(Run);
		}

		private void InitPhysics()
		{
			_gravity = new b2Vec2(0.0f, GRAVITY);
			_world = new b2World(b2Vec2.Zero);

			_world.SetAllowSleeping(true);
			_world.SetContinuousPhysics(true);
			_contactListener = new ContactListener(this);
			_world.SetContactListener(_contactListener);

			var wallDef = new b2BodyDef();
			wallDef.allowSleep = true;
			wallDef.position = b2Vec2.Zero;
			wallDef.type = b2BodyType.b2_staticBody;

			//Floor
			b2Body floorBody = _world.CreateBody(wallDef);
			floorBody.SetActive(true);

			b2EdgeShape floorShape = new b2EdgeShape();
			floorShape.Set(b2Vec2.Zero, new b2Vec2(_gameWidth / App.PTM_RATIO,0));

			b2FixtureDef floorFixture = new b2FixtureDef();
			floorFixture.friction = WALL_FRICTION;
			floorFixture.restitution = WALL_RECOIL;
			floorFixture.shape = floorShape;

			floorBody.CreateFixture(floorFixture);
			_contactListener.Floor = floorBody;

			//Left Wall
			b2Body leftWallBody = _world.CreateBody(wallDef);
			leftWallBody.SetActive(true);

			b2EdgeShape leftWallShape = new b2EdgeShape();
			//Go up very high so skip PTM_RATIO
			leftWallShape.Set(b2Vec2.Zero, new b2Vec2(0, _gameHeight));

			b2FixtureDef leftWallFixture = new b2FixtureDef();
			leftWallFixture.friction = WALL_FRICTION;
			leftWallFixture.restitution = WALL_RECOIL;
			leftWallFixture.shape = leftWallShape;

			leftWallBody.CreateFixture(leftWallFixture);

			//Right Wall
			b2Body rightWallBody = _world.CreateBody(wallDef);
			rightWallBody.SetActive(true);

			b2EdgeShape rightWallShape = new b2EdgeShape();
			//Go up very high so skip PTM_RATIO
			rightWallShape.Set(new b2Vec2(_gameWidth / App.PTM_RATIO, 0), new b2Vec2(_gameWidth / App.PTM_RATIO, _gameHeight));

			b2FixtureDef rightWallFixture = new b2FixtureDef();
			rightWallFixture.friction = WALL_FRICTION;
			rightWallFixture.restitution = WALL_RECOIL;
			rightWallFixture.shape = rightWallShape;

			rightWallBody.CreateFixture(rightWallFixture);
		}

		protected override void VisitRenderer(ref CCAffineTransform worldTransform)
		{
			base.VisitRenderer(ref worldTransform);

			if ((_debugDraw != null) && (_debugDraw.Flags != 0))
				Renderer.AddCommand(_renderDebugCommand);
		}

		private void StartDebugging()
		{
			_renderDebugCommand = new CCCustomCommand(RenderDebug);
			var debugNode = new CCDrawNode();
			AddChild(debugNode, 1000);
			_debugDraw = new Box2DDebug(debugNode, App.PTM_RATIO);

			_debugDraw.Flags = b2DrawFlags.e_shapeBit | b2DrawFlags.e_aabbBit | b2DrawFlags.e_centerOfMassBit | b2DrawFlags.e_jointBit;
			_world.SetDebugDraw(_debugDraw);
		}

		private void RenderDebug()
		{
			if (_debugDraw != null) {
				_debugDraw.Begin();
				_world.DrawDebugData();
				_debugDraw.End();
			}
		}

		private void AddScoring()
		{
			//Current best
			float currentBestLabelX = _gameWidth * CURRENT_BEST_X_PERCENT;
			float currentBestLabelY = _gameHeight * CURRENT_BEST_Y_PERCENT;
			_currentBestLN = new CCLabel(CURRENT_BEST, "Roboto-Regular.ttf", CURRENT_BEST_FONT_SIZE, CCLabelFormat.SystemFont) {
				PositionX = currentBestLabelX,
				PositionY = currentBestLabelY,
				AnchorPoint = CCPoint.AnchorMiddleTop,
				Color = CCColor3B.Black
			};
			AddChild(_currentBestLN);
			//Score
			float scoreLabelX = _gameWidth * SCORE_X_PERCENT;
			float scoreLabelY = _gameHeight * SCORE_Y_PERCENT;
			_scoreLN = new CCLabel("" + _currentBest, "Roboto-Regular.ttf", SCORE_FONT_SIZE, CCLabelFormat.SystemFont) {
				PositionX = scoreLabelX,
				PositionY = scoreLabelY,
				AnchorPoint = CCPoint.AnchorMiddleTop,
				Color = CCColor3B.Black
			};
			AddChild(_scoreLN);
		}

		private void ResetBall()
		{
			_ballN.PhysBody.LinearVelocity = b2Vec2.Zero;
			_ballN.PhysBody.AngularVelocity = 0;
			float ballDiameter = _gameWidth * BALL_RADIUS_PERCENT;
			float ballCenterX = _gameWidth * BALL_X_PERCENT;
			float ballCenterY = ballDiameter / 2 + BALL_Y_MARGIN;
			_ballN.PhysBody.SetTransform(new b2Vec2(ballCenterX / App.PTM_RATIO, ballCenterY / App.PTM_RATIO), 0);
			_ballN.UpdateBodyTransform();
		}

		private void AddBall()
		{
			//Ball
			float ballDiameter = _gameWidth * BALL_RADIUS_PERCENT;
			float ballCenterX = _gameWidth * BALL_X_PERCENT;
			float ballCenterY = ballDiameter / 2 + BALL_Y_MARGIN;

			var def = new b2BodyDef();
			def.position = new b2Vec2(ballCenterX / App.PTM_RATIO, ballCenterY / App.PTM_RATIO);
			def.type = b2BodyType.b2_dynamicBody;
			b2Body physBody = _world.CreateBody(def);
			//Circle Physics Shape
			var shape = new b2CircleShape();
			shape.Radius = ballDiameter / 2 / App.PTM_RATIO;

			var fd = new b2FixtureDef();
			fd.shape = shape;
			fd.density = 1f;
			fd.restitution = BALL_RECOIL;
			fd.friction = BALL_FRICTION;
			physBody.CreateFixture(fd);
			_contactListener.Ball = physBody;

			//byte[] picData = Plugin.EmbeddedResource.ResourceLoader.GetEmbeddedResourceBytes(this.GetType().Assembly, "menu.png");
			CCTexture2D ballTex = new CCTexture2D("soccer");
			_ballN = new CCPhysicsSprite(ballTex, physBody) {
				PositionX = ballCenterX,
				PositionY = ballCenterY,
				ContentSize = new CCSize(ballDiameter, ballDiameter)
			};
			AddChild(_ballN);
		}

		private void Run(float time)
		{
			_world.Step(time, 8, 8);

			if (_stopGameRequested) {
				_stopGameRequested = false;
				StopGame();
			} else
				_ballN.UpdateBodyTransform();
		}

		protected override void AddedToScene()
		{
			base.AddedToScene();

			// Register for touch events
			var touchListener = new CCEventListenerTouchAllAtOnce();
			touchListener.OnTouchesBegan = OnTouchesBegan;
			AddEventListener(touchListener, this);
		}

		private void StartGame()
		{
			_isGameOver = false;
			_world.Gravity = _gravity;
			_currentBestLN.Visible = false;
			Score = 0;
		}

		public void RequestStopGame()
		{
			_stopGameRequested = true;
		}

		private void StopGame()
		{
			_isGameOver = true;
			_world.Gravity = b2Vec2.Zero;
			ResetBall();
			_currentBestLN.Visible = true;
			if (Score > _currentBest)
				_currentBest = Score;
			else
				Score = _currentBest;
		}

		private void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
		{
			if (touches.Count > 0) {
				float physTouchX = touches[0].Location.X / App.PTM_RATIO;
				float physTouchY = touches[0].Location.Y / App.PTM_RATIO;
				//Check touch ball
				bool hitBall = _ballN.PhysBody.FixtureList.TestPoint(new b2Vec2(physTouchX, physTouchY));
				//if (!hitBall) {
					//Console.WriteLine("Raw touch: (" + touches[0].Location.X + "," + touches[0].Location.Y + ")");
					//Console.WriteLine("Shape center: (" + _ballN.PositionX + "," + _ballN.PositionY + ")");
					//Console.WriteLine("Shape Radius: " + _ballN.ContentSize.Width / 2);
					//float squareDistance = (touches[0].Location.X - _ballN.PositionX) * (touches[0].Location.X - _ballN.PositionX) + (touches[0].Location.Y - _ballN.PositionY) * (touches[0].Location.Y - _ballN.PositionY);
					//float squareRadius = (_ballN.ContentSize.Width / 2) * (_ballN.ContentSize.Width / 2);
					//Console.WriteLine("Hit Shape: " + (squareDistance <= squareRadius));

					//Console.WriteLine("Physics touch: (" + physTouchX + "," + physTouchY + ")");
					//Console.WriteLine("Physics center: (" + _ballN.PhysBody.Position.x + "," + _ballN.PhysBody.Position.y + ")");
					//Console.WriteLine("Physics Radius: " + _ballN.PhysBody.FixtureList.Shape.Radius);
					//squareDistance = (physTouchX - _ballN.PhysBody.Position.x) * (physTouchX - _ballN.PhysBody.Position.x) + (physTouchY - _ballN.PhysBody.Position.y) * (physTouchY - _ballN.PhysBody.Position.y);
					//squareRadius = (_ballN.PhysBody.FixtureList.Shape.Radius) * (_ballN.PhysBody.FixtureList.Shape.Radius);
					//Console.WriteLine("Hit Physics: " + (squareDistance <= squareRadius));
				//}
				if (_isGameOver && hitBall)
					StartGame();
				if (hitBall) {
					float xDelta = _ballN.PhysBody.WorldCenter.x - physTouchX;
					float angle = (float)Math.Asin(xDelta / _ballN.PhysBody.FixtureList.Shape.Radius);
					float forceX = (float)Math.Sin(angle) * FORCE_MAGNITUDE;
					float forceY = (float)Math.Cos(angle) * FORCE_MAGNITUDE;
					_ballN.PhysBody.LinearVelocity = new b2Vec2(forceX, forceY);
					Score++;
				} else if(!_isGameOver) {
					//Prevent cheating by penalizing for not hitting ball
					if(Score != 0)
						Score--;
				}
			}
		}
	}
}
