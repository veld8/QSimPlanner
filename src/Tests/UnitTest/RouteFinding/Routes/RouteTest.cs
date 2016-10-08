﻿using NUnit.Framework;
using QSP.RouteFinding.Containers;
using QSP.RouteFinding.Routes;
using System;
using System.Linq;

namespace UnitTest.RouteFinding.Routes
{
    [TestFixture]
    public class RouteTest
    {
        [Test]
        public void TotalDistanceEmptyRoute()
        {
            Assert.Throws<InvalidOperationException>(() =>
            new Route().GetTotalDistance());
        }

        [Test]
        public void GetTotalDistanceTest()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var y = new Waypoint("Y", 0.0, 1.0);
            var z = new Waypoint("Z", 1.0, 3.0);

            var route = new Route();
            route.AddLastWaypoint(x);
            route.AddLastWaypoint(y, "0");
            route.AddLastWaypoint(z, "1");

            var expectedDis = x.DistanceFrom(y) + y.DistanceFrom(z);

            Assert.AreEqual(expectedDis, route.GetTotalDistance());
        }

        [Test]
        public void GetTotalDistanceCustomDistance()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var y = new Waypoint("Y", 0.0, 1.0);
            var z = new Waypoint("Z", 1.0, 3.0);

            var route = new Route();
            route.AddLastWaypoint(x);
            route.AddLastWaypoint(y, "0", 100.0);
            route.AddLastWaypoint(z, "1", 200.0);

