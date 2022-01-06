using System;
using MelonLoader;
using UnityEngine;
using System.Collections;

namespace Candlelight
{
	public class CandleItem : MonoBehaviour
	{
		public Light lightSourceMain;
		public Light lightSourceSecondary;
		public GearItem thisGearItem;
		public Material bodyMaterial;
		public Material flameMaterial;

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

		public int currentBodyState = -1;

		public GameObject flame;
		public GameObject flameParent;
		public GameObject wick;
		public Material wickMaterial;

		public HeatSource candleHeatComponent = null;
		public LightTracking candleLightTrackingComponent = null;
		public CandleAction candleActionComponent = null;
		public ParticleSystem smokePuff;
		public ParticleSystem smokePuff2;

		public GameObject normalModelParent;
		public GameObject inspectModelParent;

		public float windSpeedExtinguish = 3f;
		public float lifeTimeDivisor = 4.5f; // about 8 hours burn time
		public string flickerPattern = "mmnmmommommnonmmonqnmmo";

		public bool isLit = false;

		public CandleItem(IntPtr intPtr) : base(intPtr) { }

		
		public void Start()
		{
			InitAll();
		}

		public void Awake()
		{
			InitAll();
		}
		

		public void InitAll()
		{
			thisGearItem = this.gameObject.GetComponent<GearItem>();
			lightSourceMain = this.transform.Find("LightsourceMain").gameObject.GetComponent<Light>();
			lightSourceSecondary = this.transform.Find("LightsourceSecondary").gameObject.GetComponent<Light>();
			lightSourceSecondary.gameObject.SetActive(false);

			flameParent = this.transform.Find("FlameParent").gameObject;
			flame = flameParent.transform.Find("Flame").gameObject;
			flameMaterial = flame.GetComponent<MeshRenderer>().material;

			normalModelParent = this.transform.Find("Normal").gameObject;
			inspectModelParent = this.transform.Find("Inspect").gameObject;

			wick = this.transform.Find("Wick").gameObject;
			wickMaterial = wick.GetComponent<MeshRenderer>().material;
			wickMaterial.SetFloat("_Emission_min", 1);

			smokePuff = wick.transform.Find("smokePuff").gameObject.GetComponent<ParticleSystem>();

			body0Mesh = normalModelParent.transform.Find("Body0").gameObject.GetComponent<MeshRenderer>();
			body1Mesh = normalModelParent.transform.Find("Body1").gameObject.GetComponent<MeshRenderer>();
			body2Mesh = normalModelParent.transform.Find("Body2").gameObject.GetComponent<MeshRenderer>();
			body3Mesh = normalModelParent.transform.Find("Body3").gameObject.GetComponent<MeshRenderer>();
						
			bodyMaterial = body0Mesh.material;
			transformCandle();

			lightSourceMain.intensity = Settings.options.lightIntensity;
			lightSourceMain.range = Settings.options.lightRange;


			if (candleActionComponent == null)
			{
				candleActionComponent = thisGearItem.gameObject.AddComponent<CandleAction>();
			}

			if (candleLightTrackingComponent == null)
			{
				candleLightTrackingComponent = lightSourceMain.gameObject.AddComponent<LightTracking>();
			}

			if (candleHeatComponent == null)
			{
				candleHeatComponent = thisGearItem.gameObject.AddComponent<HeatSource>();
			}

			InitHeat();

			// Cheaty workaround to save candle on/off status
			if (thisGearItem.m_BeenInPlayerInventory)
			{
				turnOn();
			}
			else
			{
				turnOff();
			}
		}

