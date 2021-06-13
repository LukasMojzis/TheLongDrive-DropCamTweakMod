using System;
using System.Collections.Generic;
using System.Reflection;
using TLDLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DropCamTweak
{
    public class DropCamTweak : Mod
    {
        public override string ID => ((AssemblyProductAttribute) Assembly.GetAssembly(GetType())
            .GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
        
        public override string Name => ((AssemblyDescriptionAttribute) Assembly.GetAssembly(GetType())
            .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;

        public override string Version => ((AssemblyInformationalVersionAttribute) Assembly.GetAssembly(GetType())
            .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)[0]).InformationalVersion;

        public override string Author =>
            ((AssemblyCompanyAttribute) Assembly.GetAssembly(GetType())
                .GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0]).Company;

        public static GameObject HiddenCameraObject;

        public static GameObject PlayerObject;
        public static Camera MainCamera;
        public static Color GoldColor => new Color(1.000f, 0.843f, 0.000f);
        public static GameObject PlayerHeadObject;
        public static GameObject PlayerHeadTarget;

        public override void OnLoad()
        {
            PlayerObject = GameObject.Find("Player");
            MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            PlayerHeadObject = GameObject.FindGameObjectWithTag("Player");
            PlayerHeadTarget = GameObject.Find("HeadTarget");
        }

        public override void Update()
        {
            if (!HiddenCameraObject && GameObject.Find("HiddenCamera"))
            {
                HiddenCameraObject = GameObject.Find("HiddenCamera");
                HiddenCameraObject.AddComponent<HiddenCameraScript>();
            }
        }
    }
}