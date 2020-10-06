using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;


namespace BGDispatch
{
    public static class Looper
    {
        public static void DoLoop()
        {
            Int32 time = (EntryPoint.WaitTime * 1000);
            Random rndTime = new Random();
            
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    Game.LogTrivial("[BGDispatch]: Sleeping for " + EntryPoint.WaitTime + " seconds");
                    GameFiber.Sleep(time);

                    Game.LogTrivial("[BGDispatch]: Checking...");
                    EntryPoint.GreenLight = true;

                    // TODO: Do we play the file or not?

                    // Am I in a pursuit? 
                    if (Functions.IsPedInPursuit(Game.LocalPlayer.Character))
                    {
                        EntryPoint.GreenLight = false;
                        Game.LogTrivial("[BGDispatch]: Player is in a pursuit...");
                    }

                    // Has GrammarPolice fired in the last 20 seconds? 
                    // Yes = false

                    // Is Signal-100 active?
                    // Yes = false

                    // Is OnlyInVehicle set?
                    if (EntryPoint.OnlyInVehicle == true && !Game.LocalPlayer.Character.IsInAnyPoliceVehicle)
                    {
                        EntryPoint.GreenLight = false;
                        Game.LogTrivial("[BGDispatch]: Player not in police vehicle...");
                    }

                    // If GreenLight = true, play something
                    if (EntryPoint.GreenLight == true)
                    {
                        Player.PlayAudio();
                    } else
                    {
                        Game.LogTrivial("[BGDispatch]: Audio skipped, criteria not met");
                    }     

                    // Now choose random number between MinWait and MaxWait
                    EntryPoint.WaitTime = rndTime.Next(EntryPoint.MinWaitInt, EntryPoint.MaxWaitInt);
                    time = (EntryPoint.WaitTime * 1000);
                }
            });
        }


        



    }
}




