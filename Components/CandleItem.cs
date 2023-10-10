using System;
using MelonLoader;
using UnityEngine;
using System.Collections;
using Il2Cpp;

namespace Candlelight
{
	[RegisterTypeInIl2Cpp]
	public class CandleItem : MonoBehaviour
	{
        public CandleItem(IntPtr intPtr) : base(intPtr)
        {
        }

		public string PID;
		public float burnTime = 0;
		public GearItem candleGearItem;

        public Light lightSourceMain;
		public Light lightSourceSecondary;
		public GearItem thisGearItem;
		public Material bodyMaterial;
		public Material flameMaterial;
		public int flickerRandom = 30;
		public int flickerCounter = 0;

		public MeshRenderer body0Mesh;
		public MeshRenderer body1Mesh;
		public MeshRenderer body2Mesh;
		public MeshRenderer body3Mesh;

		public float wickYbody0 = 0.47f;
		public float wickYbody1 = 0.32f;
		public float wickYbody2 = 0.13f;
		public float wickYbody3 = 0.046f;

		public float flameYbody0 = 0.551f;
		public float flameYbody1 = 0.396f;
		public float flameYbody2 = 0.216f;
		public float flameYbody3 = 0.131f;

		public Vector3 zeroRotation = new Vector3(0,0,0);
        public Vector3 lightLocalPosition = new Vector3(0, 0.667f, 0);

        public int currentBodyState = -1;

		public GameObject flame;
		public GameObject flameParent;
		public GameObject wick;
		public Material wickMaterial;

		public HeatSource candleHeatComponent;
		public LightTracking candleLightTrackingComponent;
		public LightQualitySwitch candleLightQualitySwitch;

		public ParticleSystem smokePuff;
		public ParticleSystem smokePuff2;

		public GameObject normalModelParent;
		public GameObject inspectModelParent;
		public Inspect inspectComponent;

		public float windSpeedExtinguish = 3f;
		public float lifeTimeDivisor = 4.5f; // about 8 hours burn time
		public string flickerPattern = "mmnmmommommnonmmonqnmmo";

		public bool isLit = false;						

		public void Awake()
		{
            thisGearItem = this.gameObject.GetComponent<GearItem>();

            lightSourceMain = this.transform.Find("LightsourceMain").gameObject.GetComponent<Light>();
            lightSourceMain.transform.localPosition = lightLocalPosition;
            lightSourceSecondary = this.transform.Find("LightsourceSecondary").gameObject.GetComponent<Light>();
            lightSourceSecondary.gameObject.SetActive(false);

            flameParent = this.transform.Find("FlameParent").gameObject;
            flame = flameParent.transform.Find("Flame").gameObject;
            flameMaterial = flame.GetComponent<MeshRenderer>().material;

            normalModelParent = this.transform.Find("Normal").gameObject;
            inspectModelParent = this.transform.Find("Inspect").gameObject;
            inspectModelParent.SetActive(false);

            inspectComponent = this.GetComponent<Inspect>();
			inspectComponent.m_InspectModeMesh = this.gameObject;

            wick = this.transform.Find("Wick").gameObject;
            wickMaterial = wick.GetComponent<MeshRenderer>().material;
            wickMaterial.SetFloat("_Emission_min", 1);

            flickerRandom = UnityEngine.Random.Range(5, 50);

            smokePuff = wick.transform.Find("smokePuff").gameObject.GetComponent<ParticleSystem>();

            body0Mesh = normalModelParent.transform.Find("Body0").gameObject.GetComponent<MeshRenderer>();
            body1Mesh = normalModelParent.transform.Find("Body1").gameObject.GetComponent<MeshRenderer>();
            body2Mesh = normalModelParent.transform.Find("Body2").gameObject.GetComponent<MeshRenderer>();
            body3Mesh = normalModelParent.transform.Find("Body3").gameObject.GetComponent<MeshRenderer>();

            body0Mesh.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            body1Mesh.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            body2Mesh.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            body3Mesh.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

            bodyMaterial = body0Mesh.material;            

            lightSourceMain.intensity = Settings.options.lightIntensity;
            lightSourceMain.range = Settings.options.lightRange;

            candleLightTrackingComponent = lightSourceMain.gameObject.GetComponent<LightTracking>();

            if (candleLightTrackingComponent == null)
            {
                candleLightTrackingComponent = lightSourceMain.gameObject.AddComponent<LightTracking>();
            }

            candleLightQualitySwitch = lightSourceMain.gameObject.GetComponent<LightQualitySwitch>();

            if (candleLightQualitySwitch == null)
            {
                candleLightQualitySwitch = lightSourceMain.gameObject.AddComponent<LightQualitySwitch>();
            }

            candleHeatComponent = lightSourceMain.gameObject.GetComponent<HeatSource>();

            if (candleHeatComponent == null)
            {
                candleHeatComponent = lightSourceMain.gameObject.AddComponent<HeatSource>();
            }

			transformCandle();
            turnOff();
        }
		

