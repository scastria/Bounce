using System;
using System.Collections.Generic;
using System.IO;
using CocosSharp;
using Xamarin.Forms;

namespace Bounce.Pages
{
	public class PlayPage : ContentPage
	{
		private App App { get { return ((App)Application.Current); } }

		private CCGameView _nativeGameV = null;
		private CocosSharpView _formsGameV = null;

		public PlayPage()
		{
			Title = "Play";
			_formsGameV = new CocosSharpView {
				ViewCreated = LoadGame
			};
			Content = _formsGameV;
		}

		protected override void OnDisappearing()
		{
			if (_formsGameV != null)
				_formsGameV.Paused = true;
			base.OnDisappearing();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (_formsGameV != null)
				_formsGameV.Paused = false;
		}

		private async void LoadGame(object sender, EventArgs ea)
		{
			_nativeGameV = sender as CCGameView;

			if (_nativeGameV != null) {
				//_nativeGameV.DesignResolution = _nativeGameV.ViewSize;
				//_nativeGameV.ResolutionPolicy = CCViewResolutionPolicy.ExactFit;
				_nativeGameV.DesignResolution = new CCSizeI(1000, 1620);
				_nativeGameV.ResolutionPolicy = CCViewResolutionPolicy.ShowAll;
				_nativeGameV.ContentManager.SearchPaths = new List<string> { "Fonts", "Sounds", "Images", "Animations" };
				CCScene gameScene = new CCScene(_nativeGameV);
				gameScene.AddLayer(new Game.GameLayer(App.BallFilename,_nativeGameV.DesignResolution.Width, _nativeGameV.DesignResolution.Height));
				_nativeGameV.RunWithScene(gameScene);
			}
		}
	}
}
