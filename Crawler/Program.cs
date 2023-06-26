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
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.
        private MyIni _ini = new MyIni();

        private string groupName;
        private IMyBlockGroup group;

        private PistonController pistonController;
        private ConnectionController connectionController;
        private StateController stateController;
        private DrillController drillController;
        private WelderController welderController;
        private DisplayController displayController;
 
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            MyIniParseResult parseResult;
            if (!_ini.TryParse(Me.CustomData, out parseResult))
            {
                throw new Exception(parseResult.ToString());
            }
            else
            {
                groupName = _ini.Get("Drill", "name").ToString();
                group = GridTerminalSystem.GetBlockGroupWithName(groupName);

                double vSpeed = _ini.Get("Drill", "v_speed").ToDouble();

                if (group != null)
                {
                    Echo($"Found group: {group.Name}");

                    if (vSpeed > 0)
                    {
                        pistonController = new PistonController(this, group, (float)vSpeed);
                    }
                    else 
                    {
                        pistonController = new PistonController(this, group);
                    }
                    
                    connectionController = new ConnectionController(group, this);
                    stateController = new StateController(this, pistonController, connectionController);
                    drillController = new DrillController(this, group);
                    welderController = new WelderController(this, group);
                    displayController = new DisplayController(this, group);

                }
                else
                {
                    Echo($"No drill found with name {groupName}");
                }
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            stateController.run();
            connectionController.run();
            drillController.run(pistonController.UpperHorizontalState,
                pistonController.LowerHorizontalState,
                pistonController.VerticalState);
            welderController.run(pistonController.VerticalState);

            displayController.write($"UH_Piston: {pistonController.UpperHorizontalState} \n" +
                $"LH_Piston: {pistonController.LowerHorizontalState} \n" +
                $"V_Piston: {pistonController.VerticalState} \n" +
                $"Connection Status: {connectionController.State} \n");

            Echo($"UH_Piston: {pistonController.UpperHorizontalState}");
            Echo($"LH_Piston: {pistonController.LowerHorizontalState}");
            Echo($"V_Piston: {pistonController.VerticalState}");
            Echo($"Connection Status: {connectionController.State}");
        }
    }
}