		public bool transformCandle()
		{
			//inspectModelParent.SetActive(false);					

            float burnTimeOptionDivision = Settings.options.burnTime / 4f;

            if (Settings.options.burnTime == 0)
            {
				if(currentBodyState == 2)
				{
					return true;
				}

                body0Mesh.enabled = false;
                body1Mesh.enabled = false;				
                body2Mesh.enabled = true;
                body3Mesh.enabled = false;
                bodyMaterial = body1Mesh.material;
                bodyMaterialOn();
                wick.transform.localPosition = new Vector3(wick.transform.localPosition.x, wickYbody1, wick.transform.localPosition.z);
                flameParent.transform.localPosition = new Vector3(flame.transform.localPosition.x, flameYbody1, flame.transform.localPosition.z);

                currentBodyState = 2;
                return true;
            }
            else if (burnTime > (burnTimeOptionDivision * 3))
            {
                if (currentBodyState == 3)
                {
                    return true;
                }

                body0Mesh.enabled = false;
                body1Mesh.enabled = false;
                body2Mesh.enabled = false;
                body3Mesh.enabled = true;
                bodyMaterial = body3Mesh.material;
                bodyMaterialOn();
                wick.transform.localPosition = new Vector3(wick.transform.localPosition.x, wickYbody3, wick.transform.localPosition.z);
                flameParent.transform.localPosition = new Vector3(flame.transform.localPosition.x, flameYbody3, flame.transform.localPosition.z);

                currentBodyState = 3;
                return true;
            }
            else if (burnTime > (burnTimeOptionDivision * 2))
            {
                if (currentBodyState == 2)
                {
                    return true;
                }

                body0Mesh.enabled = false;
                body1Mesh.enabled = false;
                body2Mesh.enabled = true;
                body3Mesh.enabled = false;
                bodyMaterial = body2Mesh.material;
                bodyMaterialOn();
                wick.transform.localPosition = new Vector3(wick.transform.localPosition.x, wickYbody2, wick.transform.localPosition.z);
                flameParent.transform.localPosition = new Vector3(flame.transform.localPosition.x, flameYbody2, flame.transform.localPosition.z);

                currentBodyState = 2;
                return true;
            }
            else if (burnTime > (burnTimeOptionDivision))
            {
                if (currentBodyState == 1)
                {
                    return true;
                }

                body0Mesh.enabled = false;
                body1Mesh.enabled = true;
                body2Mesh.enabled = false;
                body3Mesh.enabled = false;
                bodyMaterial = body1Mesh.material;
                bodyMaterialOn();
                wick.transform.localPosition = new Vector3(wick.transform.localPosition.x, wickYbody1, wick.transform.localPosition.z);
                flameParent.transform.localPosition = new Vector3(flame.transform.localPosition.x, flameYbody1, flame.transform.localPosition.z);

                currentBodyState = 1;
                return true;
            }
            else if (burnTime >= 0)
            {
                if (currentBodyState == 0)
                {
                    return true;
                }

                body0Mesh.enabled = true;
                body1Mesh.enabled = false;
                body2Mesh.enabled = false;
                body3Mesh.enabled = false;
                bodyMaterial = body0Mesh.material;
                bodyMaterialOn();
                wick.transform.localPosition = new Vector3(wick.transform.localPosition.x, wickYbody0, wick.transform.localPosition.z);
                flameParent.transform.localPosition = new Vector3(flame.transform.localPosition.x, flameYbody0, flame.transform.localPosition.z);

                currentBodyState = 0;
                return true;
            }         
           

            return false;
		}

		public void Update()
		{
			if (isLit)
			{
                burnTime = burnTime + GameManager.GetTimeOfDayComponent().GetTODHours(Time.deltaTime);
                
				if(flickerCounter < flickerRandom)
				{
					flickerCounter++;
				}
				
				transformCandle();
                SaveLoad.SetBurnTime(PID, burnTime);

                if (Settings.options.burnTime != 0 && burnTime > Settings.options.burnTime)
                {
                    turnOff();
					return;
                }

                if (thisGearItem.m_InPlayerInventory)
				{
					turnOff();
					return;
				}

				if(!GameManager.GetWindComponent().IsPositionOccludedFromWind(thisGearItem.transform.position) && GameManager.GetWindComponent().GetSpeedMPH() > windSpeedExtinguish)
				{
					turnOff();
					return;
				}

				if (Settings.options.enableFlicker)
				{
					Flicker();
				}
				else
				{
					if (lightSourceMain.intensity != Settings.options.lightIntensity || lightSourceMain.range != Settings.options.lightRange)
					{
						lightSourceMain.intensity = Settings.options.lightIntensity;
						lightSourceMain.range = Settings.options.lightRange;
					}
				}               
			}
		}

