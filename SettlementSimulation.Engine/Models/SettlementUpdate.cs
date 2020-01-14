using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine.Models
{
    public class SettlementUpdate
    {
        public SettlementUpdate()
        {
            NewRoads = new List<IRoad>();
            NewBuildingsAttachedToRoad = new List<(IBuilding, IRoad)>();
            RemovedRoads = new List<IRoad>();
            BuildingRemovedFromRoad = new List<(IBuilding, IRoad)>();
        }

        public List<IRoad> NewRoads { get; set; }
        public List<(IBuilding building, IRoad road)> NewBuildingsAttachedToRoad { get; set; }
        public List<IRoad> RemovedRoads { get; set; }
        public List<(IBuilding building, IRoad road)> BuildingRemovedFromRoad { get; set; }

        public SettlementUpdate Crossover(SettlementUpdate other)
        {
            var child = new SettlementUpdate();
            foreach (var road in this.NewRoads.Concat(other.NewRoads))
            {
                var buildingsToRemove = road.Buildings.Where(b => !(b is Residence) && b.Fitness == 0).ToList();
                buildingsToRemove.ForEach(b => road.RemoveBuilding(b));
                child.NewRoads.Add(road);
            }

            foreach (var (b,road) in this.NewBuildingsAttachedToRoad.Concat(other.NewBuildingsAttachedToRoad))
            {
                if (!(b is Residence) && b.Fitness == 0) continue;
                child.NewBuildingsAttachedToRoad.Add((b, road));
            }

            foreach (var (b, road) in this.BuildingRemovedFromRoad.Concat(other.BuildingRemovedFromRoad))
            {
                child.BuildingRemovedFromRoad.Add((b, road));
            }

            return child;
        }
    }
}