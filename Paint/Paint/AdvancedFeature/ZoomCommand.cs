using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public static readonly float[] ZOOM_VALUE = { 0.25f, 0.5f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f};

        public static readonly float MIN_ZOOM_VALUE = ZOOM_VALUE[0];
        public static readonly float MAX_ZOOM_VALUE = ZOOM_VALUE.Last();
        public static readonly float DEFAULT_ZOOM_VALUE = 1f;

        ZoomType _zoomType;

        public float findNearestValue(float[] arr, float value)
        {
            float result = arr[0] ;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                if (value >= arr[i] && value <= arr[i + 1])
                {
                    if (Math.Abs(value - arr[i]) < Math.Abs(value - arr[i + 1])) 
                        result = arr[i];
                    else
                        result = arr[i + 1];
                }
            }
            return result;
        }

        public ZoomCommand(MainWindow app, ZoomType zoomType) : base(app)
        {
            _zoomType = zoomType;
        }

        public override bool Execute()
        {
            float zoomRatio = findNearestValue(ZOOM_VALUE, _app.zoomRatio);
            int index = Array.IndexOf(ZOOM_VALUE, zoomRatio);

            float newZoomRatio = zoomRatio;
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