		public void Flicker()
		{
			float flickerFPS;

			if(flickerCounter<flickerRandom)
			{				
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
			lightSourceMain.intensity = intensity * Settings.options.flickerMaxIntensity;
			lightSourceMain.range = Settings.options.lightRange;
		}		

		public void turnOn(bool onLoad)
		{
            if (Settings.options.burnTime > 0 && burnTime > Settings.options.burnTime)
            {
                GameAudioManager.PlayGUIError();

                if (!onLoad)
                {
                    HUDMessage.AddMessage("The candle is completely burned down", true, true);
                }
                
                return;
            }

            if (GameManager.GetWindComponent().TooWindyForPlayerAction(windSpeedExtinguish) && !GameManager.GetWindComponent().IsPositionOccludedFromWind(thisGearItem.transform.position))
            {
                GameAudioManager.PlayGUIError();

                if (!onLoad)
                {
                    HUDMessage.AddMessage("It is too windy to light the candle", true, true);
                }

                turnOff();
                return;
            }

            flickerCounter = 0;

            lightSourceMain.gameObject.SetActive(true);
			flame.gameObject.SetActive(true);

			bodyMaterialOn();

			candleHeatComponent.TurnOn();
			
			candleLightTrackingComponent.MaybeAdd(true);
			candleLightTrackingComponent.EnableLight(true);

			lightSourceMain.color = Candlelight_Main.candleLightColor;
			flameMaterial.color = Candlelight_Main.candleFlameColor;

			isLit = true;
			MelonCoroutines.Start(toggleWickGlow(true, wickMaterial));
			alignFlameToNewPosition();
            Heat(true);
            SaveLoad.SetLitState(PID, true);
        }

		public void bodyMaterialOn()
		{
            //bodyMaterial.SetFloat("_TimeScale", 0.05f);
            bodyMaterial.SetFloat("_TimeScale", UnityEngine.Random.Range(38f, 64f)/1000);
            bodyMaterial.SetFloat("_EmissionPow", 4.1f);
			bodyMaterial.SetFloat("_EmissionFlowFactor", 0.3114f);
			bodyMaterial.SetFloat("_EmissionFactor", 0.6f);
			bodyMaterial.SetFloat("_EmissionNoise", 1);
			bodyMaterial.SetFloat("_EmissionNoise1", 1);
		}

		public void bodyMaterialOff()
		{
			bodyMaterial.SetFloat("_TimeScale", 0);
			bodyMaterial.SetFloat("_EmissionPow", 0);
			bodyMaterial.SetFloat("_EmissionFlowFactor", 0);
			bodyMaterial.SetFloat("_EmissionFactor", 0);
			bodyMaterial.SetFloat("_EmissionNoise", 0);
			bodyMaterial.SetFloat("_EmissionNoise1", 0);			
		}

		public void alignFlameToNewPosition()
		{
			flameParent.transform.eulerAngles = zeroRotation;
		}

		public void turnOff()
		{
			lightSourceMain.gameObject.SetActive(false);
			lightSourceSecondary.gameObject.SetActive(false);
			flame.gameObject.SetActive(false);
			candleHeatComponent.TurnOffImmediate();
			Heat(false);

            bodyMaterialOff();
			
			if(isLit)
			{
				MelonCoroutines.Start(toggleWickGlow(false, wickMaterial));
				smokePuff.Play();
			}

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
                candleHeatComponent.TurnOff();
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

		public static IEnumerator toggleWickGlow(bool glow, Material wickMaterial)
		{
			float lerpDuration = 2f;
			float startValue;
			float endValue;
			float valueToLerp;

			if (glow)
			{
				startValue = 1.0f;
				endValue = 0.9f;
			}
			else
			{
				startValue = 0.9f;
				endValue = 1.0f;
			}

			float timeElapsed = 0;

			while (timeElapsed < lerpDuration)
			{				
				valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
				timeElapsed += Time.deltaTime;
				wickMaterial.SetFloat("_Emission_min", valueToLerp);

				yield return null;
			}

			valueToLerp = endValue;
			wickMaterial.SetFloat("_Emission_min", valueToLerp);
		}
    }
}