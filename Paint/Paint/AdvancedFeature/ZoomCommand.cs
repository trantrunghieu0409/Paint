using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.AdvancedFeature
{
    public enum ZoomType
    {
        ZOOM_IN,
        ZOOM_OUT,
        DEFAULT
    }

    internal class ZoomCommand : Command
    {
        public static readonly int[] ZOOM_VALUE = { 25, 50, 100, 200, 300, 400, 500, 600, 700, 800 };

        public static readonly int MIN_ZOOM_VALUE = ZOOM_VALUE[0];
        public static readonly int MAX_ZOOM_VALUE = ZOOM_VALUE.Last();
        public static readonly int DEFAULT_ZOOM_VALUE = 100;

        ZoomType _zoomType;

        public ZoomCommand(MainWindow app, ZoomType zoomType) : base(app)
        {
            _zoomType = zoomType;
        }

        public override bool Execute()
        {
            int zoomRatio = _app.zoomRatio;
            int index = Array.IndexOf(ZOOM_VALUE, zoomRatio);

            int newZoomRatio = zoomRatio;
            switch (_zoomType)
            {
                case ZoomType.ZOOM_IN:
                    if (zoomRatio > MIN_ZOOM_VALUE) newZoomRatio = ZOOM_VALUE[--index];
                    break;
                case ZoomType.ZOOM_OUT: 
                    if (zoomRatio < MAX_ZOOM_VALUE) newZoomRatio = ZOOM_VALUE[++index]; 
                    break;
                default: 
                    newZoomRatio = DEFAULT_ZOOM_VALUE; 
                    break;
            }

            if (zoomRatio != newZoomRatio)
            {
                _app.zoomRatio = newZoomRatio;
            }

            return false;
        }
    }
}