		public bool transformCandle()
		{
			inspectModelParent.SetActive(false);

			if (currentBodyState != 0 && thisGearItem.m_CurrentHP >= 75)
			{
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
			else if (currentBodyState != 1 && thisGearItem.m_CurrentHP >= 50 && thisGearItem.m_CurrentHP < 75)
			{
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
			else if (currentBodyState != 2 && thisGearItem.m_CurrentHP >= 25 && thisGearItem.m_CurrentHP < 50)
			{
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
			else if (currentBodyState != 3 && thisGearItem.m_CurrentHP >= 0 && thisGearItem.m_CurrentHP < 25)
			{
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

			return false;
		}

		public void Update()
		{
			if (isLit)
			{
				transformCandle();

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
				
				//thisGearItem.m_MaxHP = thisGearItem.m_MaxHP - (todminutes/3);
				if(thisGearItem.m_CurrentHP > 0)
				{
					float todminutes = GameManager.GetTimeOfDayComponent().GetTODMinutes(Time.deltaTime);
					thisGearItem.m_CurrentHP = thisGearItem.m_CurrentHP - (todminutes/lifeTimeDivisor);
					//MelonLogger.Msg("HP current: " + thisGearItem.m_CurrentHP);
				}
				else
				{
					thisGearItem.m_CurrentHP = 0f;
					turnOff();
				}
			}
		}

		public void Flicker()
		{
			float flickerFPS;

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

		public bool canIgnite()
		{	
			if (thisGearItem.m_CurrentHP < 0.1f || (GameManager.GetWindComponent().TooWindyForPlayerAction(windSpeedExtinguish) && !GameManager.GetWindComponent().IsPositionOccludedFromWind(thisGearItem.transform.position)))
			{				
				return false;
			}

			return true;
		}

		public bool turnOn()
		{
			if(!canIgnite())
			{
				if (thisGearItem.m_CurrentHP < 0.1f)
				{
					GameAudioManager.PlayGUIError();
					HUDMessage.AddMessage("The candle is completely burned down", true, true);
					turnOff();
					return false;
				}

				if (GameManager.GetWindComponent().TooWindyForPlayerAction(windSpeedExtinguish) && !GameManager.GetWindComponent().IsPositionOccludedFromWind(thisGearItem.transform.position))
				{
					GameAudioManager.PlayGUIError();
					HUDMessage.AddMessage("It is too windy to light the candle", true, true);
					turnOff();
					return false;
				}

				return false;
			}
			
			lightSourceMain.gameObject.SetActive(true);
			flame.gameObject.SetActive(true);
			bodyMaterialOn();

			candleHeatComponent.TurnOn();
			
			//candleLightTrackingComponent.MaybeAdd(true);
			//candleLightTrackingComponent.EnableLight(true);

			lightSourceMain.color = Candlelight_Main.candleLightColor;
			flameMaterial.color = Candlelight_Main.candleFlameColor;

			isLit = true;
			MelonCoroutines.Start(toggleWickGlow(true, wickMaterial));
			alignFlameToNewPosition();

			thisGearItem.m_BeenInPlayerInventory = true;
			return true;
		}

		public void bodyMaterialOn()
		{
			bodyMaterial.SetFloat("_TimeScale", 0.05f);
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
			//LightingManager.Remove(candleLightTrackingComponent);
			bodyMaterialOff();
			
			if(isLit)
			{
				MelonCoroutines.Start(toggleWickGlow(false, wickMaterial));
				smokePuff.Play();
			}

			isLit = false;
			thisGearItem.m_BeenInPlayerInventory = false;
		}

		public void fakeOff()
		{
			lightSourceMain.gameObject.SetActive(false);
			lightSourceSecondary.gameObject.SetActive(false);
			flame.gameObject.SetActive(false);
			candleHeatComponent.TurnOffImmediate();
			//smokePuff.Play();

			bodyMaterialOff();
		}

		public void fakeOn()
		{
			if (isLit)
			{
				turnOn();
			}
			else
			{
				turnOff();
			}
		}

		public void InitHeat()
		{
			candleHeatComponent.m_MaxTempIncrease = 2f;
			candleHeatComponent.m_MaxTempIncreaseInnerRadius = 1f;
			candleHeatComponent.m_MaxTempIncreaseOuterRadius = 1.5f;
			candleHeatComponent.m_TimeToReachMaxTempMinutes = 2f;
			candleHeatComponent.m_StartingTemp = 0f;
		}

		public void toggle()
		{
			if(isLit)
			{
				turnOff();
			}
			else
			{
				turnOn();
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