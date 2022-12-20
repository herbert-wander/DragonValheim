using System;

namespace DragonValheim
{
    class PlantDV
    {
        static readonly Utils helper = DragonValheim.modInstance.Helper;
        public void RemovePlantBiomePlantingRestriction(Plant instancia)
        {
            Heightmap.Biome allBiomes = 0;
            foreach (Heightmap.Biome biome in Enum.GetValues(typeof(Heightmap.Biome)))
            {
                allBiomes |= biome;
            }

            instancia.m_biome = allBiomes;
        }

        public string InsertTimerInHoverText(Plant instance)
        {
            double percentage = helper.GetPercentage(instance.TimeSincePlanted(), instance.m_growTime);
            string color = helper.GetStageColor(percentage);
            string timeLeft = helper.FormatSecondsToTime((int)(instance.m_growTime - instance.TimeSincePlanted()));
            string extraInfos = $"<color={color}>{percentage}% - {timeLeft} Left</color>";
            return extraInfos;
        }
    }

}
