using System;
using NetTopologySuite.IO;
using NetTopologySuite.Operation.Distance;

namespace NetTopologySuite.Samples.Operation.Distance
{
    /// <summary>
    /// Example of computing distance and closest points between geometries
    /// using the DistanceOp class.
    /// </summary>
    public class ClosestPointExample
    {
        //private static GeometryFactory fact;
        private readonly WKTReader wktRdr = new WKTReader();

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        public static void main(string[] args)
        {
            var example = new ClosestPointExample();
            example.Run();
        }

        /// <summary>
        ///
        /// </summary>
        public ClosestPointExample() { }

        /// <summary>
        ///
        /// </summary>
        public virtual void  Run()
        {
            FindClosestPoint("POLYGON ((200 180, 60 140, 60 260, 200 180))", "POINT (140 280)");
            FindClosestPoint("POLYGON ((200 180, 60 140, 60 260, 200 180))", "MULTIPOINT (140 280, 140 320)");
            FindClosestPoint("LINESTRING (100 100, 200 100, 200 200, 100 200, 100 100)", "POINT (10 10)");
            FindClosestPoint("LINESTRING (100 100, 200 200)", "LINESTRING (100 200, 200 100)");
            FindClosestPoint("LINESTRING (100 100, 200 200)", "LINESTRING (150 121, 200 0)");
            FindClosestPoint("POLYGON (( 76 185, 125 283, 331 276, 324 122, 177 70, 184 155, 69 123, 76 185 ), ( 267 237, 148 248, 135 185, 223 189, 251 151, 286 183, 267 237 ))", "LINESTRING ( 153 204, 185 224, 209 207, 238 222, 254 186 )");
            FindClosestPoint("POLYGON (( 76 185, 125 283, 331 276, 324 122, 177 70, 184 155, 69 123, 76 185 ), ( 267 237, 148 248, 135 185, 223 189, 251 151, 286 183, 267 237 ))", "LINESTRING ( 120 215, 185 224, 209 207, 238 222, 254 186 )");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wktA"></param>
        /// <param name="wktB"></param>
        public virtual void  FindClosestPoint(string wktA, string wktB)
        {
            Console.WriteLine("-------------------------------------");
            try
            {
                var A = wktRdr.Read(wktA);
                var B = wktRdr.Read(wktB);
                Console.WriteLine("Geometry A: " + A);
                Console.WriteLine("Geometry B: " + B);
                var distOp = new DistanceOp(A, B);

                double distance = distOp.Distance();
                Console.WriteLine("Distance = " + distance);

                var closestPt = distOp.NearestPoints();
                var fact = A.Factory;
                var closestPtLine = fact.CreateLineString(closestPt);
                Console.WriteLine("Closest points: " + closestPtLine + " (distance = " + closestPtLine.Length + ")");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
