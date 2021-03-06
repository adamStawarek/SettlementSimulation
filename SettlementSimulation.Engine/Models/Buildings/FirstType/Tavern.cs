﻿using System;
using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Models.Buildings.SecondType;

namespace SettlementSimulation.Engine.Models.Buildings.FirstType
{
    [Epoch(Epoch.First)]
    public class Tavern : Building
    {
        public override double Probability => 0.07;
        public override int Space => 1;

        public override double CalculateFitness(BuildingRule model)
        {
            var maxResidencesPerTavern = Position.DistanceTo(model.SettlementCenter) < 15 ? 60 : 80;

            var residences = model.Roads
                .SelectMany(b => b.Buildings)
                .Where(r => r.Position.DistanceTo(this.Position) <= 10)
                .Count(b => b is Residence);
            var taverns = model.Roads
                .SelectMany(b => b.Buildings)
                .Where(r => r.Position.DistanceTo(this.Position) <= 10)
                .Count(b => b is Tavern);

            if (residences / (taverns + 1) < maxResidencesPerTavern)
            {
                return 0;
            }

            return 3;
        }
    }
}