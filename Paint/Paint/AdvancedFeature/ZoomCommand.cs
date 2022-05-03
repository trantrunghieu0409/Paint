using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.AdvancedFeature
{
    internal class ZoomCommand : Command
    {
        public static readonly int[] ZOOM_VALUE = { 25, 50, 100, 200, 300, 400, 500, 600, 700, 800 };

        public static readonly int MIN_ZOOM_VALUE = ZOOM_VALUE[0];
        public static readonly int MAX_ZOOM_VALUE = ZOOM_VALUE.Last();


        public ZoomCommand(MainWindow app) : base(app)
        {
        }

        public override bool Execute()
        {
            return false;
        }
    }
}
