﻿using System;
using System.Drawing;
using QSP.Common;
using QSP.LibraryExtension;
using QSP.TOPerfCalculation;
using QSP.TOPerfCalculation.Boeing.PerfData;
using QSP.UI.MsgBox;
using QSP.Utilities.Units;

namespace QSP.UI.UserControls.TakeoffLanding.TOPerf.Controllers
{
    public class BoeingController : FormController
    {
        public BoeingController(PerfTable acPerf, TOPerfElements elements)
            : base(acPerf, elements)
        { }

        public override void WeightUnitChanged(object sender, EventArgs e)
        {
            double wt;

            if (double.TryParse(elements.weight.Text, out wt))
            {
                if (elements.wtUnit.SelectedIndex == 0)
                {
                    // LB -> KG 
                    wt *= AviationTools.Constants.LbKgRatio;
                }
                else
                {
                    // KG -> LB
                    wt *= AviationTools.Constants.KgLbRatio;
                }

                elements.weight.Text = ((int)Math.Round(wt)).ToString();
            }
        }

        public override void Initialize()
        {
            SetDefaultSurfCond();
            SetDefaultFlaps();
            SetDerate();
            SetPackOptions();
            SetAntiIceOptions();
        }

        private void SetPackOptions()
        {
            var items = elements.Packs.Items;
            items.Clear();
            items.AddRange(new string[] { "ON", "OFF" });
            elements.Packs.SelectedIndex = 0;
        }

        private void SetAntiIceOptions()
        {
            var items = elements.AntiIce.Items;
            items.Clear();
            items.AddRange(new string[]
            {
                "OFF",
                "ONLY ENG A/I",
                "ENG AND WING A/I"
            });
            elements.AntiIce.SelectedIndex = 0;
        }

        public override void FlapsChanged(object sender, EventArgs e)
        {
            SetDerate();
        }

        private void SetDerate()
        {
            var thrustComboBox = elements.thrustRating;
            string text = thrustComboBox.Text;
            var table = ((BoeingPerfTable)acPerf.Item)
                         .Tables[elements.flaps.SelectedIndex];

            var items = thrustComboBox.Items;
            items.Clear();

            if (table.AltnRatingAvail)
            {
                foreach (var i in table.ThrustRatings)
                {
                    items.Add(i);
                }

                thrustComboBox.SelectedIndex = 0;
                thrustComboBox.Text = text;
                thrustComboBox.Visible = true;
                elements.ThrustRatingLbl.Visible = true;
            }
            else
            {
                thrustComboBox.Visible = false;
                elements.ThrustRatingLbl.Visible = false;
            }
        }

        private void SetDefaultSurfCond()
        {
            var items = elements.surfCond.Items;

            items.Clear();
            items.AddRange(new string[] { "Dry", "Wet" });
            elements.surfCond.SelectedIndex = 0;
        }

        private void SetDefaultFlaps()
        {
            var items = elements.flaps.Items;
            items.Clear();

            foreach (var i in ((BoeingPerfTable)acPerf.Item).Flaps)
            {
                items.Add(i);
            }

            elements.flaps.SelectedIndex = 0;
        }

        public override void Compute(object sender, EventArgs e)
        {
            try
            {
                var para = new BoeingParameterValidator(elements).Validate();

                var report = new TOPerfCalculation.Boeing.TOReportGenerator(
                    (BoeingPerfTable)acPerf.Item, para).TakeOffReport();

                var text = report.ToString(
                    elements.tempUnit.SelectedIndex == 0 ?
                    TemperatureUnit.Celsius :
                    TemperatureUnit.Fahrenheit,
                    elements.lengthUnit.SelectedIndex == 0 ?
                    LengthUnit.Meter :
                    LengthUnit.Feet);

                // To center the text in the richTxtBox
                elements.result.Text = text.ShiftToRight(15);

                OnCalculationComplete(EventArgs.Empty);
                elements.result.ForeColor = Color.Black;
            }
            catch (InvalidUserInputException ex)
            {
                MsgBoxHelper.ShowWarning(ex.Message);
            }
            catch (RunwayTooShortException)
            {
                MsgBoxHelper.ShowWarning("Runway length is insufficient for takeoff.");
            }
            catch (PoorClimbPerformanceException)
            {
                MsgBoxHelper.ShowWarning("Aircraft too heavy to meet " +
                    "climb performance requirement.");
            }
        }
    }
}