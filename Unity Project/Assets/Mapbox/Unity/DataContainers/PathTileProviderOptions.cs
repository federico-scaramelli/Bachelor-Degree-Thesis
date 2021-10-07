namespace Mapbox.Unity.Map
{
    using System;
    using UnityEngine;
    using Mapbox.Directions;

    [Serializable]
    public class PathTileProviderOptions : ExtentOptions
    {
        public DirectionsResponse response;
        [Range(0,4)]
        public int visibleBuffer=1;

        public override void SetOptions(ExtentOptions extentOptions)
        {
            PathTileProviderOptions options = extentOptions as PathTileProviderOptions;
            if (options != null)
            {
                response = options.response;
                visibleBuffer = options.visibleBuffer;
            }
            else
            {
                Debug.LogError("ExtentOptions type mismatch : Using " + extentOptions.GetType() + " to set extent of type " + this.GetType());
            }
        }

        public void SetOptions(DirectionsResponse Response, int VisibleBuffer)
        {
            response = Response;
            visibleBuffer = VisibleBuffer;
        }
    }
}
