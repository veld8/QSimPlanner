using System;
using System.Linq;
using QSP.MathTools;
using QSP.AviationTools;

namespace QSP.WindAloft
{

    public class WxFileLoader
    {
        private WindTable[] windTables = new WindTable[Utilities.FullWindDataSet.Count()];

        public void ImportAllTables()
        {
            // For 100mb, u_table = wx1.csv, v_table = wx2.csv
            // For 200mb, u_table = wx3.csv, v_table = wx4.csv
            // ...


            for (int i = 0; i <= Utilities.FullWindDataSet.Length - 1; i++)
            {
                WindTable table = new WindTable();

                string u = Utilities.wxFileDirectory + "\\wx" + Convert.ToString(i * 2 + 1) + ".csv";
                string v = Utilities.wxFileDirectory + "\\wx" + Convert.ToString(i * 2 + 2) + ".csv";

                table.LoadFromFile(u, v);

                windTables[i] = table;

            }

        }

        public Tuple<double, double> GetWindUV(double lat, double lon, double FL)
        {
            double press = CoversionTools.AltToPressureMb(FL * 100);
            int index = getIndicesForInterpolation(press);

            double uWind = InterpolationOld.Interpolate(Utilities.FullWindDataSet[index], Utilities.FullWindDataSet[index + 1], press, windTables[index].GetUWind(lat, lon), windTables[index + 1].GetUWind(lat, lon));
            double vWind = InterpolationOld.Interpolate(Utilities.FullWindDataSet[index], Utilities.FullWindDataSet[index + 1], press, windTables[index].GetVWind(lat, lon), windTables[index + 1].GetVWind(lat, lon));

            return new Tuple<double, double>(uWind, vWind);
        }

        private int getIndicesForInterpolation(double press)
        {
            //let the return value be x, use indices x and x+1 for interpolation
            //works for extrapolation as well

            int len = Utilities.FullWindDataSet.Length;

            if (press < Utilities.FullWindDataSet[0])
            {
                return 0;
            }

            for (int i = 0; i < len - 1; i++)
            {
                if (press >= Utilities.FullWindDataSet[i] && press <= Utilities.FullWindDataSet[i + 1])
                {
                    return i;
                }
            }

            return len - 2;

        }

    }
}

