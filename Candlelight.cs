using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection; 
using System.Collections;
using Il2Cpp;

namespace Candlelight
{
	public class Candlelight_Main : MelonMod
	{
        public static Color candleLightColor = new Color(0.72f, 0.46f, 0.25f);
        public static Color candleFlameColor = new Color(0f, 0f, 0f);

        public override void OnApplicationStart()
        {
            ClassInjector.RegisterTypeInIl2Cpp<CandleItem>();
         
            Candlelight.Settings.OnLoad();
        }

        public override void OnUpdate()
        {
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.interactButton) || InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.interactButton2))
            {
                PlayerManager currentPlayerManager = GameManager.GetPlayerManagerComponent();
                GameObject targetObject = currentPlayerManager.GetInteractiveObjectUnderCrosshairs(2.5f);

                if (targetObject != null && targetObject.name.Contains("GEAR_Candle"))
                {
                    CandleItem thisCandle = targetObject.GetComponent<CandleItem>();

                    if (thisCandle.isLit)
                    {
                        if(currentPlayerManager.m_ItemInHands.m_TorchItem && !currentPlayerManager.m_ItemInHands.IsLitTorch())
                        {
                            currentPlayerManager.m_ItemInHands.m_TorchItem.Ignite();
                        }
                        else
                        {
                            thisCandle.turnOff();
                        }
                    }
                    else
                    {
                        if (currentPlayerManager.m_ItemInHands)
                        {
                            if(currentPlayerManager.m_ItemInHands.IsLitMatch() || currentPlayerManager.m_ItemInHands.IsLitFlare() || currentPlayerManager.m_ItemInHands.IsLitTorch())
                            {
                                thisCandle.turnOn();
                            }
                        }
                    }
                }
            }
        }
    }
}