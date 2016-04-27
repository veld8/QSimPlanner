﻿using System.Net;

namespace QSP.Metar
{
    public static class MetarDownloader
    {
        private const string metarUrl =
            "http://weather.noaa.gov/pub/data/observations/metar/stations/";

        private const string tafUrl =
            "http://weather.noaa.gov/pub/data/forecasts/taf/stations/";

        private static string trimIcao(string icao)
        {
            return icao.Trim().ToUpper();
        }

        public static string GetMetar(string icao)
        {
            using (var client = new WebClient())
            {
                return client.DownloadString(metarUrl + trimIcao(icao) + ".TXT");
            }
        }

        public static string GetTaf(string icao)
        {
            using (var client = new WebClient())
            {
                return client.DownloadString(tafUrl + trimIcao(icao) + ".TXT");
            }
        }

        public static bool TryGetMetar(string icao, out string metar)
        {
            try
            {
                metar = GetMetar(icao);
                return true;
            }
            catch
            {
                metar = null;
                return false;
            }
        }

        public static bool TryGetTaf(string icao, out string taf)
        {
            try
            {
                taf = GetTaf(icao);
                return true;
            }
            catch
            {
                taf = null;
                return false;
            }
        }

        public static string TryGetMetar(string icao)
        {
            try
            {
                return GetMetar(icao);
            }
            catch
            {
                return "Downloading Metar failed.";
            }
        }

        public static string TryGetTaf(string icao)
        {
            try
            {
                return GetTaf(icao);
            }
            catch
            {
                return "Downloading TAF failed.";
            }
        }

        public static string TryGetMetarTaf(string icao)
        {
            return TryGetMetar(icao) + "\n\n" + TryGetTaf(icao);
        }
    }
}