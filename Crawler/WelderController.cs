using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class WelderController
        {
            private MyGridProgram _gridProgram;
            private List<IMyShipWelder> welders = new List<IMyShipWelder>();
            public WelderController(MyGridProgram gridProgram, IMyBlockGroup group)
            {
                _gridProgram = gridProgram;
                group.GetBlocksOfType(welders);
            }

            private void EnableWelders()
            {
                welders.ForEach(welder =>
                {
                    welder.Enabled = true;
                });
            }

            private void DisableWelders() 
            {
                welders.ForEach(welder =>
                {
                    welder.Enabled = false;
                });
            }

            public void run(PistonStatus verticalState)
            {
                if (verticalState == PistonStatus.Extending)
                {
                    EnableWelders();
                }
                else
                {
                    DisableWelders();
                }
            }
        }
    }
}
