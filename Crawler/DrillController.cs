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
        public class DrillController
        {
            private MyGridProgram _gridProgram;
            private List<IMyShipDrill> shipDrills = new List<IMyShipDrill>();

            public DrillController(MyGridProgram gridProgram, IMyBlockGroup group)
            {
                _gridProgram = gridProgram;
                group.GetBlocksOfType(shipDrills);
            }

            private void EnableDrills()
            {
                shipDrills.ForEach(drill =>
                {
                    drill.Enabled = true;
                });
            }

            private void DisableDrills()
            {
                shipDrills.ForEach(drill =>
                {
                    drill.Enabled = false;
                });
            }

            public void run(PistonStatus upperHorizontalState, PistonStatus lowerHorizontalState, PistonStatus verticalState)
            {
                List<PistonStatus> pistonStatuses = new List<PistonStatus>()
                {
                    upperHorizontalState,
                    lowerHorizontalState,
                    verticalState
                };

                bool isPistonMoving = pistonStatuses.Any(status => 
                    status == PistonStatus.Extending ||
                    status == PistonStatus.Retracting
                );

                if (isPistonMoving) 
                {
                    EnableDrills();
                }
                else
                {
                    DisableDrills();
                }
            }
        }
    }
}
