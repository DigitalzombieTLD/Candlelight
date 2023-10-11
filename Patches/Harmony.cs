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

    [HarmonyLib.HarmonyPatch(typeof(Panel_BodyHarvest), "TransferHideFromCarcassToInventory")]
	public class harvestFatPatcher
	{
		public static void Postfix(ref Panel_BodyHarvest __instance)
		{
			GearItem thisHide = __instance.m_BodyHarvest.m_HidePrefab.GetComponent<GearItem>();
			float harvestAmount = __instance.m_MenuItem_Hide.m_HarvestAmount;
			int fatReward = 0;
			bool msgAdded = false;

			if (harvestAmount>0)
			{				
				if (thisHide.name.Contains("GEAR_RabbitPelt"))
				{
					fatReward = 1;
				}
				else if (thisHide.name.Contains("GEAR_WolfPelt")|| thisHide.name.Contains("GEAR_LeatherHide"))
				{
					fatReward = 3;
				}
				else if (thisHide.name.Contains("GEAR_BearHide"))
				{
					fatReward = 5;
				}
				else if (thisHide.name.Contains("GEAR_MooseHide"))
				{
					fatReward = 7;
				}

				for (int x = 1; x <= harvestAmount; x++)
				{
					for(int y = 1; y<= fatReward; y++)
					{				
                        GearItem gearItem = Addressables.LoadAssetAsync<GameObject>("GEAR_FatRaw").WaitForCompletion().GetComponent<GearItem>();
						GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory(gearItem, 1, 1f, InventoryInstantiateFlags.None);
                    
						if (!msgAdded)
						{
							msgAdded = true;
							string message = string.Concat(new object[]	{ gearItem.DisplayName,	" (", fatReward, ")"});
							GearMessage.AddMessage(gearItem.name, Localization.Get("GAMEPLAY_Harvested"), message, false, true);
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
    
    [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "MaybeDisableInspectModeMesh")]
    public class InspectModeMeshDisablePatcher
    {
        public static bool Prefix(ref PlayerManager __instance, GearItem gi)
        {
            if (gi.name.Contains("GEAR_Candle"))
            {
                return false;
            }

            return true;
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "MaybeEnableInspectModeMesh")]
    public class InspectModeMeshEnablePatcher
    {
        public static bool Prefix(ref PlayerManager __instance, GearItem gi)
        {
            if (gi.name.Contains("GEAR_Candle"))
            {
                return false;
            }

            return true;
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