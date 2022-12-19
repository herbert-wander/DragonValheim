using System;

namespace DragonValheim
{
    class PlantDV
    {
        static Utils helper = DragonValheim.modInstance.Helper;
        public void removePlantBiomePlantingRestriction(Plant instancia)
        {
            Heightmap.Biome allBiomes = 0;
            foreach (Heightmap.Biome biome in Enum.GetValues(typeof(Heightmap.Biome)))
            {
                allBiomes |= biome;
            }

            instancia.m_biome = allBiomes;
        }

        public string InsertTimerInHoverText(Plant instance, string result)
        {
            double percentage = helper.GetPercentage(instance.TimeSincePlanted(), instance.m_growTime);
            string color = helper.GetStageColor(percentage);
            string timeLeft = helper.FormatSecondsToTime((int)(instance.m_growTime - instance.TimeSincePlanted()));
            string extraInfos = $"<color={color}>{percentage}% - {timeLeft} Left</color>";
            return extraInfos;
        }
    }

}
