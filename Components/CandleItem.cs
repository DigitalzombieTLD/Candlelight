using System;
using MelonLoader;
using UnityEngine;
using System.Collections;
using Il2Cpp;
using Harmony;

namespace Candlelight
{
	[RegisterTypeInIl2Cpp]
	public class CandleItem : MonoBehaviour
	{
        public CandleItem(IntPtr intPtr) : base(intPtr)
        {
        }

        public float burnTime = 0;

        public GearItem candleGearItem;

        public Inspect inspectThingy;
        

        public List<GameObject> bodyObject = new List<GameObject>();
        public List<Material> bodyMaterial = new List<Material>();

        public List<GameObject> inspectObjects = new List<GameObject>();

        public List<GameObject> wickObject = new List<GameObject>();
        public List<Material> wickMaterial = new List<Material>();

        public List<GameObject> flameObject = new List<GameObject>();
        public List<Material> flameMaterial = new List<Material>();

        public List<GameObject> smokeObject = new List<GameObject>();
        public List<ParticleSystem> smokeParticle = new List<ParticleSystem>();
                

        public GameObject lightObject;
        public Light light;
        public HeatSource candleHeatComponent;
		public LightTracking candleLightTrackingComponent;
		public LightQualitySwitch candleLightQualitySwitch;
        public MissionIlluminationArea missionIlluminationArea;

        public string PID;
        
        public int flickerRandom = 30;
        public int flickerCounter = 0;
        public int currentBodyState = 0; // 0 = full
		       
		public string flickerPattern = "mmnmmommommnonmmonqnmmo";

		public bool isLit = false;						

		public void Awake()
		{
            //this.transform.parent = GearManager.m_GearCategory.transform;
            //this.transform.parent = GameObject.Find("Design/Gear").transform;

            candleGearItem = this.gameObject.GetComponent<GearItem>();
            inspectThingy = this.gameObject.GetComponent<Inspect>();

            lightObject = this.transform.Find("Lightsource").gameObject;
            light = lightObject.GetComponent<Light>();

            for (int candleStates = 0; candleStates < 4; candleStates++)
            {
                bodyObject.Add(this.transform.Find("CandleBody" + candleStates).gameObject);
                bodyMaterial.Add(bodyObject[candleStates].GetComponent<MeshRenderer>().material);

                inspectObjects.Add(this.transform.Find("CandleInspect" + candleStates).gameObject);

                wickObject.Add(bodyObject[candleStates].transform.Find("Wick").gameObject);
                wickMaterial.Add(wickObject[candleStates].GetComponent<MeshRenderer>().material);

                flameObject.Add(bodyObject[candleStates].transform.Find("Flame").gameObject);
                flameMaterial.Add(flameObject[candleStates].GetComponent<MeshRenderer>().material);

                smokeObject.Add(bodyObject[candleStates].transform.Find("smokePuff").gameObject);
                smokeParticle.Add(smokeObject[candleStates].GetComponent<ParticleSystem>());
            }
          
            inspectThingy.m_InspectModeMesh = inspectObjects[currentBodyState];
            inspectThingy.m_NormalMesh = bodyObject[currentBodyState];

            inspectThingy.m_InspectModeMesh.SetActive(false);
            inspectThingy.m_NormalMesh.SetActive(true);

            flickerRandom = UnityEngine.Random.Range(5, 50);
            light.intensity = Settings.options.lightIntensity;
            light.range = Settings.options.lightRange;

            candleLightTrackingComponent = lightObject.GetComponent<LightTracking>();

            if (candleLightTrackingComponent == null)
            {
                candleLightTrackingComponent = lightObject.AddComponent<LightTracking>();
                candleLightTrackingComponent.m_LightComponent = light;
            }

            candleLightQualitySwitch = lightObject.GetComponent<LightQualitySwitch>();

            if (candleLightQualitySwitch == null)
            {
                candleLightQualitySwitch = lightObject.AddComponent<LightQualitySwitch>();
                candleLightQualitySwitch.m_Light = light;
            }

            candleHeatComponent = lightObject.GetComponent<HeatSource>();

            if (candleHeatComponent == null)
            {
                candleHeatComponent = lightObject.AddComponent<HeatSource>();
            }

            if (Settings.options.endless)
            {
                tranformBody(UnityEngine.Random.RandomRangeInt(0, 4));
            }

            SaveLoad.SetLitState(PID, isLit);
            SaveLoad.SetBurnTime(PID, burnTime);
            SaveLoad.SetBodyState(PID, currentBodyState);
        }

