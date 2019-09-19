﻿using System;
using System.Collections.Generic;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.ModAPI;

// TODO: counterpush should also affect mass blocks
// TODO: a way to enforce counterpush server side
// TODO: block-box resolution field checking? ( GetEntitiesIn*() instead of GetTopMostEntitiesIn*() )
// TODO: parallel just like sensor does it
// TODO: implement terminal controls, saving, synching...

namespace Digi.EnhancedGravityGenerators
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class EGGMod : MySessionComponentBase
    {
        public static EGGMod Instance = null;

        private int skipPlanets = SKIP_PLANETS;

        public List<MyEntity> Entities = new List<MyEntity>();
        public List<MyPlanet> Planets = new List<MyPlanet>();
        private Func<IMyEntity, bool> entityFilterCached;

        private const int SKIP_PLANETS = 60 * 10;

        public override void LoadData()
        {
            Instance = this;
            Log.SetUp("Enhanced Gravity Generators", 464877997, "EnhancedGravityGenerators");

            entityFilterCached = new Func<IMyEntity, bool>(EntityFilter);
        }

        public override void BeforeStart()
        {
            Log.Init();
        }

        protected override void UnloadData()
        {
            Instance = null;
            Planets.Clear();
            Log.Close();
        }

        public override void UpdateBeforeSimulation()
        {
            try
            {
                if(++skipPlanets >= SKIP_PLANETS)
                {
                    skipPlanets = 0;
                    MyAPIGateway.Entities.GetEntities(null, entityFilterCached);
                }
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }

        private bool EntityFilter(IMyEntity ent)
        {
            var p = ent as MyPlanet;

            if(p != null)
                Planets.Add(p);

            return false; // don't add to the list, it's null
        }
    }
}