using System;
using MelonLoader;
using Harmony;
using UnityEngine;
using System.Reflection;
using System.Xml.XPath;
using System.Globalization;
using UnhollowerRuntimeLib;
using ModSettings;
using System.Collections;
using System.IO;
using System.Collections.Generic;

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

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			
		}

		public override void OnUpdate()
		{
			/*
			if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Keypad0))
			{				
				MelonLogger.Msg("Windspeed: " + GameManager.GetWindComponent().GetSpeedMPH());
			}
			*/
		}
	}
}
