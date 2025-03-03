using MoonWorks.Graphics;
using MoonWorks;
using Snake.Content;
using Snake.GameStates;

namespace Snake
{
	public class SnakeGame : Game
	{
		LoadState LoadState;
		LogoState LogoState;
		CreditsState CreditsState;
		GameplayState GameplayState;
		TitleState TitleState;

		GameState CurrentState;

		public SnakeGame(
			AppInfo appInfo,
			WindowCreateInfo windowCreateInfo,
			FramePacingSettings framePacingSettings,
			ShaderFormat shaderFormats,
			bool debugMode
		) : base(appInfo, windowCreateInfo, framePacingSettings, shaderFormats, debugMode)
		{
			Inputs.Mouse.Hide();

			TextureAtlases.Init(GraphicsDevice);
			StaticAudioPacks.Init(AudioDevice);
			StreamingAudio.Init(AudioDevice);
			Fonts.LoadAll(GraphicsDevice, RootTitleStorage);

			CreditsState = new CreditsState(this, TitleState);
			LogoState = new LogoState(this, CreditsState, TitleState);
			TitleState = new TitleState(this, LogoState, GameplayState);
			LoadState = new LoadState(this, LogoState);
			CreditsState.SetTransitionState(TitleState); // i hate this

			GameplayState = new GameplayState(this, TitleState);
			TitleState.SetTransitionStateB(GameplayState);

			SetState(LoadState);
		}

		protected override void Update(System.TimeSpan dt)
		{
			if (Inputs.Keyboard.IsPressed(MoonWorks.Input.KeyCode.F11))
			{
				if (MainWindow.ScreenMode == ScreenMode.Fullscreen)
					MainWindow.SetScreenMode(ScreenMode.Windowed);
				else
					MainWindow.SetScreenMode(ScreenMode.Fullscreen);

			}

			CurrentState.Update(dt);
		}

		protected override void Draw(double alpha)
		{
			CurrentState.Draw(MainWindow, alpha);
		}

		protected override void Destroy()
		{

		}

		public void SetState(GameState gameState)
		{
			if (CurrentState != null)
			{
				CurrentState.End();
			}

			gameState.Start();
			CurrentState = gameState;
		}
	}
}
