using MelonLoader;
using UnityEngine;
using UnhollowerRuntimeLib;

namespace Candlelight
{
    public class Candlelight_Main : MelonMod
    {
		public static Color candleLightColor = new Color(0.72f, 0.46f, 0.25f);
		public static Color candleFlameColor = new Color(0f, 0f, 0f);

		public override void OnApplicationStart()
        {
			ClassInjector.RegisterTypeInIl2Cpp<CandleItem>();
			ClassInjector.RegisterTypeInIl2Cpp<CandleAction>();

			Candlelight.Settings.OnLoad();
		}
	}
}
