using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine.Models
{
    public class SettlementUpdate
    {
        public SettlementUpdate(UpdateType updateType)
        {
            UpdateType = updateType;
            NewRoads = new List<IRoad>();
            NewBuildings = new List<IBuilding>();
            UpdatedBuildings = new List<(IBuilding oldBuilding, IBuilding newBuilding)>();
            UpdatedRoads = new List<(IRoad oldRoad, IRoad newRoad)>();
        }

        public UpdateType UpdateType { get; }
        public List<IRoad> NewRoads { get; set; }
        public List<IBuilding> NewBuildings { get; set; }
        public List<(IBuilding oldBuilding, IBuilding newBuilding)> UpdatedBuildings { get; set; }
        public List<(IRoad oldRoad, IRoad newRoad)> UpdatedRoads { get; set; }
        public MutationResult FloodMutationResult { get; set; }
        public MutationResult EarthquakeMutationResult { get; set; }

        public SettlementUpdate Crossover(SettlementUpdate other)
        {
            if (this.UpdateType != other.UpdateType)
                throw new ArgumentException("In crossover both parents must be of the same settlement update type");

            var child = new SettlementUpdate(this.UpdateType);

            switch (UpdateType)
            {
                case UpdateType.NewRoads:
                    foreach (var road in this.NewRoads.Concat(other.NewRoads))
                    {
                        if (child.NewRoads.Any(r => r.IsCrossed(road)))
                            continue;
                        if (road.IsVertical)
                        {
                            if (child.NewRoads.Where(g => g.IsVertical).Any(g => Math.Abs(g.Start.X - road.Start.X) <= 2 &&
                                                                             g.Segments.Any(s => road.Segments.Any(r => r.Position.Y == s.Position.Y))))
                                continue;
                        }
                        else
                        {
                            if (child.NewRoads.Where(g => !g.IsVertical).Any(g => Math.Abs(g.Start.Y - road.Start.Y) <= 2 &&
                                                                              g.Segments.Any(s => road.Segments.Any(r => r.Position.X == s.Position.X))))
                                continue;
                        }

                        var buildingsToRemove = road.Buildings.Where(b => !(b is Residence) && Math.Abs(b.Fitness.Value) < 0.1).ToList();
                        buildingsToRemove.ForEach(b =>
                        {
                            road.RemoveBuilding(b);
                            road.AddBuilding(new Residence()//otherwise some roads would have no buildings
                                {Position = b.Position, Road = road, Direction = b.Direction});
                        });
                        if (!road.Buildings.Any())
                            continue;
                        child.NewRoads.Add(road);
                    }
                    break;
                case UpdateType.NewBuildings:
                    foreach (var b in this.NewBuildings.Concat(other.NewBuildings))
                    {
                        if ((!(b is Residence) && Math.Abs(b.Fitness.Value) < 0.1))
                            continue;
                        if (child.NewBuildings.Any(nb => nb.Position.Equals(b.Position)))
                            continue;
                        child.NewBuildings.Add(b);
                    }
                    break;
                case UpdateType.NewTypes:
                    foreach (var b in this.UpdatedBuildings.Concat(other.UpdatedBuildings))
                    {
                        if ((!(b.newBuilding is Residence) && (int)b.newBuilding.Fitness <= 0))
                            continue;
                        if ((int)b.newBuilding.Fitness <= 0)
                            continue;
                        if (child.UpdatedBuildings.Any(nb => nb.newBuilding.Position.Equals(b.newBuilding.Position)))
                            continue;
                        child.UpdatedBuildings.Add(b);
                    }

                    foreach (var r in this.UpdatedRoads.Concat(other.UpdatedRoads))
                    {
                        if (child.UpdatedRoads.Any(ur => ur.oldRoad == r.oldRoad))
                            continue;
                        child.UpdatedRoads.Add(r);
                    }
                    break;

            }
            return child;
        }
    }
}