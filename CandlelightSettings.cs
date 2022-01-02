using UnityEngine;
using ModSettings;

namespace Candlelight
{
    internal class CandlelightSettingsMain : JsonModSettings
    {     

		[Section("General")]		

		[Name("Light intensity")]
		[Description("Non-Flicker only. Default: 3")]
		[Slider(0f, 6f)]
		public float lightIntensity = 3f;

		[Name("Light range")]
		[Description("Default: 6")]
		[Slider(0f, 12f)]
		public float lightRange = 6f;

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
		}
    }
}
