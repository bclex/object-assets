//using OA.Ultima.Configuration.Properties;
//using System.Collections.Generic;
//using UnityEngine;

//namespace OA.Ultima.Core
//{
//    /// <summary>
//    /// Contains a list of all valid resolutions, and the code to change the size of the rendered window.
//    /// </summary>
//    class Resolutions
//    {
//        public static readonly List<ResolutionProperty> FullScreenResolutionsList;
//        public static readonly List<ResolutionProperty> PlayWindowResolutionsList;
//        public const int MAX_BUFFER_SIZE = 2056;

//        public static void SetWindowSize(GameWindow window)
//        {
//            RectInt game;
//            RectInt screen;
//            if (window != null)
//            {
//                game = window.ClientBounds;
//                screen = Screen.GetWorkingArea(new RectInt(game.x, game.y, game.width, game.height));
//            }
//            else
//                screen = Screen.GetWorkingArea(new System.Drawing.Point(0, 0));
//            foreach (var mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
//            {
//                if (mode.Format != SurfaceFormat.Color)
//                    continue;
//                var res = new ResolutionProperty(mode.Width, mode.Height);
//                if (res.Width <= MAX_BUFFER_SIZE && res.Height <= MAX_BUFFER_SIZE)
//                    if (!FullScreenResolutionsList.Contains(res))
//                        FullScreenResolutionsList.Add(res);
//            }
//            foreach (var res in FullScreenResolutionsList)
//                if (!PlayWindowResolutionsList.Contains(res) && res.Width <= screen.width && res.Height <= screen.height)
//                    PlayWindowResolutionsList.Add(res);
//        }

//        static Resolutions()
//        {
//            FullScreenResolutionsList = new List<ResolutionProperty>();
//            PlayWindowResolutionsList = new List<ResolutionProperty>();
//            SetWindowSize(null);
//        }

//        public static bool IsValidFullScreenResolution(ResolutionProperty resolution)
//        {
//            foreach (var res in FullScreenResolutionsList)
//                if (resolution.Width == res.Width && resolution.Height == res.Height)
//                    return true;
//            return false;
//        }

//        public static bool IsValidPlayWindowResolution(ResolutionProperty resolution)
//        {
//            foreach (var res in PlayWindowResolutionsList)
//                if (resolution.Width == res.Width && resolution.Height == res.Height)
//                    return true;
//            return false;
//        }
//    }
//}
