﻿using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Models.Buildings;

namespace SettlementSimulation.Engine
{
    public class Dna : ICopyable<Dna>
    {
        #region fields
        private readonly Field[,] _fields;
        private readonly List<Point> _mainRoad;
        #endregion

        #region properties
        public List<IRoad> Genes { get; set; }
        public float Fitness { get; private set; }
        public Point SettlementCenter =>
            new Point((int)Genes.Average(g => g.Center.X), (int)Genes.Average(g => g.Center.Y));
        #endregion

        public Dna(
            Field[,] fields,
            IEnumerable<Point> mainRoad,
            bool shouldInitGenes = true)
        {
            _fields = fields;
            _mainRoad = mainRoad.ToList();
            _mainRoad.ForEach(p => _fields[p.X, p.Y].IsBlocked = true);
            Genes = new List<IRoad>();

            if (!shouldInitGenes) return;
            InitializeGenes();
        }

        private void InitializeGenes()
        {
            var minRadius = _fields.GetLength(0) / 100 < 10 ? 10 : _fields.GetLength(0) / 100;
            var maxRadius = _fields.GetLength(0) / 10 < 10 ? 10 : _fields.GetLength(0) / 10;
            var settlementFields = _fields.ToList()
                .Where(f => f.InSettlement &&
                            f.Position.X > maxRadius &&
                            f.Position.X < _fields.GetLength(0) - maxRadius &&
                            f.Position.Y > maxRadius &&
                            f.Position.Y < _fields.GetLength(1) - maxRadius)
                .ToList();

            Point center = new Point(-1, -1);
            int radius = -1;
            for (int r = maxRadius; r >= minRadius; r--)
            {
                Field centerField = settlementFields
                    .FirstOrDefault(f =>
                        f.Position.GetCircularPoints(r, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 2.0, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 4.0, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 6.0, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 8.0, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 10.0, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement));

                if (centerField != null)
                {
                    radius = r;
                    center = centerField.Position;
                    break;
                }

                if (r == minRadius)
                {
                    throw new Exception("Cannot find center for initial point");
                }
            }

            var roadGenerator = new RoadPointsGenerator();
            var initialRoadsCount = 3;

            var initialRoads = new List<IRoad>(initialRoadsCount);
            var firstRoadPoints = roadGenerator.GenerateStraight(new RoadGenerationTwoPoints()
            {
                Start = new Point(center.X - radius / 2, center.Y),
                End = new Point(center.X + radius / 2, center.Y),
                Fields = _fields
            });
            initialRoads.Add(new Road(firstRoadPoints));
            AddRoad(initialRoads.First());

            while (initialRoads.Count != initialRoadsCount)
            {
                var roadToAttach = initialRoads[RandomProvider.Next(initialRoads.Count)];
                var roadPoints = roadGenerator.GenerateAttached(new RoadGenerationAttached()
                {
                    Road = roadToAttach,
                    Roads = initialRoads,
                    Fields = _fields
                }).ToList();

                if (!roadPoints.Any()) continue;

                var newRoad = new Road(roadPoints);
                initialRoads.Add(newRoad);
                AddRoad(newRoad);
            }
        }

        public float CalculateFitness(Epoch epoch, int generation)
        {
            //TODO
            return 0;
        }

        public Dna Crossover(Dna otherParent, Epoch epoch)
        {
            return this;
        }

        public void Mutate(Epoch epoch, float mutationRate = 0.01F)
        {
            //TODO
        }

        public ISettlementStructure AddNewSettlementStructure(Epoch epoch, Action setNextEpoch)
        {
            if (Genes.All(r => r.Buildings.Count > 0.25 * r.Length) ||
                Genes.All(r => r.GetPossiblePositionsToAttachBuilding(this.Genes).Count == 0))
            {
                var roadToAttach = this.Genes[RandomProvider.Next(this.Genes.Count)];
                var road = this.CreateNewRoad(roadToAttach);
                this.AddRoad(road);
                return CanAddRoad(road) ? road : null;
            }

            if (Genes.Sum(g => g.Buildings.Count) < EpochSpecific.GetBuildingsCount(epoch))
            {
                var roadToAttach = this.Genes
                    .Where(g => g.GetPossiblePositionsToAttachBuilding(this.Genes).Count > 0)
                    .OrderBy(r => r.Buildings.Count)
                    .First();
                var building = this.CreateNewBuilding(roadToAttach, epoch);
                roadToAttach.AddBuilding(building);
                return building;
            }

            if (epoch != Epoch.Third)
            {
                setNextEpoch.Invoke();
            }

            return null;
        }

        public Dna Copy()
        {
            var copy = new Dna(_fields, _mainRoad, false);
            Genes.Cast<ICopyable<Road>>().ToList().ForEach(g => copy.Genes.Add(g.Copy()));
            copy.Fitness = this.Fitness;
            return copy;
        }

        private IRoad CreateNewRoad(IRoad road)
        {
            var roadGenerator = new RoadPointsGenerator();
            var roadPoints = roadGenerator.GenerateAttached(new RoadGenerationAttached()
            {
                Road = road,
                Roads = this.Genes,
                Fields = this._fields,
                SettlementCenter = this.SettlementCenter,
                MinDistanceBetweenRoads = 10
            }).ToList();

            return new Road(roadPoints);
        }

        private IBuilding CreateNewBuilding(IRoad road, Epoch epoch)
        {
            var positions = road.GetPossiblePositionsToAttachBuilding(this.Genes);
            if (!positions.Any()) return null;

            var building = Building.GetRandom(epoch);
            building.Position = positions[RandomProvider.Next(positions.Count)];

            return building;
        }

        private void AddRoad(IRoad road)
        {
            if (!CanAddRoad(road)) return;

            foreach (var segment in road.Segments)
            {
                _fields[segment.Position.X, segment.Position.Y].IsBlocked = true;
            }

            foreach (var building in road.Buildings)
            {
                _fields[building.Position.X, building.Position.Y].IsBlocked = true;
            }
            this.Genes.Add(road);
        }

        private bool CanAddRoad(IRoad road)
        {
            if (!road.Segments.Any())
                return false;

            if (this.Genes.Any(g => g.Start.DistanceTo(road.Start) < 2 ||
                                    g.Start.DistanceTo(road.End) < 2 ||
                                    g.End.DistanceTo(road.Start) < 2 ||
                                    g.End.DistanceTo(road.End) < 2))
                return false;

            if (road.Segments.Any(s => !this._fields[s.Position.X, s.Position.Y].InSettlement ||
                                       (this._fields[s.Position.X, s.Position.Y].IsBlocked.HasValue &&
                                        this._fields[s.Position.X, s.Position.Y].IsBlocked.Value)))
            {
                return false;
            }

            if (road.Buildings.Any(b => !this._fields[b.Position.X, b.Position.Y].InSettlement ||
                                        (this._fields[b.Position.X, b.Position.Y].IsBlocked.HasValue &&
                                         this._fields[b.Position.X, b.Position.Y].IsBlocked.Value)))
            {
                return false;
            }

            return true;
        }
    }
}