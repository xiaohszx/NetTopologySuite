using System;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NUnit.Framework;

namespace NetTopologySuite.Tests.NUnit.Algorithm
{
    //[Ignore("The Minimum Bounding Circle logic does not look to have been included in NTS as yet")]
    public class MinimumBoundingCircleTest : GeometryTestCase
    {
        private readonly GeometryFactory _geometryFactory;
        private readonly WKTReader _reader;

        public MinimumBoundingCircleTest()
        {
            var gs = new NtsGeometryServices(PrecisionModel.Fixed.Value, 0);
            _geometryFactory = gs.CreateGeometryFactory();
            _reader = new WKTReader(gs);
        }

        [Test]
        public void TestEmptyPoint()
        {
            DoMinimumBoundingCircleTest("POINT EMPTY", "MULTIPOINT EMPTY");
        }

        [Test]
        public void TestPoint()
        {
            DoMinimumBoundingCircleTest("POINT (10 10)", "MULTIPOINT ((10 10))", new Coordinate(10, 10), 0);
        }

        [Test]
        public void TestPoints2()
        {
            DoMinimumBoundingCircleTest("MULTIPOINT ((10 10), (20 20))", "MULTIPOINT ((10 10), (20 20))", new Coordinate(15, 15), 7.0710678118654755);
        }

        [Test]
        public void TestPointsInLine()
        {
            DoMinimumBoundingCircleTest("MULTIPOINT ((10 10), (20 20), (30 30))", "MULTIPOINT ((10 10), (30 30))",
            new Coordinate(20, 20), 14.142135623730951);
        }

        [Test]
        public void TestPoints3()
        {
            DoMinimumBoundingCircleTest("MULTIPOINT ((10 10), (20 20), (10 20))", "MULTIPOINT ((10 10), (20 20), (10 20))",
            new Coordinate(15, 15), 7.0710678118654755);
        }

        [Test]
        public void TestObtuseTriangle()
        {
            DoMinimumBoundingCircleTest("POLYGON ((100 100, 200 100, 150 90, 100 100))", "MULTIPOINT ((100 100), (200 100))",
                new Coordinate(150, 100), 50);
        }

        [Test]
        public void TestTriangleWithMiddlePoint()
        {
            DoMinimumBoundingCircleTest("MULTIPOINT ((10 10), (20 20), (10 20), (15 19))", "MULTIPOINT ((10 10), (20 20), (10 20))",
                new Coordinate(15, 15), 7.0710678118654755);
        }

        [Test]
        public void TestQuadrilateral() 
        {
            DoMinimumBoundingCircleTest("POLYGON ((26426 65078, 26531 65242, 26096 65427, 26075 65136, 26426 65078))", "MULTIPOINT ((26531 65242), (26075 65136), (26096 65427))",
        new Coordinate(26284.84180271327, 65267.114509082545), 247.4360455914027 );
    }

        [Test]
        public void TestMaxDiameterLine()
        {
            DoMaxDiameterTest("LINESTRING (100 200, 300 100)", "LINESTRING (100 200, 300 100)");
        }

        [Test]
        public void TestMaxDiameterPolygon()
        {
            DoMaxDiameterTest("POLYGON ((100 200, 300 150, 110 100, 100 200))", "LINESTRING (300 150, 100 200)");
            DoMaxDiameterTest("POLYGON ((110 200, 300 150, 100 100, 110 200))", "LINESTRING (300 150, 100 100)");
            DoMaxDiameterTest("POLYGON ((0 0, 6 0, 5 5, 0 0))", "LINESTRING (5 5, 0 0)");
        }

        static double TOLERANCE = 1.0e-5;

        private void DoMaxDiameterTest(string wkt, string expectedWKT)
        {
            var mbc = new MinimumBoundingCircle(Read(wkt));
            var diamActual = mbc.GetMaximumDiameter();
            var expected = Read(expectedWKT);

            CheckEqual(expected, diamActual);
        }


        private void DoMinimumBoundingCircleTest(string wkt, string expectedWKT)
        {
            DoMinimumBoundingCircleTest(wkt, expectedWKT, null, -1);
        }

        private void DoMinimumBoundingCircleTest(string wkt, string expectedWKT, Coordinate expectedCentre, double expectedRadius)
        {
            var mbc = new MinimumBoundingCircle(_reader.Read(wkt));
            var exPts = mbc.GetExtremalPoints();
            Geometry actual = _geometryFactory.CreateMultiPointFromCoords(exPts);
            double actualRadius = mbc.GetRadius();
            var actualCentre = mbc.GetCentre();
            //TestContext.WriteLine("   Centre = " + actualCentre + "   Radius = " + actualRadius);

            var expected = _reader.Read(expectedWKT);
            bool isEqual = actual.Equals(expected);
            // need this hack because apparently equals does not work for MULTIPOINT EMPTY
            if (actual.IsEmpty && expected.IsEmpty)
                isEqual = true;
            if (!isEqual)
            {
                TestContext.WriteLine("Actual = " + actual + ", Expected = " + expected);
            }
            Assert.IsTrue(isEqual);

            if (expectedCentre != null)
            {
                Assert.IsTrue(expectedCentre.Distance(actualCentre) < TOLERANCE);
            }
            if (expectedRadius >= 0)
            {
                Assert.IsTrue(Math.Abs(expectedRadius - actualRadius) < TOLERANCE);
            }
        }
    }
}
