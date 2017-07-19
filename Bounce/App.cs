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
	public enum MenuItemType { Play, Balls, Crash }

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
            MobileCenter.Start("ios=0bbdcbfa-f977-4d30-adb7-38763e20966d;" +
				   "uwp={Your UWP App secret here};" +
				   "android=a15c7693-2f2e-4457-a63e-65ccb7b65f29;",
				   typeof(Analytics), typeof(Crashes), typeof(Push));
        }
	}
}
