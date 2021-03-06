﻿using System;
using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Models.Buildings.SecondType;

namespace SettlementSimulation.Engine.Models.Buildings.ThirdType
{
    [Epoch(Epoch.Third)]
    public class University : Building
    {
        public override double Probability => 0.005;
        public override int Space => 2;

        public override double CalculateFitness(BuildingRule model)
        {
            var universities = model.Roads.SelectMany(b => b.Buildings).Count(b => b is University);
            var schools = model.Roads.SelectMany(b => b.Buildings).Count(b => b is School);
            if (schools / (universities+1) < 10)
            {
                //("No more than one school per 100 residences");
                return 0;
            }

            return 15;
        }
    }
}