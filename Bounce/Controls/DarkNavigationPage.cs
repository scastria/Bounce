using System;
using Xamarin.Forms;

namespace Bounce.Controls
{
	public class DarkNavigationPage : NavigationPage
	{
		public DarkNavigationPage(Page root) : base(root)
		{
			BarBackgroundColor = Color.Black;
			BarTextColor = Color.White;
		}
	}
}
