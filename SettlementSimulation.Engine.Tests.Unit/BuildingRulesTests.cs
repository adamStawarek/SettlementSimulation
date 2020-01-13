using System.Collections.Generic;
using NUnit.Framework;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.AreaGenerator.Models.Terrains;
using SettlementSimulation.Engine.Helpers;
using System.Linq;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models.Buildings.SecondType;
using SettlementSimulation.Engine.Models.Buildings.ThirdType;

namespace SettlementSimulation.Engine.Tests.Unit
{
    public class BuildingRulesTests
    {
        private Settlement _settlement;

        [SetUp]
        public void SetUp()
        {
            var fields = new Field[100, 100];
            int waterLevel = 10;

            for (int i = 0; i < fields.GetLength(0); i++)
            {
                for (int j = 0; j < fields.GetLength(1); j++)
                {
                    var position = new Point(i, j);
                    if (j < waterLevel)
                    {
                        fields[i, j] = new Field()
                        {
                            InSettlement = false,
                            Position = position,
                            Terrain = new Water()
                        };
                    }
                    else
                    {
                        fields[i, j] = new Field()
                        {
                            InSettlement = true,
                            Position = position,
                            Terrain = new Lowland(),
                            DistanceToWater = position.Y - waterLevel,
                            DistanceToMainRoad = position.Y - waterLevel - 1
                        };
                    }
                }
            }

            var roadGenerator = new RoadPointsGenerator();
            var mainRoad = roadGenerator.GenerateStraight(new RoadGenerationTwoPoints()
            {
                Start = new Point(0, waterLevel),
                End = new Point(fields.GetLength(0) - 1, waterLevel),
                Fields = fields
            });//in this case main road should be straight horizontal line

            _settlement = new Settlement(fields, mainRoad, false);

            var road1 = new Road(roadGenerator.GenerateStraight(new RoadGenerationTwoPoints()
            {
                Start = new Point(0, 50),
                End = new Point(99, 50),
                Fields = _settlement.Fields
            }));
            _settlement.AddRoad(road1);
        }

