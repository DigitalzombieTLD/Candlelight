using MelonLoader;
using System;
using UnityEngine;
using Il2Cpp;
using UnityEngine.AddressableAssets;

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
                        //GearItem gearItem = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_FatRaw", false);					
                        GearItem gearItem = Addressables.LoadAssetAsync<GameObject>("GEAR_FatRaw").WaitForCompletion().GetComponent<GearItem>();
						GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory(gearItem, 1, -1f, false, false);

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

	[HarmonyLib.HarmonyPatch(typeof(GearItem), "IsLitMatch")]
	public class isLitPatcher
	{
		public static void Postfix(ref GearItem __instance, ref bool __result)
		{
			if (__instance.name.Contains("GEAR_Candle"))
			{
				if (__instance.m_BeenInPlayerInventory)
				{
					CandleItem thisCandle = __instance.gameObject.GetComponent<CandleItem>();

					if (thisCandle.isLit)
					{
						__result = true;
					}
				}	
			}
		}
	}

	
	[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ProcessPickupItemInteraction")]
	public class candleIdontEvenCareAnymore
	{
		public static bool Prefix(ref GearItem item, ref bool forceEquip, ref bool skipAudio)
		{
			if (item.name.Contains("GEAR_Candle"))
			{			
				CandleItem candleComponent = item.gameObject.GetComponent<CandleItem>();
				
				if (candleComponent.isLit)
				{
					candleComponent.turnOff();
					return false;
				}
				else
				{
					item.m_BeenInPlayerInventory = false;
				}
			}

			return true;
		}
	}

	//GearItem AddItemToPlayerInventory(GearItem gi, bool trackItemLooted = true, bool enableNotificationFlag = false)

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

	[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "StartPlaceMesh", new Type[] { typeof(GameObject), typeof(float), typeof(PlaceMeshFlags) })]
	public class turnOffWhileMoving
	{
		public static void Prefix(ref PlayerManager __instance, ref GameObject objectToPlace)
		{
			if (objectToPlace.name.Contains("GEAR_Candle"))
			{
				CandleItem candleComponent = objectToPlace.GetComponent<CandleItem>();
				candleComponent.fakeOff();
			}
		}
	}

	[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "PlaceMeshInWorld")]
	public class maybeTurnOnAgain
	{
		private static bool Prefix(ref PlayerManager __instance)
		{
			if (__instance.m_ObjectToPlace.name.Contains("GEAR_Candle"))
			{
				CandleItem candleComponent = __instance.m_ObjectToPlace.gameObject.GetComponent<CandleItem>();

				candleComponent.fakeOn();
			}
			return true;
		}
	}

	[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "CancelPlaceMesh")]
	public class maybeTurnOnAgainAfterCancel
	{
		private static bool Prefix(ref PlayerManager __instance)
		{
			if (__instance.m_ObjectToPlace.name.Contains("GEAR_Candle"))
			{
				CandleItem candleComponent = __instance.m_ObjectToPlace.gameObject.GetComponent<CandleItem>();

				candleComponent.fakeOn();
			}
			return true;
		}
	}
}