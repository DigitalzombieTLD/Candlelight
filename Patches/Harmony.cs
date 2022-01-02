﻿using System;
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
	/*[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ItemInHandsPlaceable")]
	public class itemInHandPLacementPatcher
	{
		public static void Postfix(ref PlayerManager __instance, ref bool __result)
		{
			MelonLogger.Msg("postfix fired - original result" + __result);

			if (GameManager.GetPlayerInVehicle().IsInside())
			{
				__result = true;
			}
		}
	}*/

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
						GearItem gearItem = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_FatRaw", false);					
						
						if(!msgAdded)
						{
							msgAdded = true;
							string message = string.Concat(new object[]	{ gearItem.m_DisplayName,	" (", fatReward, ")"});
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
					__result = true;
				}	
			}
		}
	}

	[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ProcessPickupItemInteraction")]
	public class candleTurnOffOnStow
	{
		public static bool Prefix(ref GearItem item, ref bool forceEquip, ref bool skipAudio)
		{
			if (item.name.Contains("GEAR_Candle"))
			{
				CandleItem candleComponent = item.gameObject.GetComponent<CandleItem>();

				if (candleComponent)
				{
					candleComponent.turnOff();
				}
			}

			return true;
		}
	}

	/*[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "EnterInspectGearMode")]
	public class candleTurnOffOnInspect
	{
		public static bool Prefix(ref GearItem item)
		{
			if (item.name.Contains("GEAR_Candle"))
			{
				CandleItem candleComponent = item.gameObject.GetComponent<CandleItem>();

				if (candleComponent)
				{
					candleComponent.fakeOff();
				}
			}

			return true;
		}
	}*/

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

	[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessInteraction")]
	public class ExecuteInteractActionCardGame
	{
		public static bool Prefix(ref PlayerManager __instance)
		{
			if (__instance.m_InteractiveObjectUnderCrosshair != null && __instance.m_InteractiveObjectUnderCrosshair.name.Contains("GEAR_Candle") && __instance.m_ItemInHands)
			{
				CandleItem thisCandle = __instance.m_InteractiveObjectUnderCrosshair.gameObject.GetComponent<CandleItem>();

				if(__instance.m_ItemInHands.m_MatchesItem || __instance.m_ItemInHands.m_FlareItem)
				{
					CandleAction.ExecuteCustomPrimaryAction(thisCandle);
					return false;
				}			
			}       
			return true;
		}
	}

	/*[HarmonyLib.HarmonyPatch(typeof(PlayerAnimation), "OnAnimationEvent_Generic_IgniteComplete")]
	public class onAfterMatchIgnition
	{
		private static void Postfix(ref PlayerAnimation __instance)
		{
			MelonLogger.Msg("ignite complete!");

			if (CandleAction.currentPlayerManager.m_InteractiveObjectUnderCrosshair != null && CandleAction.currentPlayerManager.m_InteractiveObjectUnderCrosshair.name.Contains("GEAR_Candle"))
			{
				CandleItem candleComponent = CandleAction.currentPlayerManager.m_InteractiveObjectUnderCrosshair.gameObject.GetComponent<CandleItem>();

				candleComponent.turnOn();
			}
		}
	}*/
}