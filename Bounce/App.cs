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

		public string BallFilename { get; set; }

		private MasterDetailPage _rootPage = null;
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
				//Create new PlayPage each time in case ball image has changed
				detailPage = new PlayPage();
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
