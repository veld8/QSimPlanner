﻿using QSP.LibraryExtension;
using QSP.RouteFinding.AirwayStructure;
using QSP.RouteFinding.Containers;
using QSP.RouteFinding.Data.Interfaces;
using QSP.RouteFinding.Routes;
using QSP.RouteFinding.TerminalProcedures.Star;
using System;
using System.Collections.Generic;
using System.Linq;
using RouteString = System.Collections.Generic.IReadOnlyList<string>;

namespace QSP.RouteFinding.RouteAnalyzers.Extractors
{
    // Given a route as a RouteString, Extract() returns an object 
    // containing:
    //
    // * An DestRoute containing the departure runway and STAR
    //   (if STAR exists).
    //   There are 3 cases:
    //   1. The last element of input RouteString is not a STAR.
    //   2. The first waypoint of STAR is in wptList, which
    //      may or may not be connected to an airway.
    //   3. The first waypoint of STAR is NOT in wptList.

    //   For different cases, the returning route contains:
    //   Case 1. The last enroute waypoint, then direct to the dest runway.
    //   Case 2. The first waypoint of STAR, then go to the dest runway
    //           (via STAR).
    //   Case 3. The last enroute waypoint, then direct to the first waypoint 
    //           of STAR. Then go to the dest runway (via STAR).
    //
    // * A RemainingRoute. In all cases, the last entry in RemainingRoute
    //   is guranteed to be the same as the first waypoint in the DestRoute.
    //
    // The input route should not contain the destination ICAO, and must 
    // contain one element.

    public class StarExtractor
    {
        private WaypointList wptList;
        private StarCollection stars;
        private Waypoint rwyWpt;
        private string icao;
        private string rwy;

        private LinkedList<string> route;

        public StarExtractor(
            IEnumerable<string> route,
            string icao,
            string rwy,
            Waypoint rwyWpt,
            WaypointList wptList,
            StarCollection stars)
        {
            this.route = new LinkedList<string>(route);
            this.icao = icao;
            this.rwy = rwy;
            this.rwyWpt = rwyWpt;
            this.wptList = wptList;
            this.stars = stars;
        }

        public ExtractResult Extract()
        {
            var last = route.Last.Value;
            var star = TryGetStar(last, rwyWpt);

            if (star == null)
            {
                // Case 1
                var wpt = FindWpt(last);//TODO: What if not found?

                var neighbor = new Neighbor("DCT", wpt.Distance(rwyWpt));
                var node1 = new RouteNode(wpt, neighbor);
                var node2 = new RouteNode(rwyWpt, null);
                var destRoute = new Route(node1, node2);

                return new ExtractResult(route.ToList(), destRoute);
            }

            // Remove STAR from RouteString.
            route.RemoveLast();

            // Case 2,3 
            var candidates = wptList.FindAllById(route.Last.Value);
            var starFirstWpt = star.First();

            if (starFirstWpt.ID != route.Last.Value)
            {
                throw new ArgumentException($"{route.Last.Value} is not the"
                    + $" first waypoint of the STAR {last}.");
            }

            // TODO: Maybe add a distance upper limit?
            if (candidates.Count == 0)
            {
                // Case 3

                route.RemoveLast();
                // Now the last item of route is the last enroute waypoint.
                
                var lastEnrouteWpt = FindWpt(route.Last.Value);
                var firstStarWpt = star.First();
                double distance1 = lastEnrouteWpt.Distance(firstStarWpt);

                var neighbor1 = new Neighbor("DCT", distance1);
                var node1 = new RouteNode(lastEnrouteWpt, neighbor1);

                double distance2 = star.TotalDistance();
                var innerWpts = star.WithoutFirstAndLast();
                                
                var neighbor2 = new Neighbor(last, distance2, innerWpts);
                var node2 = new RouteNode(firstStarWpt, neighbor2);

                var node3 = new RouteNode(rwyWpt, null);
                var destRoute = new Route(node1, node2, node3);

                return new ExtractResult(route.ToList(), destRoute);
            }
            else
            {
                // Case 2
                var firstStarWpt = star.First();
                double distance = star.TotalDistance();
                var innerWpts = star.WithoutFirstAndLast();

                var neighbor = new Neighbor(last, distance, innerWpts);
                var node1 = new RouteNode(firstStarWpt, neighbor);

                var node2 = new RouteNode(rwyWpt, null);
                var destRoute = new Route(node1, node2);

                return new ExtractResult(route.ToList(), destRoute);
            }
        }

        public class ExtractResult
        {
            public RouteString RemainingRoute;
            public Route DestRoute;

            public ExtractResult(RouteString RemainingRoute, Route DestRoute)
            {
                this.RemainingRoute = RemainingRoute;
                this.DestRoute = DestRoute;
            }
        }
        
        private IReadOnlyList<Waypoint> TryGetStar(string starName,
            Waypoint rwyWpt)
        {
            try
            {
                return stars.StarWaypoints(starName, rwy, rwyWpt);
            }
            catch
            {
                // no Star in route                
                return null;
            }
        }

        private Waypoint FindWpt(string ident)
        {
            return wptList
                .FindAllById(ident)
                .Select(i => wptList[i])
                .GetClosest(rwyWpt);
        }
    }
}
