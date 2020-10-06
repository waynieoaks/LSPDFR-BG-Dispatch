using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;

namespace BGDispatch
{
    public class EntryPoint : Plugin
    {
        // Settings variables
        public static string INIpath = "Plugins\\LSPDFR\\BGDispatch.ini";
        public static string Audiopath = "Plugins\\LSPDFR\\BGDispatch";

        public static Int16 MinWaitInt { get; set; }
        public static Int16 MaxWaitInt { get; set; }
        public static Int32 WaitTime { get; set; }
        public static Byte Volume { get; set; }
        public static Boolean OnlyInVehicle { get; set; }
        public static Boolean GreenLight { get; set; }

        public IEnumerable <string> WavFiles { get; set; }

        //Initialization of the plugin.
        public override void Initialize()
        {
            Game.LogTrivial("[BGDispatch]: Starting EntryPoint.");

                // Disable the native police scanner on loadup
                NativeFunction.Natives.SetAudioFlag("PoliceScannerDisabled", true);

            Game.LogTrivial("[BGDispatch]: Loading settings...");
                LoadValuesFromIniFile();

            // Sanity check - Does directory exist? 
            if (!Directory.Exists(Audiopath))
            {
                Game.LogTrivial("--------------------------------------");
                Game.LogTrivial("ERROR during initialisation");
                Game.LogTrivial("Decription: Audio directory is missing");
                Game.LogTrivial("--------------------------------------");
                Game.DisplayNotification("~r~~h~[BGDispatch]:~h~~s~ ERROR during initialisation. Please send logs.");
                return;
            }

            // Sanity check - Are there 2 or more wav files? 
            WavFiles = Directory.EnumerateFiles(Audiopath, "*.wav", SearchOption.TopDirectoryOnly);
            Int32 WavCount = WavFiles.Count();

            if (WavCount < 2)
            {
                Game.LogTrivial("--------------------------------------");
                Game.LogTrivial("ERROR during initialisation");
                Game.LogTrivial("Decription: Not enough audio files. Only " + WavCount + " files found");
                Game.LogTrivial("--------------------------------------");
                Game.DisplayNotification("~r~~h~[BGDispatch]:~h~~s~ ERROR during initialisation. Please send logs.");
                return;
            }

           //  Game.LogTrivial("[BGDispatch]: " + WavCount.ToString() + " audio files found");
            foreach (var wav in WavFiles)
            {
               // string WavName = wav.Substring(Audiopath.Length + 1);
               // Game.LogTrivial(WavName);

            }

            // TODO: Build playlist

            // TODO: Randomise playlist

            Game.LogTrivial("~b~Background Dispatch (BGDispatch)~b~ " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been loaded.");
            Game.DisplayNotification("~b~Background Dispatch~b~ " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been loaded.");

            EntryPoint.WaitTime = 120; // Wait 2 minutes before first audio file plays

            Looper.DoLoop();

            //    //GrammarPolice Keybinding Fiber
            //    GameFiber.StartNew(delegate
            //    {
            //        while (true)
            //        {
            //            GameFiber.Yield();

            //            //If key pressed, then here
            //        }
            //    });
        }
       
        private static void LoadValuesFromIniFile()
        {
            // string path = "Plugins\\RecovFR.ini";
            InitializationFile ini = new InitializationFile(INIpath);
            ini.Create();

            try
            {
                if (ini.DoesKeyExist("Settings", "MinWait")) { MinWaitInt = ini.ReadByte("Settings", "MinWait", 20); }
                else
                {
                    ini.Write("Settings", "MinWait", "20");
                    MinWaitInt = 20;
                }
                // Sanity check: MinWait time in range?
                if (MinWaitInt < 10 || MinWaitInt > 180)
                {
                    MinWaitInt = 20;
                    Game.LogTrivial("[BGDispatch]: WARNING MinWait intival out of range, using default");
                }

                if (ini.DoesKeyExist("Settings", "MaxWait")) { MaxWaitInt = ini.ReadByte("Settings", "MaxWait", 75); }
                else
                {
                    ini.Write("Settings", "MaxWait", "75");
                    MaxWaitInt = 75;
                }

                //Sanity check: MaxWait time in range? 
                if (MaxWaitInt < 20 || MaxWaitInt > 300)
                {
                    MinWaitInt = 20;
                    MaxWaitInt = 75;
                    Game.LogTrivial("[BGDispatch]: WARNING MaxWait intival out of range, using default");
                }

                //Sanity check: MaxWait more than MinWait? 
                if (MinWaitInt > MaxWaitInt)
                {
                    MaxWaitInt = 75;
                    Game.LogTrivial("[BGDispatch]: WARNING Wait intivals out of range (Min>Max), using default");
                }

                if (ini.DoesKeyExist("Settings", "OnlyInVehicle")) { OnlyInVehicle = ini.ReadBoolean("Settings", "OnlyInVehicle", true); }
                else
                {
                    ini.Write("Settings", "OnlyInVehicle", "true");
                    OnlyInVehicle = true;
                }

                if (ini.DoesKeyExist("Settings", "Volume")) { Volume = ini.ReadByte("Settings", "Volume", 20); }
                else
                {
                    ini.Write("Settings", "Volume", "20");
                    Volume = 20;
                }

                Game.LogTrivial("[BGDispatch]: Settings initialisation complete.");
            }
            catch (Exception e)
            {
                ErrorLogger(e, "Initialisation", "Unable to read INI file.");
            }
        }

        public static void ErrorLogger(Exception Err, String ErrMod, String ErrDesc)
        {
            Game.LogTrivial("--------------------------------------");
            Game.LogTrivial("ERROR during " + ErrMod);
            Game.LogTrivial("Decription: " + ErrDesc);
            Game.LogTrivial(Err.ToString());
            Game.LogTrivial("--------------------------------------");
            Game.DisplayNotification("~r~~h~[BGDispatch]:~h~~s~ ERROR during " + ErrMod + ". Please send logs.");
        }

        public override void Finally()
        {
        }
    }
}
