using System;
using System.Collections.Generic;
using System.Reflection;

namespace RetroGameHandler.UserControllImages
{
    public static class ControllImages
    {
        public static CImages CImages { get; set; }

        public static void Init()
        {
            CImages = new CImages();
        }
    }

    public class CImages
    {
        public CImages()
        {
            var type = typeof(IUserControllImage);
            foreach (Type t in Assembly.GetCallingAssembly().GetTypes())
            {
                if (t.GetInterface("IUserControllImage") != null)
                {
                    IUserControllImage executor = Activator.CreateInstance(t) as IUserControllImage;
                    Images.Add(executor.GetType().Name, executor);
                }
            }
        }

        public Dictionary<string, IUserControllImage> Images { get; private set; } = new Dictionary<string, IUserControllImage>();
    }
}