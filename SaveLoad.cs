using MelonLoader;
using UnityEngine;
using System.Collections;
using Il2Cpp;
using Ini;
using Ini.Model;
using Ini.Parser;
using ModData;
using Il2CppNewtonsoft.Json.Linq;
using Il2CppMono;

namespace Candlelight
{
	public static class SaveLoad
	{
        public static IniDataParser iniDataParser;
        public static IniData thisIniData;
        public static ModDataManager dataManager;
        public static string moddataString;
        public static bool reloadPending = true;

        public static void LoadTheCandles()
		{
            if (reloadPending)
            {
                iniDataParser = new IniDataParser();
                thisIniData = new IniData();
                dataManager = new ModDataManager("CandleLight", true);

                iniDataParser.Configuration.AllowCreateSectionsOnFly = true;
                iniDataParser.Configuration.SkipInvalidLines = true;
                iniDataParser.Configuration.OverrideDuplicateKeys = true;
                iniDataParser.Configuration.AllowDuplicateKeys = false;

                moddataString = dataManager.Load();
                thisIniData = iniDataParser.Parse(moddataString);

                reloadPending = false;
            }
        }      

        public static void MaybeAddCandle(string candleID)
        {
            if (!thisIniData.Sections.ContainsSection(candleID))
            {
                thisIniData.Sections.AddSection(candleID);
            }
        }
               
		public static float GetBurnTime(string candleID)
		{
            if (candleID == null || !thisIniData[candleID].ContainsKey("burnTime"))
            {
                return 0f;
            }

            return float.Parse(thisIniData[candleID]["burnTime"]);
        }

		public static bool GetLitState(string candleID)
		{
            if (candleID == null || !thisIniData[candleID].ContainsKey("isLit"))
            {
                return false;
            }

            return bool.Parse(thisIniData[candleID]["isLit"]);
        }

        public static int GetBodyState(string candleID)
        {
            if (candleID == null || !thisIniData[candleID].ContainsKey("bodyState"))
            {
                return UnityEngine.Random.RandomRange(0,3);
            }

            return int.Parse(thisIniData[candleID]["bodyState"]);
        }

        public static void SetBurnTime(string candleID, float burnTime)
        {
            if (thisIniData == null)
            {
              
            }
            else
            {
                thisIniData[candleID].AddKey("burnTime");
                thisIniData[candleID]["burnTime"] = burnTime.ToString();
            }     
        }

        public static void SetLitState(string candleID, bool litState)
        {  
            if(thisIniData == null)
            {

            }
            else
            {
                thisIniData[candleID].AddKey("isLit");
                thisIniData[candleID]["isLit"] = litState.ToString();
            }            
        }

        public static void SetBodyState(string candleID, int bodyState)
        {
            if (thisIniData == null)
            {

            }
            else
            {
                thisIniData[candleID].AddKey("bodyState");
                thisIniData[candleID]["bodyState"] = bodyState.ToString();
            }            
        }

        public static void SaveTheCandles()
		{
            dataManager.Save(thisIniData.ToString());
        }
	}
}