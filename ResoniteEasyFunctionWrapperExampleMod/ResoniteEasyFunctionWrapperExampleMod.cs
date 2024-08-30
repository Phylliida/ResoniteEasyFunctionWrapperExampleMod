using HarmonyLib; // HarmonyLib comes included with a ResoniteModLoader install
using ResoniteModLoader;
using ResoniteHotReloadLib;
using ResoniteEasyFunctionWrapper;
using FrooxEngine;
using System.Collections.Generic;
using System;

namespace ResoniteEasyFunctionWrapperExampleMod
{

    // Class name can be anything
    public class MyExportedMethods
    {
        // All methods should be static

        // Each method will be exported
        // One limitation: all methods must have unique names (no overloading)
        public static int ExampleMethod(int a, int b)
        {
            return a + b;
        }

        // out variables will be named outputs
        // ref variables will be inputs and outputs
        public static int ExampleMethod2(int a, out int b)
        {
            b = a + 1;
            return a + 2;
        }

        // variables that aren't valid resonite flux inputs will be
        // made into strings with uuids representing them
        // (there's some caching to prevent spamming if you return constant values)
        // this allows you to use any type
        public static void ComplicatedUslessMethod(string wow, int bees, float ok, Slot item,
            out Dictionary<String, String> out1, out float out2, out Grabbable out3)
        {
            out1 = new Dictionary<string, string>();
            if (wow != null)
            {
                out1[wow] = bees.ToString();
            }
            out2 = ok;
            out3 = null;
            if (item != null)
            {
                out3 = item.GetComponent<Grabbable>();
            }
        }

        public static string readFromDict(Dictionary<string, string> dict, string key)
        {
            return dict[key];
        }
    }

    public class ResoniteEasyFunctionWrapperExampleMod : ResoniteMod
    {
        public override string Name => "ResoniteEasyFunctionWrapperExampleMod";
        public override string Author => "TessaCoil";
        public override string Version => "1.0.0"; //Version of the mod, should match the AssemblyVersion
        public override string Link => "https://github.com/Phylliida/ResoniteEasyFunctionWrapperExampleMod"; // Optional link to a repo where this mod would be located

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> enabled = new ModConfigurationKey<bool>("enabled", "Should the mod be enabled", () => true); //Optional config settings

        private static ModConfiguration Config; //If you use config settings, this will be where you interface with them.
        private static string harmony_id = "example.Me.MyMod";

        private static Harmony harmony;
        public override void OnEngineInit()
        {
            HotReloader.RegisterForHotReload(this);

            Config = GetConfiguration(); //Get the current ModConfiguration for this mod
            Config.Save(true); //If you'd like to save the default config values to file
        
            SetupMod();
        }

        public static void SetupMod()
        {
            harmony = new Harmony(harmony_id);
            harmony.PatchAll();

            ResoniteEasyFunctionWrapper.ResoniteEasyFunctionWrapper.WrapClass(
                typeof(MyExportedMethods),
                modNamespace: harmony_id);
        }

        static void BeforeHotReload()
        {
            harmony = new Harmony(harmony_id);
            // This runs in the current assembly (i.e. the assembly which invokes the Hot Reload)
            harmony.UnpatchAll();

            // Remove menus and class wrappings
            ResoniteEasyFunctionWrapper.ResoniteEasyFunctionWrapper.UnwrapClass(
                classType:typeof(MyExportedMethods),
                modNamespace: harmony_id);
            
            // This is where you unload your mod, free up memory, and remove Harmony patches etc.
        }

        static void OnHotReload(ResoniteMod modInstance)
        {
            // This runs in the new assembly (i.e. the one which was loaded fresh for the Hot Reload)
            
            // Get the config
            Config = modInstance.GetConfiguration();

            // Now you can setup your mod again
            SetupMod();
        }
    }
}
