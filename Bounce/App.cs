using System;
using Bounce.Controls;
using Bounce.Pages;
using Xamarin.Forms;

namespace Bounce
{
	public enum MenuItemType { Play, Balls }

	public class App : Application
	{
		public const int PTM_RATIO = 64;

		private MasterDetailPage _rootPage = null;
		private PlayPage _playPage = null;
		private BallsPage _ballsPage = null;

		public App()
		{
			_rootPage = new MasterDetailPage {
				Master = new DarkNavigationPage(new MenuPage()) { Title = "Menu", Icon = "menu" }
			};
			//Device.OnPlatform(iOS: delegate { _rootPage.IsGestureEnabled = false; });
			SetDetailPage(MenuItemType.Play);
			MainPage = _rootPage;
		}

		public void SetDetailPage(MenuItemType type)
		{
			NavigationPage detailNavPage = null;
			ContentPage detailPage = null;
			switch (type) {
			case MenuItemType.Play:
				if (_playPage == null)
					_playPage = new PlayPage();
				detailPage = _playPage;
				break;
			default:
				if (_ballsPage == null)
					_ballsPage = new BallsPage();
				detailPage = _ballsPage;
				break;
			}
			detailNavPage = new DarkNavigationPage(detailPage);
			_rootPage.Detail = detailNavPage;
			_rootPage.IsPresented = false;
		}
	}
}
