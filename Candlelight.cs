using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection; 
using System.Collections;
using Il2Cpp;
using Ini.Parser;

namespace Candlelight
{
	public class Candlelight_Main : MelonMod
	{
        public static Color candleLightColor = new Color(0.72f, 0.46f, 0.25f);
        public static Color candleFlameColor = new Color(0f, 0f, 0f);
        public int layerMask = 0;
        public static RaycastHit hit;
        
        public override void OnInitializeMelon()
        {
            Candlelight.Settings.OnLoad();
            layerMask |= 1 << 17; // gear layer            
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName.Contains("MainMenu"))
            {
                SaveLoad.reloadPending = true;
            }

            if (sceneName.Contains("SANDBOX"))
            {
                SaveLoad.LoadTheCandles();
            }
        }

        public override void OnUpdate()
        {           
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.interactButton) || InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.interactButton2))
            {
                if (Physics.Raycast(GameManager.GetMainCamera().transform.position, GameManager.GetMainCamera().transform.TransformDirection(Vector3.forward), out hit, 2.5f, layerMask))
                {
                    GameObject hitObject = hit.collider.gameObject;
                    string hitObjectName = hitObject.name;
                    
                    if (hitObjectName.Contains("GEAR_Candle"))
                    {
                        CandleItem thisCandle;

                        thisCandle = hitObject.transform.GetComponent<CandleItem>();

                        if (thisCandle != null)
                        {        
                            PlayerManager currentPlayerManager = GameManager.GetPlayerManagerComponent();

                            if (thisCandle.isLit)
                            {                               
                                if (currentPlayerManager.m_ItemInHands && currentPlayerManager.m_ItemInHands.m_TorchItem && !currentPlayerManager.m_ItemInHands.IsLitTorch())
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
                                    if (currentPlayerManager.m_ItemInHands.IsLitMatch() || currentPlayerManager.m_ItemInHands.IsLitFlare() || currentPlayerManager.m_ItemInHands.IsLitTorch())
                                    {
                                        thisCandle.turnOn(false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}