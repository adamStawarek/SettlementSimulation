using NUnit.Framework;
using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.AreaGenerator.Tests.Unit
{
    public class BuilderHelperTests
    {
        private BuilderHelper _builderHelper;

        [SetUp]
        public void Setup()
        {
            _builderHelper = new BuilderHelper();
        }

        [Test]
        public void GetBoundaryPoints_WithPointList()
        {
            //change:
            //111
            //111
            //111
            //to:
            //111
            //1-1
            //111
            var points = new List<Point>
           {
               new Point(0,0),
               new Point(1,0),
               new Point(2,0),

               new Point(0,1),
               new Point(1,1),
               new Point(2,1),

               new Point(0,2),
               new Point(1,2),
               new Point(2,2),
           };

            var expectedPoints = new List<Point>
            {
                new Point(0,0),
                new Point(1,0),
                new Point(2,0),

                new Point(0,1),
                new Point(2,1),

                new Point(0,2),
                new Point(1,2),
                new Point(2,2),
            };
            var boundaryPoints = _builderHelper.GetBoundaryPoints(points);

            CollectionAssert.AreEquivalent(expectedPoints, boundaryPoints);
        }

        [Test]
        public void GetBoundaryPoints_WithPointMatrix()
        {
            //change:
            //111
            //111
            //111
            //to:
            //111
            //1-1
            //111
            var points = new[,]
            {
                {1,1,1 },
                {1,1,1 },
                {1,1,1}
            };


            var expectedPoints = new List<Point>
            {
                new Point(0,0),
                new Point(1,0),
                new Point(2,0),

                new Point(0,1),
                new Point(2,1),

                new Point(0,2),
                new Point(1,2),
                new Point(2,2),
            };
            var boundaryPoints = _builderHelper.GetBoundaryPoints(points);

            CollectionAssert.AreEquivalent(expectedPoints, boundaryPoints);
        }
    }
}