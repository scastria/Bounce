using System;
using Box2D.Common;
using Box2D.Dynamics;
using CocosSharp;

namespace Bounce.Controls
{
	public class CCPhysicsSprite : CCSprite
	{
		private const float EPSILON = 0.000001f;

		public b2Body PhysBody { get; private set; }

		public CCPhysicsSprite(CCTexture2D fileName,b2Body physBody) : base(fileName)
		{
			PhysBody = physBody;
		}

		public void UpdateBodyTransform()
		{
			b2Vec2 pos = PhysBody.Position;

			float x = pos.x * App.PTM_RATIO;
			float y = pos.y * App.PTM_RATIO;

			if (IgnoreAnchorPointForPosition) {
				x += AnchorPointInPoints.X * App.PTM_RATIO;
				y += AnchorPointInPoints.Y * App.PTM_RATIO;
			}

			// Make matrix
			float radians = PhysBody.Angle;
			if (Math.Abs(radians) > EPSILON)
				Rotation = CCMathHelper.ToDegrees(-radians);

			PositionX = x;
			PositionY = y;
		}
	}
}
