using AlternativeActionUtilities;
using System;
using MelonLoader;
using UnityEngine;
using System.Collections;

namespace Candlelight
{
	public class CandleAction : AlternativeAction
	{
		public CandleItem thisCandle;
		public static PlayerManager currentPlayerManager;
		public CandleAction(IntPtr intPtr) : base(intPtr) { }

		public void Awake()
		{
			if (!currentPlayerManager)
			{
				currentPlayerManager = GameManager.GetPlayerManagerComponent();
			}
		}

		public override void ExecuteTertiary()
		{
			if(!thisCandle)
			{
				thisCandle = this.gameObject.GetComponent<CandleItem>();
			}

			ExecuteCustomPrimaryAction(thisCandle);
		}

		public static void ExecuteCustomPrimaryAction(CandleItem candleItem)
		{		
			if (currentPlayerManager.m_ItemInHands)
			{
				if(candleItem.isLit)
				{
					candleItem.turnOff();
					return;
				}

				if (currentPlayerManager.m_ItemInHands.m_MatchesItem || currentPlayerManager.m_ItemInHands.m_FlareItem)
				{
					if ((currentPlayerManager.m_ItemInHands.IsLitMatch() || currentPlayerManager.m_ItemInHands.IsLitFlare()))
					{
						candleItem.turnOn();
						return;
					}
					else if (!currentPlayerManager.m_ItemInHands.IsLitMatch() && !currentPlayerManager.m_ItemInHands.m_FlareItem && !candleItem.isLit)
					{
						currentPlayerManager.m_ItemInHands.m_MatchesItem.IgniteAfterDelay();
						MelonCoroutines.Start(waitForMatchIgnition(candleItem));

						return;
					}
				}
			}

			if (candleItem.isLit)
			{
				candleItem.turnOff();
			}
		}

		public static IEnumerator waitForMatchIgnition(CandleItem candleToIgnite)
		{
			for(int x=0; x<=15; x++)
			{
				if(!candleToIgnite.isLit && candleToIgnite.canIgnite() && currentPlayerManager.m_ItemInHands)
				{
					if(currentPlayerManager.m_ItemInHands.IsLitMatch())
					{
						candleToIgnite.turnOn();						
					}
					else
					{
						yield return new WaitForSeconds(0.2f);
					}					
				}
			}			
		}
	}
}