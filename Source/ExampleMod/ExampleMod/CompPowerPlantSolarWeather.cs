using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace LinksSolarMod
{
    
    public class CompPowerPlantSolarWeather : CompPowerPlant
    {
        private const float FullSunPower = 1700f;

        private const float NightPower = 0f;

        private static readonly Vector2 BarSize = new Vector2(2.3f, 0.14f);

        private static readonly Material PowerPlantSolarBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f));

        private static readonly Material PowerPlantSolarBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f));

        //Here I create a new Red Material to use in the power display bar to show when its effected by weather
        private static readonly Material PowerPlantSolarBarUnderPoweredFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.red);

        //Create two private floats lastWeatherPowerFactor is used in transitioning our weatherpowerFactor in Mathf.Lerp()
        //Current is of course our currentpower factor thats set by whatever weather state we are in
        private float lastWeatherPowerFactor = 1f;
        private float currentWeatherPowerFactor = 0f;


        //lets set up some float variables for what we want the power factor to be for different weather types
        //I've only added 4 you can add the rest ;)
        private float hardSnowPowerFactor = 0.09f;
        private float clearPowerFactor = 1f;
        private float rainPowerFactor = 0.8f;
        private float fogPowerFactor = 0.9f;


        //here we create a WeatherDef variable
        public WeatherDef currentWeather
        {
            get
            {
                // and assign it to whatever the current map's current weather is
                // we will use this "currentWeather" variable in our if() statements where before we used "this.parent.Map.weatherManager.curWeather" in every if statement its untidy and harder to read
                return this.parent.Map.weatherManager.curWeather;
            }
        }
        private WeatherDef lastWeather = WeatherDef.Named("Clear");

        //We have moved the powerfactor deciding logic to inside "DesiredPowerOutput" getter 
        protected override float DesiredPowerOutput
        {
            get
            {
                //hopefully this part is easy to read

                //Check if the current weather state is a state we want to change the weatherPowerFactor variable
                if (currentWeather == WeatherDef.Named("Clear"))
                {

                    //and here is where we smoothly transition the poweroutput along with the changing of the weather                


                    this.currentWeatherPowerFactor = Mathf.Lerp(lastWeatherPowerFactor, clearPowerFactor , this.parent.Map.weatherManager.TransitionLerpFactor);
                    // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                    //notice we aren't return a plain old float (return 0.1f) instead we are setting this.currentPowerFactor to the result of Mathf.Lerp() which is a function that takes two numbers (from, to, time) and changes them over time by a third number and returns a resulting float
                   //


                    //Since we need to remember what powerfactor we were using last to switch nicely to the new weatherpowerfactor we need to store it
                    //so since we are in this code block that means the current weather is now Clear 
                    //therefore the lastWeather = clear
                    lastWeatherPowerFactor = this.currentWeatherPowerFactor;
                }
                else if (currentWeather == WeatherDef.Named("SnowHard"))
                {
                    //if we were just in the if(currentWeather == WeatherDef.Named("Clear")) statement then lastWeatherPowerFactor is 1.0f and 0.09f is the powerfactor we want now its snowing hard so the Lerp function increments up from 0.09f to 1.f
                    //this works the same way in each statement
                    //also notice the second parameter we pass to Mathf.Lerp() hardSnowPowerFactor we created and gave a value of 0.09f at the top? 
                    this.currentWeatherPowerFactor = Mathf.Lerp(lastWeatherPowerFactor, hardSnowPowerFactor, this.parent.Map.weatherManager.TransitionLerpFactor);
                    //You guessed it the lastWeatherPowerFactor is now set to our currentWeatherFactor 0.09f
                    lastWeatherPowerFactor = this.currentWeatherPowerFactor;
                }
                else if (currentWeather == WeatherDef.Named("Rain"))
                {
                    this.currentWeatherPowerFactor = Mathf.Lerp(lastWeatherPowerFactor, rainPowerFactor , this.parent.Map.weatherManager.TransitionLerpFactor);
                    lastWeatherPowerFactor = this.currentWeatherPowerFactor;
                }
                else if (currentWeather == WeatherDef.Named("Fog"))
                {
                    this.currentWeatherPowerFactor = Mathf.Lerp(lastWeatherPowerFactor, fogPowerFactor , this.parent.Map.weatherManager.TransitionLerpFactor);
                    lastWeatherPowerFactor = this.currentWeatherPowerFactor;
                }
                //there are 8 Weather defs in total I've only implemented it to work 4, the other 4 are all yours :D

                //this is where the power output is decided notice 1700f (Max solar output) * (times) our weatherFactor
                return Mathf.Lerp(0f, 1700f * this.currentWeatherPowerFactor, this.parent.Map.skyManager.CurSkyGlow) * this.RoofedPowerOutputFactor;
                // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                //So the 1700f * this.currentPowerFactor line above where we are multiplying the max solar output by our currentPowerFactor the "this.currentPowerFactor" isnt changing straight away to 0.5f 
                //it will instead work its way from our lastWeatherPowerFactor (Whatever it last was) we were just at (0f) to our next powerFactor which is fog in this case (0.5f)
                //eg: 0f > 0.1f > 0.2f > 0.3f > 0.4f > 0.5f
                //which results in a smooth transition rather than the immediate drop of the last version
            }

        }



        /// <summary>
        /// OLD CODE BITS
        /// 
        /// We removed weatherPowerFactor entirely and replaced it with two new variables lastWeatherPowerFactor and currentWeatherPowerFactor
        /// and we moved the weather logic to DesiredPoweroutput
        /// </summary>

        //protected override float DesiredPowerOutput
        //{
        //    get
        //    {
        //        //this is where the power output is decided notice 1700f (Max solar output) * (times) our weatherFactor
        //        return Mathf.Lerp(0f, 1700f * this.weatherPowerFactor, this.parent.Map.skyManager.CurSkyGlow) * this.RoofedPowerOutputFactor;
        //    }

        //}

        ////This is where the weatherPowerFactor is changed, we create a new float (private float weatherPowerFactor) and change its value depending on the weather.
        //private float weatherPowerFactor
        //{
        //    get
        //    {
        //        //So if all you want to do is set power penalties depending on the weather add another "else if()" statement and set this.weatherPowerFactor to whatever you want the power factor to be.
        //        //the power factor should be between 0 and 1.0 
        //        //1700 power units * 0.5 = 850 power units
        //        //conversly 1700 * 1.5 = 2550 ( a big power increase)
        //        if (this.parent.Map.weatherManager.curWeather == WeatherDef.Named("Fog"))
        //        {
        //            return 0.2f;
        //        }
        //        else if (this.parent.Map.weatherManager.curWeather == WeatherDef.Named("SnowHard"))
        //        {
        //            return 0.001f;
        //        }
        //        else if (this.parent.Map.weatherManager.curWeather == WeatherDef.Named("Rain"))
        //        {
        //            return 0.9f;
        //        }


        //        return 1.0f;
        //    }
        //}











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
