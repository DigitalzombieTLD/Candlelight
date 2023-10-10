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

        public static void LoadTheCandles()
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

        public static void SetBurnTime(string candleID, float burnTime)
        {
            MelonLogger.Msg("Set burntime: " + burnTime);
            thisIniData[candleID].AddKey("burnTime");
            thisIniData[candleID]["burnTime"] = burnTime.ToString();
        }

        public static void SetLitState(string candleID, bool litState)
        {            
            thisIniData[candleID].AddKey("isLit");
            thisIniData[candleID]["isLit"] = litState.ToString();
        }

        public static void SaveTheCandles()
		{
            dataManager.Save(thisIniData.ToString());
        }
	}
}