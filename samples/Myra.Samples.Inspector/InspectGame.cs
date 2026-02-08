using Myra.Graphics2D.UI;
using System;
using System.Linq;
using Myra.Graphics2D.UI.Styles;


#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#if ANDROID
#endif
#else
using System.Threading.Tasks;
using Stride.Engine;
using Stride.Games;
using Stride.Graphics;
using Stride.Core.Mathematics;
#endif

namespace Myra.Samples.Inspector
{
	public class InspectGame : Game
	{
#if !STRIDE
		private readonly GraphicsDeviceManager _graphics;
#endif

		private RootWidgets widgets;
		private Desktop _desktop;
		
		public static InspectGame Instance { get; private set; }

		public InspectGame()
		{
			Instance = this;

#if !STRIDE
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800
			};
			Window.AllowUserResizing = true;
#else
#endif

			IsMouseVisible = true;
		}

#if STRIDE
		protected override Task LoadContent()
		{
			MyraEnvironment.Game = this;

			_allWidgets = new Widgets();

			_desktop = new Desktop();
			_desktop.Widgets.Add(_allWidgets);

			return base.LoadContent();
		}
#else
		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;
			MyraEnvironment.EnableModalDarkening = true;

//			Stylesheet.Current = DefaultAssets.DefaultStylesheet2X;

			widgets = new RootWidgets();

			_desktop = new Desktop();
			_desktop.Root = widgets;

#if MONOGAME && !ANDROID
			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			_desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};
#endif
		}
#endif

		private int selectedIndex = 0;
		private bool lastDown, lastUp;
		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			var state = Keyboard.GetState();
			
			// prevent spam cycling by only triggering an input change up or down once
			bool thisDown = state[Keys.Down] == KeyState.Down;
			bool thisUp = state[Keys.Up] == KeyState.Down;
			if (thisDown & !lastDown)
				CycleSelection(false);
			else if(thisUp & !lastUp)
				CycleSelection(true);
			
			lastDown = thisDown;
			lastUp = thisUp;
		}

		private void CycleSelection(bool forward)
		{
			if (forward)
			{
				selectedIndex++;
				if (selectedIndex >= widgets.inspectables.Count)
					selectedIndex = 0;
			}
			else
			{
				selectedIndex--;
				if (selectedIndex < 0)
					selectedIndex = widgets.inspectables.Count - 1;
			}
			widgets.Inspect( widgets.inspectables[selectedIndex] );
		}
		
		private string TextDisplay
		{
			get
			{
				Type inspectedType = widgets.inspectables[selectedIndex].GetType();
				string baseType = string.Empty;
				if (inspectedType.BaseType != null)
				{
					baseType = $" BaseType: {inspectedType.BaseType.Name}";
				}
				
				return $"\nInspecting object [{selectedIndex+1}] of [{widgets.inspectables.Count}]:\n\n Type: {inspectedType.Name}\n in: {inspectedType.Namespace}\n{baseType}\n\n Assembly:\n{inspectedType.Assembly.GetName().Name}\n\n\n\n\n\n\nIs mouse over GUI: {_desktop.IsMouseOverGUI}";
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

#if !STRIDE
			GraphicsDevice.Clear(Color.Black);
#else
			// Clear screen
			GraphicsContext.CommandList.Clear(GraphicsDevice.Presenter.BackBuffer, Color.Black);
			GraphicsContext.CommandList.Clear(GraphicsDevice.Presenter.DepthStencilBuffer, DepthStencilClearOptions.DepthBuffer | DepthStencilClearOptions.Stencil);

			// Set render target
			GraphicsContext.CommandList.SetRenderTargetAndViewport(GraphicsDevice.Presenter.DepthStencilBuffer, GraphicsDevice.Presenter.BackBuffer);
#endif
			widgets.labelOverGui.Text = TextDisplay;
			_desktop.Render();
		}
	}
}