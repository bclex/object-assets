using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
namespace UnityEngine
{
    public static class ResourcesEx
    {
        public static Object LoadFromUri(string address) { return LoadFromUri(new Uri(address)); }
        public static Object LoadFromUri(Uri address)
        {
            using (var client = new WebClient())
            {
                using (var stream = client.OpenRead(address))
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }
            }
            return GameObject.Find("Cube00");
        }
    }

    public static class BrowserExtensions
    {
    }
}