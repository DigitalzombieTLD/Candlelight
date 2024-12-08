using MelonLoader;
using System;
using UnityEngine;
using Il2Cpp;
using UnityEngine.AddressableAssets;
using static Il2Cpp.PlayerManager;

namespace Candlelight
{
	[HarmonyLib.HarmonyPatch(typeof(GearItem), "Awake")]
	public class candleComponentPatcher
	{
		public static void Postfix(ref GearItem __instance)
		{
			if (__instance.name.Contains("GEAR_Candle"))
			{
                CandleItem candleComponent = __instance.gameObject.GetComponent<CandleItem>();

                if (candleComponent == null)
				{
					candleComponent = __instance.gameObject.AddComponent<CandleItem>();		
                }

                ObjectGuid candleGUID = __instance.gameObject.GetComponent<ObjectGuid>();

                if (candleGUID == null)
                {
                    candleGUID = __instance.gameObject.AddComponent<ObjectGuid>();
                }

                __instance.m_ObjectGuid = candleGUID;

                if (candleGUID != null)
                {
                    candleComponent.PID = candleGUID.GetPDID();

                    if (candleGUID.PDID == null)
                    {
                        candleGUID.MaybeRuntimeRegister();
                        candleComponent.PID = candleGUID.GetPDID();
                    }                    
                }
            }
		}
	}

    [HarmonyLib.HarmonyPatch(typeof(GearItem), "Deserialize")]
    public class candleDeserializePatcher
    {
        public static void Postfix(ref GearItem __instance, GearItemSaveDataProxy proxy)
        {            
            if (__instance.name.Contains("GEAR_Candle"))
            {
                CandleItem candleComponent = __instance.gameObject.GetComponent<CandleItem>();

                if (candleComponent != null)
                {             
                    ObjectGuid candleGUID = __instance.gameObject.GetComponent<ObjectGuid>();

                    if (candleGUID != null)
                    {
                        candleComponent.PID = candleGUID.GetPDID();

                        if (candleGUID.PDID == null)
                        {                            
                            candleGUID.MaybeRuntimeRegister();
                            candleComponent.PID = candleGUID.GetPDID();
                        }

                        candleComponent.burnTime = SaveLoad.GetBurnTime(candleComponent.PID);

                        if(Settings.options.endless)
                        {
                            candleComponent.tranformBody(SaveLoad.GetBodyState(candleComponent.PID));
                        }

                        if (SaveLoad.GetLitState(candleComponent.PID) == true)
                        {
                            candleComponent.turnOn(true);
                        }
                    }
                }
            }
        }
    }

	[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "AddItemToPlayerInventory")]
	public class candleTurnOffOnStow
	{
		public static void Prefix(ref PlayerManager __instance, ref GearItem gi, ref bool trackItemLooted, ref bool enableNotificationFlag)
		{
			if (gi.name.Contains("GEAR_Candle"))
			{
				CandleItem candleComponent = gi.gameObject.GetComponent<CandleItem>();

				if (candleComponent.isLit)
				{
                    candleComponent.turnOff();					
				}
			}
		}
	}

	[HarmonyLib.HarmonyPatch(typeof(GearItem), "Drop")]
	public class candleTurnOffOnDrop
	{
		public static bool Prefix(ref GearItem __instance, ref int numUnits, ref bool playSound, ref bool stickToFeet)
		{
			if (__instance.name.Contains("GEAR_Candle"))
			{
				CandleItem candleComponent = __instance.gameObject.GetComponent<CandleItem>();
				if (candleComponent)
				{
                    candleComponent.turnOff();
				}
			}

			return true;
		}
	}

    [HarmonyLib.HarmonyPatch(typeof(GearItem), "Clone")]
    public class candleTurnOffOnClone
    {
        public static void Prefix(ref GearItem __instance)
        {
            if (__instance.name.Contains("GEAR_Candle"))
            {
                CandleItem candleComponent = __instance.gameObject.GetComponent<CandleItem>();
                if (candleComponent)
                {
                    candleComponent.turnOff();
                }
            }
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(GearItem), "GetHoverText")]
    public class hoverTextPatch
    {
        public static void Postfix(ref GearItem __instance, ref string __result)
        {            
            if (Settings.options.showBurntimeHover && __instance.name.Contains("GEAR_Candle"))
            {
                if (Settings.options.endless)
                {
                    __result = __instance.DisplayName + " [∞]";
                }
                else
                {
                    CandleItem candleComponent = __instance.gameObject.GetComponent<CandleItem>();
                    if (candleComponent)
                    {
                        float remainingHours = Settings.options.burnTime - candleComponent.burnTime;

                        if(remainingHours <= 0.001f)
                        {
                            __result = __instance.DisplayName + " [Burned out]";
                        }
                        else
                        {
                            TimeSpan time = TimeSpan.FromHours(remainingHours);
                            string str = time.ToString(@"hh\:mm");
                            __result = __instance.DisplayName + "[" + str + "]";
                        }                        
                    }
                }                      
            }
        }
    }
     
    [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "GetGearPlacePoint")]
    public class PlacePointHotfixPatch
    {
        public static void Postfix(ref PlayerManager __instance, ref GearPlacePoint __result, ref GameObject go, ref Vector3 searchPos)
        {
            if(__instance.m_ObjectToPlace && __instance.m_ObjectToPlace.name.Contains("GEAR_Candle"))
            {
                __result = null;
            }
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(SaveGameSystem), nameof(SaveGameSystem.SaveSceneData))]
    public class SaveCandles
    {
        public static void Postfix(ref SlotData slot)
        {
            SaveLoad.SaveTheCandles();        
        }
    }
}