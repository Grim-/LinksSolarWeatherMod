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

        //Here I create a new Red Material to use in the power display bar to show when its effected by weather
        private static readonly Material PowerPlantSolarBarUnderPoweredFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.red);


        protected override float DesiredPowerOutput
        {
            get
            {
                //this is where the power output is decided notice 1700f (Max solar output) * (times) our weatherFactor
                return Mathf.Lerp(0f, 1700f * this.weatherPowerFactor, this.parent.Map.skyManager.CurSkyGlow) * this.RoofedPowerOutputFactor;
            }

        }

        //This is where the weatherPowerFactor is changed, we create a new float (private float weatherPowerFactor) and change its value depending on the weather.
        private float weatherPowerFactor
        {
            get
            {
                //So if all you want to do is set power penalties depending on the weather add another "else if()" statement and set this.weatherPowerFactor to whatever you want the power factor to be.
                //the power factor should be between 0 and 1.0 
                //1700 power units * 0.5 = 850 power units
                //conversly 1700 * 1.5 = 2550 ( a big power increase)
                if (this.parent.Map.weatherManager.curWeather == WeatherDef.Named("Fog"))
                {
                    return 0.2f;
                }
                else if(this.parent.Map.weatherManager.curWeather == WeatherDef.Named("SnowHard"))
                {
                    return 0.001f;
                }
                else if(this.parent.Map.weatherManager.curWeather == WeatherDef.Named("Rain"))
                {
                    return 0.9f;
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

     
            var curWeather = this.parent.Map.weatherManager.curWeather;

            //This line simply uses the red material we defined above if the current weather is either fog or snowhard
            //you can add the other weathers here by using || (or operator) and adding another 
            /// if(curWeather == WeatherDef.Named("Fog") || curWeather == WeatherDef.Named("SnowHard") || curWeather == WeatherDef.Named("Rain") || currWeather = WeatherDef.Named("Yours here");
            if (curWeather == WeatherDef.Named("Fog") || curWeather == WeatherDef.Named("SnowHard") || curWeather == WeatherDef.Named("Rain"))
            {
                r.filledMat = CompPowerPlantSolarWeather.PowerPlantSolarBarUnderPoweredFilledMat;
            }
            else
            {
                //otherwise use the normal yellow one
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