        [Test]
        public void MarketRule_IsSatisfied_When_No_Other_Markets()
        {
            //add randomly 100 residences
            IRoad road = _settlement.Genes.First();
            var market = new Market()
            {
                Position = new Point(50, 51)
            };
            _settlement.AddBuildingToRoad(road, market);

            while (_settlement.Genes.SelectMany(g => g.Buildings).Count() < 120)
            {
                var buildingPositions = road.GetPossibleBuildingPositions(new PossibleBuildingPositions(_settlement.Genes, _settlement.Fields));
                if (!buildingPositions.Any())
                    continue;

                var building = new Residence { Position = buildingPositions[RandomProvider.Next(buildingPositions.Count)] };
                _settlement.AddBuildingToRoad(road, building);
            }


            Assert.AreEqual(5,market.GetFitness(new BuildingRule()
            {
                BuildingRoad = road,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void MarketRule_Is_Not_Satisfied_When_Exists_Other_Market_Which_Is_Less_Than_50_Pixels_Away()
        {
            //add randomly 100 residences
            IRoad road = _settlement.Genes.First();
            var market1 = new Market()
            {
                Position = new Point(50, 51)
            };
            _settlement.AddBuildingToRoad(road, market1);
            var market2 = new Market()
            {
                Position = new Point(90, 51)
            };
            _settlement.AddBuildingToRoad(road, market2);

            while (_settlement.Genes.SelectMany(g => g.Buildings).Count() < 120)
            {
                var buildingPositions = road.GetPossibleBuildingPositions(new PossibleBuildingPositions(_settlement.Genes, _settlement.Fields));
                if (!buildingPositions.Any())
                    continue;

                var building = new Residence { Position = buildingPositions[RandomProvider.Next(buildingPositions.Count)] };
                _settlement.AddBuildingToRoad(road, building);
            }


            Assert.AreEqual(0, market1.GetFitness(new BuildingRule()
            {
                BuildingRoad = road,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void MarketRule_IsSatisfied_When_Exists_Other_Market_But_It_Further_Than_50_Pixels_Away()
        {
            IRoad road1 = _settlement.Genes.First();
            var market1 = new Market()
            {
                Position = new Point(50, 51)
            };
            _settlement.AddBuildingToRoad(road1, market1);

            //add vertical road to the end of the road to increase distance between markets
            var roadGenerator = new RoadPointsGenerator();
            var road2 = new Road(roadGenerator.GenerateStraight(new RoadGenerationTwoPoints()
            {
                Start = new Point(98, 51),
                End = new Point(98, 21),
                Fields = _settlement.Fields
            }));
            _settlement.AddRoad(road2);
            var market2 = new Market()
            {
                Position = new Point(99, 21)
            };
            _settlement.AddBuildingToRoad(road1, market2);


            while (_settlement.Genes.SelectMany(g => g.Buildings).Count() < 120)
            {
                var buildingPositions = road1.GetPossibleBuildingPositions(new PossibleBuildingPositions(_settlement.Genes, _settlement.Fields));
                if (!buildingPositions.Any())
                    continue;

                var building = new Residence { Position = buildingPositions[RandomProvider.Next(buildingPositions.Count)] };
                _settlement.AddBuildingToRoad(road1, building);
            }

            Assert.AreEqual(5,market1.GetFitness(new BuildingRule()
            {
                BuildingRoad = road1,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void SchoolRule_IsSatisfied__When_Ratio_Schools_Per_Residences_Is_Less_Than_1_To_100()
        {
            IRoad road = _settlement.Genes.First();
            var school = new School()
            {
                Position = new Point(50, 51)
            };

            var buildingPositions = road.GetPossibleBuildingPositions(new PossibleBuildingPositions(_settlement.Genes, _settlement.Fields));
            buildingPositions.Remove(school.Position);

            while (_settlement.Genes.SelectMany(g => g.Buildings).Count() < 120)
            {
                var position = buildingPositions[RandomProvider.Next(buildingPositions.Count)];
                var building = new Residence { Position = position };
                _settlement.AddBuildingToRoad(road, building);
            }

            Assert.AreEqual(5,school.GetFitness(new BuildingRule()
            {
                BuildingRoad = road,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void SchoolRule_Is_Not_Satisfied__When_Ratio_Schools_Per_Residences_Is_Greater_Than_1_To_100()
        {
            IRoad road = _settlement.Genes.First();
            var school1 = new School()
            {
                Position = new Point(50, 51)
            };
            _settlement.AddBuildingToRoad(road, school1);

            while (_settlement.Genes.SelectMany(g => g.Buildings).Count() < 120)
            {
                var buildingPositions = road.GetPossibleBuildingPositions(new PossibleBuildingPositions(_settlement.Genes, _settlement.Fields));
                if (!buildingPositions.Any())
                    continue;

                var building = new Residence { Position = buildingPositions[RandomProvider.Next(buildingPositions.Count)] };
                _settlement.AddBuildingToRoad(road, building);
            }

            var school2 = new School()
            {
                Position = new Point(80, 51)
            };

            Assert.AreEqual(0, school2.GetFitness(new BuildingRule()
            {
                BuildingRoad = road,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void ChurchRule_IsSatisfied_When_There_Are_More_Than_100_Residences_And_No_Other_Churches_In_Neighborhood()
        {
            IRoad road = _settlement.Genes.First();
            var church = new Church()
            {
                Position = new Point(50, 51)
            };

            var buildingPositions = road.GetPossibleBuildingPositions(new PossibleBuildingPositions(_settlement.Genes, _settlement.Fields));
            buildingPositions.Remove(church.Position);

            while (_settlement.Genes.SelectMany(g => g.Buildings).Count() < 120)
            {
                var position = buildingPositions[RandomProvider.Next(buildingPositions.Count)];
                var building = new Residence { Position = position };
                _settlement.AddBuildingToRoad(road, building);
            }

            Assert.AreEqual(5,church.GetFitness(new BuildingRule()
            {
                BuildingRoad = road,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void ChurchRule_Is_Not_Satisfied_When_There_Are_More_Less_100_ResidencesAnd_In_Neighborhood()
        {
            IRoad road = _settlement.Genes.First();
            var church = new Church()
            {
                Position = new Point(50, 51)
            };

            var buildingPositions = road.GetPossibleBuildingPositions(new PossibleBuildingPositions(_settlement.Genes, _settlement.Fields));
            buildingPositions.Remove(church.Position);

            while (_settlement.Genes.SelectMany(g => g.Buildings).Count() < 10)
            {
                var position = buildingPositions[RandomProvider.Next(buildingPositions.Count)];
                var building = new Residence { Position = position };
                _settlement.AddBuildingToRoad(road, building);
            }

            Assert.AreEqual(0, church.GetFitness(new BuildingRule()
            {
                BuildingRoad = road,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void ChurchRule_Is_Not_Satisfied_When_There_Is_Other_Church_In_Neighborhood()
        {
            IRoad road = _settlement.Genes.First();
            var church = new Church()
            {
                Position = new Point(50, 51)
            };

            var existingChurch = new Church()
            {
                Position = new Point(70, 51)
            };
            _settlement.AddBuildingToRoad(road, existingChurch);

            var buildingPositions = road.GetPossibleBuildingPositions(new PossibleBuildingPositions(_settlement.Genes, _settlement.Fields));
            buildingPositions.Remove(church.Position);

            while (_settlement.Genes.SelectMany(g => g.Buildings).Count() < 10)
            {
                var position = buildingPositions[RandomProvider.Next(buildingPositions.Count)];
                var building = new Residence { Position = position };
                _settlement.AddBuildingToRoad(road, building);
            }

            Assert.AreEqual(0, church.GetFitness(new BuildingRule()
            {
                BuildingRoad = road,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void AdministrationRule_IsSatisfied_When_Distance_To_SettlementCenter_Is_Less_Than_40_Pixels()
        {
            IRoad road = _settlement.Genes.First();
            var administration = new Administration()
            {
                Position = new Point(50, 51)
            };

            var buildingPositions = road.GetPossibleBuildingPositions(new PossibleBuildingPositions(_settlement.Genes, _settlement.Fields));
            buildingPositions.Remove(administration.Position);

            while (_settlement.Genes.SelectMany(g => g.Buildings).Count() < 120)
            {
                var position = buildingPositions[RandomProvider.Next(buildingPositions.Count)];
                var building = new Residence { Position = position };
                _settlement.AddBuildingToRoad(road, building);
            }

            Assert.AreEqual(10,administration.GetFitness(new BuildingRule()
            {
                BuildingRoad = road,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void AdministrationRule_Is_Not_Satisfied_When_Distance_To_SettlementCenter_Is_Greater_Than_40_Pixels()
        {
            IRoad road = _settlement.Genes.First();
            var administration = new Administration()
            {
                Position = new Point(99, 51)
            };

            var buildingPositions = road.GetPossibleBuildingPositions(new PossibleBuildingPositions(_settlement.Genes, _settlement.Fields));
            buildingPositions.Remove(administration.Position);

            while (_settlement.Genes.SelectMany(g => g.Buildings).Count() < 120)
            {
                var position = buildingPositions[RandomProvider.Next(buildingPositions.Count)];
                var building = new Residence { Position = position };
                _settlement.AddBuildingToRoad(road, building);
            }

            Assert.AreEqual(0, administration.GetFitness(new BuildingRule()
            {
                BuildingRoad = road,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void PortRule_IsSatisfied_When_Distance_To_Water_Is_Less_Than_10_Pixels_And_There_Is_No_Other_Port()
        {
            IRoad road1 = _settlement.Genes.First();
            var roadGenerator = new RoadPointsGenerator();
            var road2 = new Road(roadGenerator.GenerateStraight(new RoadGenerationTwoPoints()
            {
                Start = new Point(50, 51),
                End = new Point(50, 10),
                Fields = _settlement.Fields
            }));
            _settlement.AddRoad(road2);

            var port = new Port()
            {
                Position = new Point(51, 10)
            };

            Assert.AreEqual(15,port.GetFitness(new BuildingRule()
            {
                BuildingRoad = road1,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void PortRule_Is_Not_Satisfied_When_Distance_To_Water_Is_Greater_Than_10_Pixels()
        {
            var road1 = _settlement.Genes.First();
            var roadGenerator = new RoadPointsGenerator();
            var road2 = new Road(roadGenerator.GenerateStraight(new RoadGenerationTwoPoints()
            {
                Start = new Point(50, 51),
                End = new Point(50, 10),
                Fields = _settlement.Fields
            }));
            _settlement.AddRoad(road2);

            var port = new Port()
            {
                Position = new Point(51, 25)
            };

            Assert.AreEqual(0, port.GetFitness(new BuildingRule()
            {
                BuildingRoad = road1,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void PortRule_Is_Not_Satisfied_When_There_Is_Other_Port()
        {
            IRoad road1 = _settlement.Genes.First();
            var roadGenerator = new RoadPointsGenerator();
            var road2 = new Road(roadGenerator.GenerateStraight(new RoadGenerationTwoPoints()
            {
                Start = new Point(50, 51),
                End = new Point(50, 10),
                Fields = _settlement.Fields
            }));
            _settlement.AddRoad(road2);

            var port = new Port()
            {
                Position = new Point(51, 10)
            };

            var exitingPort = new Port()
            {
                Position = new Point(49, 10)
            };
            _settlement.AddBuildingToRoad(road2, exitingPort);

            Assert.AreEqual(0, port.GetFitness(new BuildingRule()
            {
                BuildingRoad = road1,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void UniversityRule_IsSatisfied__When_Ratio_Universities_Per_Schools_Is_Less_Than_1_To_5()
        {
            IRoad road = _settlement.Genes.First();

            var schools = new List<School>(5)
            {
                new School()
                {
                    Position = new Point(50, 51)
                },
                new School()
                {
                    Position = new Point(55, 51)
                },
                new School()
                {
                    Position = new Point(60, 51)
                },
                new School()
                {
                    Position = new Point(65, 51)
                },
                new School()
                {
                    Position = new Point(70, 51)
                }
            };

            schools.ForEach(s => _settlement.AddBuildingToRoad(road, s));

            var university = new University()
            {
                Position = new Point(50, 49)
            };

            Assert.AreEqual(15,university.GetFitness(new BuildingRule()
            {
                BuildingRoad = road,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }

        [Test]
        public void UniversityRule_Is_Not_Satisfied__When_Ratio_Universities_Per_Schools_Is_Greater_Than_1_To_5()
        {
            IRoad road = _settlement.Genes.First();

            var schools = new List<School>(4)
            {
                new School()
                {
                    Position = new Point(50, 51)
                },
                new School()
                {
                    Position = new Point(55, 51)
                },
                new School()
                {
                    Position = new Point(60, 51)
                },
                new School()
                {
                    Position = new Point(65, 51)
                }
            };

            schools.ForEach(s => _settlement.AddBuildingToRoad(road, s));

            var university = new University()
            {
                Position = new Point(50, 49)
            };

            Assert.AreEqual(0, university.GetFitness(new BuildingRule()
            {
                BuildingRoad = road,
                Fields = _settlement.Fields,
                Roads = _settlement.Genes,
                SettlementCenter = _settlement.SettlementCenter
            }));
        }
    }
}