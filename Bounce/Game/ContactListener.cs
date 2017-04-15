using System;
using Box2D.Collision;
using Box2D.Dynamics;
using Box2D.Dynamics.Contacts;

namespace Bounce.Game
{
	public class ContactListener : b2ContactListener
	{
		public b2Body Ball { get; set; }
		public b2Body Floor { get; set; }

		private GameLayer _layer = null;

		public ContactListener(GameLayer layer)
		{
			_layer = layer;
		}

		public override void BeginContact(b2Contact contact)
		{
			base.BeginContact(contact);

			b2Body bodyA = contact.GetFixtureA().Body;
			b2Body bodyB = contact.GetFixtureB().Body;
			if (((bodyA == Ball) || (bodyB == Ball)) && ((bodyA == Floor) || (bodyB == Floor)))
				_layer.RequestStopGame();
		}

		public override void PostSolve(b2Contact contact, ref b2ContactImpulse impulse)
		{
		}

		public override void PreSolve(b2Contact contact, b2Manifold oldManifold)
		{
		}
	}
}