        public void tranformBody(int newState)
        {
            if(GameManager.GetPlayerManagerComponent().m_InspectModeActive)
            {
                if (GameManager.GetPlayerManagerComponent().m_Inspect == inspectThingy)
                {
                    return;
                }
            }
           
            if (newState != currentBodyState && newState < 4)
            {
                bodyObject[currentBodyState].SetActive(false);
                currentBodyState = newState;
                bodyObject[currentBodyState].SetActive(true);
                                
                inspectThingy.m_NormalMesh = bodyObject[newState];
                inspectThingy.m_InspectModeMesh.SetActive(false);
                                
                inspectThingy.m_InspectModeMesh = inspectObjects[currentBodyState];
                SaveLoad.SetBodyState(PID, currentBodyState);
            }
        }	

		public void Update()
		{
            float burnTimeOptionDivision = Settings.options.burnTime / 4f;

            if (!Settings.options.endless && (burnTime > (burnTimeOptionDivision * 3)))
            {
                tranformBody(3);
            }
            else if (!Settings.options.endless && (burnTime > (burnTimeOptionDivision * 2)))
            {
                tranformBody(2);
            }
            else if (!Settings.options.endless && (burnTime > (burnTimeOptionDivision)))
            {
                tranformBody(1);
            }
            else if (!Settings.options.endless && (burnTime >= 0))
            {
                tranformBody(0);
            }

            if (isLit)
			{
                burnTime = burnTime + GameManager.GetTimeOfDayComponent().GetTODHours(Time.deltaTime);
                SaveLoad.SetBurnTime(PID, burnTime);

                if (candleGearItem.m_InPlayerInventory)
                {
                    turnOff();
                    return;
                }

                if (!Settings.options.endless && burnTime > Settings.options.burnTime)
                {
                    turnOff();
					return;
                }

				if(!GameManager.GetWindComponent().IsPositionOccludedFromWind(candleGearItem.transform.position) && GameManager.GetWindComponent().GetSpeedMPH() > Settings.options.windSensitivity)
				{
					turnOff();
					return;
				}				

                if (light.color != Candlelight_Main.candleLightColor)
                {
                    light.color = Candlelight_Main.candleLightColor;
                }

                if (flameMaterial[currentBodyState].color != Candlelight_Main.candleFlameColor)
                {
                    flameMaterial[currentBodyState].color = Candlelight_Main.candleFlameColor;
                }

                if (!Settings.options.enableFlicker && light.intensity != Settings.options.lightIntensity)
                {
                    light.intensity = Settings.options.lightIntensity;
                }

                if (light.range != Settings.options.lightRange)
                {
                    light.range = Settings.options.lightRange;
                }

                if (Settings.options.enableFlicker)
                {                    
                    Flicker();
                }
            }
		}

		public void Flicker()
		{
			float flickerFPS;

            if (flickerCounter<flickerRandom)
			{
                flickerCounter++;
                return;
			}

			if (GameManager.GetWeatherComponent().IsIndoorEnvironment())
			{
				flickerFPS = Settings.options.flickerFPS;
			}
			else
			{
				flickerFPS = Settings.options.flickerFPS + GameManager.GetWindComponent().GetSpeedMPH();
			}			 

			int x = (int)(Time.time * flickerFPS);

			int y = x % flickerPattern.Length;
			float intensity = (flickerPattern[y] - 'a') / (float)('m' - 'a');
			light.intensity = intensity * Settings.options.flickerMaxIntensity;
        }		

