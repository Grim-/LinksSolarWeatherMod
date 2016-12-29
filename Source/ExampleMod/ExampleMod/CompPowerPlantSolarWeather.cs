using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace LinksSolarMod
{
    [StaticConstructorOnStartup]
    public class CompPowerPlantSolarWeather : CompPowerPlant
    {
        private const float FullSunPower = 1700f;

        private const float NightPower = 0f;

        private static readonly Vector2 BarSize = new Vector2(2.3f, 0.14f);

        private static readonly Material PowerPlantSolarBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f));

        private static readonly Material PowerPlantSolarBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f));

        private static readonly Material PowerPlantSolarBarUnderPoweredFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.red);


        protected override float DesiredPowerOutput
        {
            get
            {
                return Mathf.Lerp(0f, 1700f * this.weatherPowerFactor, this.parent.Map.skyManager.CurSkyGlow) * this.RoofedPowerOutputFactor;
            }

        }

        private float weatherPowerFactor
        {
            get
            {
                if (this.parent.Map.weatherManager.curWeather == WeatherDef.Named("Fog"))
                {
                    return 0.2f;
                }
                else if(this.parent.Map.weatherManager.curWeather == WeatherDef.Named("SnowHard"))
                {
                    return 0.001f;
                }
                return 1.0f;
            }
        }


        private float RoofedPowerOutputFactor
        {
            get
            {
                int num = 0;
                int num2 = 0;
                foreach (IntVec3 current in this.parent.OccupiedRect())
                {
                    num++;
                    if (this.parent.Map.roofGrid.Roofed(current))
                    {
                        num2++;
                    }
                }
                return (float)(num - num2) / (float)num;
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
            r.center = this.parent.DrawPos + Vector3.up * 0.1f;
            r.size = CompPowerPlantSolarWeather.BarSize;
            r.fillPercent = base.PowerOutput / 1700f;

            if(this.parent.Map.weatherManager.curWeather == WeatherDef.Named("Fog") || this.parent.Map.weatherManager.curWeather == WeatherDef.Named("SnowHard"))
            {
                r.filledMat = CompPowerPlantSolarWeather.PowerPlantSolarBarUnderPoweredFilledMat;
            }
            else
            {
                r.filledMat = CompPowerPlantSolarWeather.PowerPlantSolarBarFilledMat;
            }
            r.unfilledMat = CompPowerPlantSolarWeather.PowerPlantSolarBarUnfilledMat;
            r.margin = 0.15f;
            Rot4 rotation = this.parent.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            r.rotation = rotation;
            GenDraw.DrawFillableBar(r);
        }
    }
}
