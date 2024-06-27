using UnityEngine;
using ModSettings;
using Il2CppMono;

namespace Candlelight
{
    internal class CandlelightSettingsMain : JsonModSettings
    {     

		[Section("General")]

        [Name("Burntime")]
        [Description("Burntime in hours")]
        [Slider(1, 48)]
        public float burnTime = 8f;

        [Name("Show burntime")]
        [Description("Show remaining burntime of candle in hover text")]
        public bool showBurntimeHover = true;

        [Name("Unlimited burntime")]
        [Description("Endless light")]
        public bool endless = false;

        [Name("Wind sensitivity")]
        [Description("Candle extinguishes if wind gets faster than set value - 0 candles stays always on")]
        [Slider(0f, 30f)]
        public float windSensitivity = 3f;

        [Name("Light intensity")]
		[Description("Non-Flicker only. Default: 3")]
		[Slider(0f, 6f)]
		public float lightIntensity = 3f;

		[Name("Light range")]
		[Description("Default: 6")]
		[Slider(0f, 12f)]
		public float lightRange = 6f;

        [Name("Interact button")]
        [Description("Button to ignite / extinguish candle")]
        public KeyCode interactButton = KeyCode.Mouse2;

        [Name("Alternative interact button")]
        [Description("Button to ignite / extinguish candle")]
        public KeyCode interactButton2 = KeyCode.Mouse2;

        [Section("Light color")]

		[Name("Red")]
		[Description("Default: 0.7")]
		[Slider(0, 1f)]
		public float colorLightRed = 0.72f;

		[Name("Green")]
		[Description("Default: 0.6")]
		[Slider(0, 1f)]
		public float colorLightGreen = 0.46f;

		[Name("Blue")]
		[Description("Default: 0.2")]
		[Slider(0, 1f)]
		public float colorLightBlue = 0.25f;

		[Section("Flame color")]

		[Name("Red")]
		[Description("Default: 1")]
		[Slider(0, 1f)]
		public float colorFlameRed = 1f;

		[Name("Green")]
		[Description("Default: 1")]
		[Slider(0, 1f)]
		public float colorFlameGreen = 1f;

		[Name("Blue")]
		[Description("Default: 1")]
		[Slider(0, 1f)]
		public float colorFlameBlue = 0.25f;

		[Section("Flicker")]

		[Name("Enable")]
		[Description("Enable / Disable flickering light effect")]
		public bool enableFlicker = true;

		[Name("Intensity")]
		[Description("Default: 3")]
		[Slider(0f, 6f)]
		public float flickerMaxIntensity = 3f;

		[Name("Speed")]
		[Description("Default: 10")]
		[Slider(0, 60)]
		public int flickerFPS = 10;

		protected override void OnConfirm()
        {
            base.OnConfirm();
			Candlelight_Main.candleLightColor = new Color(colorLightRed, colorLightGreen, colorLightBlue);
			Candlelight_Main.candleFlameColor = new Color(colorFlameRed, colorFlameGreen, colorFlameBlue);
			Candlelight_Main.currentBurntimeSetting = burnTime;
            Candlelight_Main.isEndless = endless;
        }
    }

    internal static class Settings
    {
        public static CandlelightSettingsMain options;

        public static void OnLoad()
        {
            options = new CandlelightSettingsMain();
            options.AddToModSettings("Candlelight Settings");
			Candlelight_Main.candleLightColor = new Color(Settings.options.colorLightRed, Settings.options.colorLightGreen, Settings.options.colorLightBlue);
			Candlelight_Main.candleFlameColor = new Color(Settings.options.colorFlameRed, Settings.options.colorFlameGreen, Settings.options.colorFlameBlue);

            Candlelight_Main.currentBurntimeSetting = Settings.options.burnTime;
            Candlelight_Main.isEndless = Settings.options.endless;
        }
    }
}