		public void turnOn(bool onLoad)
		{
            if (!Settings.options.endless && burnTime > Settings.options.burnTime)
            {
                if (!onLoad)
                {
                    GameAudioManager.PlayGUIError();
                    HUDMessage.AddMessage("The candle is completely burned down", true, true);
                }
                
                return;
            }

            if (GameManager.GetWindComponent().TooWindyForPlayerAction(Settings.options.windSensitivity) && !GameManager.GetWindComponent().IsPositionOccludedFromWind(candleGearItem.transform.position))
            {
                

                if (!onLoad)
                {
                    GameAudioManager.PlayGUIError();
                    HUDMessage.AddMessage("It is too windy to light the candle", true, true);
                }

                return;
            }

            flickerCounter = 0;

            lightObject.SetActive(true);
			flameObject[currentBodyState].SetActive(true);

            bodyMaterial[currentBodyState].SetFloat("_TimeScale", 0.04f);
            bodyMaterial[currentBodyState].SetFloat("_EmissionPow", 4.1f);
            bodyMaterial[currentBodyState].SetFloat("_EmissionFlowFactor", 0.3114f);
            bodyMaterial[currentBodyState].SetFloat("_EmissionFactor", 0.6f);
            bodyMaterial[currentBodyState].SetFloat("_EmissionNoise", 1);
            bodyMaterial[currentBodyState].SetFloat("_EmissionNoise1", 1);

			candleLightTrackingComponent.MaybeAdd(true);
            candleLightTrackingComponent.EnableLight(true);

            if (!missionIlluminationArea)
            {
                missionIlluminationArea = this.gameObject.AddComponent<MissionIlluminationArea>();
            }

            missionIlluminationArea.m_Radius = 2.5f;

            Heat(true);
			isLit = true;
            SaveLoad.SetLitState(PID, true);
        }

		public void turnOff()
		{
            candleLightTrackingComponent.EnableLight(false);
            light.gameObject.SetActive(false);
            flameObject[currentBodyState].SetActive(false);
			Heat(false);

            bodyMaterial[currentBodyState].SetFloat("_TimeScale", 0);
            bodyMaterial[currentBodyState].SetFloat("_EmissionPow", 0);
            bodyMaterial[currentBodyState].SetFloat("_EmissionFlowFactor", 0);
            bodyMaterial[currentBodyState].SetFloat("_EmissionFactor", 0);
            bodyMaterial[currentBodyState].SetFloat("_EmissionNoise", 0);
            bodyMaterial[currentBodyState].SetFloat("_EmissionNoise1", 0);

            if (isLit)
			{
                smokeParticle[currentBodyState].Play();
			}

            MissionIlluminationArea.s_RegisteredIlluminationAreas.Remove(missionIlluminationArea);
            GameObject.Destroy(missionIlluminationArea);

			SaveLoad.SetLitState(PID, false);
			flickerCounter = 0;
            isLit = false;			
		}				

		public void Heat(bool enable)
		{
			if(enable)
			{			
				candleHeatComponent.m_MaxTempIncrease = 2f;
				candleHeatComponent.m_MaxTempIncreaseInnerRadius = 2f;
				candleHeatComponent.m_MaxTempIncreaseOuterRadius = 1f;
				candleHeatComponent.m_TimeToReachMaxTempMinutes = 2f;
				candleHeatComponent.m_StartingTemp = 2f;
				candleHeatComponent.TurnOn();
            }
			else
			{
                candleHeatComponent.TurnOffImmediate();
            }
        }

		public void toggle()
		{
			if(isLit)
			{
				turnOff();				
			}
			else
			{
				turnOn(false);
			}			
		}
    }
}