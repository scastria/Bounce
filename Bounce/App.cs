using System;
using Bounce.Controls;
using Bounce.Pages;
using CocosSharp;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Push;
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
			CCLog.Logger = Console.WriteLine;
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

        protected override void OnStart()
        {
            base.OnStart();
			MobileCenter.Start("ios=2d4da22f-0d7b-4921-a7ae-785c7e0b0c31;" +
				   "uwp={Your UWP App secret here};" +
				   "android=344ec95f-9a3b-4342-bea3-d4314fd6a67c;",
				   typeof(Analytics), typeof(Crashes), typeof(Push));
        }
	}
}