            Assert.AreEqual(300.0, route.GetTotalDistance(), 1E-8);
        }

        [Test]
        public void AddFirstWaypointTest1()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var y = new Waypoint("Y", 0.0, 1.0);

            var route = new Route();
            route.AddLastWaypoint(y);

            route.AddFirstWaypoint(x, "0", 100.0);

            Assert.IsTrue(x.Equals(route.FirstWaypoint));
            Assert.IsTrue("0" == route.First.Value.AirwayToNext);
            Assert.AreEqual(100.0, route.First.Value.DistanceToNext, 1E-8);
            Assert.IsTrue(y.Equals(route.First.Next.Value.Waypoint));
        }

        [Test]
        public void AddFirstWaypointTest1EmptyRoute()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var route = new Route();

            route.AddFirstWaypoint(x, "0", 100.0);

            Assert.IsTrue(x.Equals(route.FirstWaypoint));
            Assert.IsTrue("0" == route.First.Value.AirwayToNext);
            Assert.AreEqual(100.0, route.First.Value.DistanceToNext, 1E-8);
        }

        [Test]
        public void AddFirstWaypointTest2()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var y = new Waypoint("Y", 0.0, 1.0);

            var route = new Route();
            route.AddLastWaypoint(y);

            route.AddFirstWaypoint(x, "0");

            Assert.IsTrue(x.Equals(route.FirstWaypoint));
            Assert.IsTrue("0" == route.First.Value.AirwayToNext);
            Assert.AreEqual(
                x.DistanceFrom(y), route.First.Value.DistanceToNext, 1E-8);
            Assert.IsTrue(y.Equals(route.First.Next.Value.Waypoint));
        }

        [Test]
        public void AddFirstWaypointTest2EmptyRoute()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var route = new Route();

            route.AddFirstWaypoint(x, "0");

            Assert.IsTrue(x.Equals(route.FirstWaypoint));
            Assert.IsTrue("0" == route.First.Value.AirwayToNext);
        }

        [Test]
        public void AddLastWaypointTest1()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var y = new Waypoint("Y", 0.0, 1.0);

            var route = new Route();
            route.AddLastWaypoint(x);

            route.AddLastWaypoint(y, "0", 100.0);

            Assert.IsTrue(y.Equals(route.LastWaypoint));
            Assert.IsTrue("0" == route.Last.Previous.Value.AirwayToNext);
            Assert.AreEqual(
                100.0, route.Last.Previous.Value.DistanceToNext, 1E-8);
            Assert.IsTrue(x.Equals(route.Last.Previous.Value.Waypoint));
        }

        [Test]
        public void AddLastWaypointTest1EmptyRoute()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var route = new Route();

            route.AddLastWaypoint(x, "0", 100.0);

            Assert.IsTrue(x.Equals(route.LastWaypoint));
        }

        [Test]
        public void AddLastWaypointTest2()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var y = new Waypoint("Y", 0.0, 1.0);

            var route = new Route();
            route.AddLastWaypoint(x);

            route.AddLastWaypoint(y, "0");

            Assert.IsTrue(y.Equals(route.LastWaypoint));
            Assert.IsTrue("0" == route.Last.Previous.Value.AirwayToNext);
            Assert.AreEqual(
                x.DistanceFrom(y),
                route.Last.Previous.Value.DistanceToNext,
                1E-8);

            Assert.IsTrue(x.Equals(route.Last.Previous.Value.Waypoint));
        }

        [Test]
        public void AddLastWaypointTest2EmptyRoute()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var route = new Route();

            route.AddLastWaypoint(x, "0");

            Assert.IsTrue(x.Equals(route.LastWaypoint));
        }

        [Test]
        public void AddLastWaypointTest3()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var y = new Waypoint("Y", 0.0, 1.0);

            var route = new Route();
            route.AddFirstWaypoint(x, "0", 100.0);

            route.AddLastWaypoint(y);

            Assert.IsTrue(y.Equals(route.LastWaypoint));
            Assert.IsTrue("0" == route.Last.Previous.Value.AirwayToNext);
            Assert.AreEqual(
                100.0,
                route.Last.Previous.Value.DistanceToNext,
                1E-8);

            Assert.IsTrue(x.Equals(route.Last.Previous.Value.Waypoint));
        }

        [Test]
        public void AddLastWaypointTest3EmptyRoute()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var route = new Route();

            route.AddLastWaypoint(x);

            Assert.IsTrue(x.Equals(route.LastWaypoint));
        }

        [Test]
        public void AddLastRouteTest1()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var y = new Waypoint("Y", 0.0, 1.0);

            var route = new Route();
            route.AddLastWaypoint(x);

            route.AddLast(GetRoute1(), "0");

            Assert.IsTrue(x.Equals(route.FirstWaypoint));
            Assert.IsTrue("0" == route.First.Value.AirwayToNext);
            Assert.AreEqual(
                x.DistanceFrom(y),
                route.First.Value.DistanceToNext,
                1E-8);

            Assert.IsTrue(y.Equals(route.First.Next.Value.Waypoint));
        }

        [Test]
        public void AddLastRouteTest1EmptyRoute()
        {
            var y = new Waypoint("Y", 0.0, 1.0);
            var z = new Waypoint("Z", 0.0, 3.0);

            var route = new Route();

            route.AddLast(GetRoute1(), "0");

            Assert.IsTrue(y.Equals(route.FirstWaypoint));
            Assert.IsTrue("1" == route.First.Value.AirwayToNext);
            Assert.AreEqual(
                y.DistanceFrom(z),
                route.First.Value.DistanceToNext,
                1E-8);

            Assert.IsTrue(z.Equals(route.First.Next.Value.Waypoint));
        }

        [Test]
        public void AddLastRouteTest2()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var y = new Waypoint("Y", 0.0, 1.0);

            var route = new Route();
            route.AddLastWaypoint(x);

            route.AddLast(GetRoute1(), "0", 100.0);

            Assert.IsTrue(x.Equals(route.FirstWaypoint));
            Assert.IsTrue("0" == route.First.Value.AirwayToNext);
            Assert.AreEqual(
                100.0,
                route.First.Value.DistanceToNext,
                1E-8);

            Assert.IsTrue(y.Equals(route.First.Next.Value.Waypoint));
        }

        [Test]
        public void AddLastRouteTest2EmptyRoute()
        {
            var y = new Waypoint("Y", 0.0, 1.0);
            var z = new Waypoint("Z", 0.0, 3.0);

            var route = new Route();

            route.AddLast(GetRoute1(), "0", 100.0);

            Assert.IsTrue(y.Equals(route.FirstWaypoint));
            Assert.IsTrue("1" == route.First.Value.AirwayToNext);
            Assert.AreEqual(
                y.DistanceFrom(z),
                route.First.Value.DistanceToNext,
                1E-8);

            Assert.IsTrue(z.Equals(route.First.Next.Value.Waypoint));
        }

        private Route GetRoute1()
        {
            var route = new Route();

            route.AddLastWaypoint(new Waypoint("Y", 0.0, 1.0));
            route.AddLastWaypoint(new Waypoint("Z", 0.0, 3.0), "1");

            return route;
        }

        [Test]
        public void ConnectRouteTest()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var y = new Waypoint("Y", 0.0, 1.0);
            var z = new Waypoint("Z", 0.0, 3.0);

            var route = new Route();
            route.AddLastWaypoint(x);
            route.AddLastWaypoint(y, "0");

            var expected = new Route(route);

            route.ConnectRoute(GetRoute1());

            expected.AddLastWaypoint(z, "1");

            Assert.IsTrue(Enumerable.SequenceEqual(expected, route));
        }

        [Test]
        public void ConnectRouteEmptyNodes()
        {
            var route = new Route();
            route.ConnectRoute(GetRoute1());

            Assert.IsTrue(Enumerable.SequenceEqual(
                route, GetRoute1()));
        }

        [Test]
        public void ConnectRouteMismatchShouldThrowException()
        {
            var x = new Waypoint("X", 0.0, 0.0);
            var y = new Waypoint("Y", 0.0, 1.0);
            var z = new Waypoint("Z", 0.0, 3.0);

            var route = new Route();
            route.AddLastWaypoint(x);

            Assert.Throws<ArgumentException>(() =>
            route.ConnectRoute(GetRoute1()));
        }
    }
}
