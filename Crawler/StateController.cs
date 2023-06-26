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
using static IngameScript.Program;

namespace IngameScript
{
    partial class Program
    {
        public class StateController
        {
            private MyGridProgram _gridProgram;
            private PistonController _pistonController;
            private ConnectionController _connectionController;

            public StateController(MyGridProgram gridProgram, PistonController pistonController, ConnectionController connectionController)
            {
                _gridProgram = gridProgram;
                _pistonController = pistonController;
                _connectionController = connectionController;
            }

            private bool IsPistonMoving() 
            {
                List<PistonStatus> pistonStatuses = new List<PistonStatus>()
                {
                    _pistonController.VerticalState,
                    _pistonController.GetUpperHorizontalPiston().Status,
                    _pistonController.GetLowerHorizontalPiston().Status
                };

                return pistonStatuses.Any(pistonStatus => 
                    pistonStatus == PistonStatus.Extending ||
                    pistonStatus == PistonStatus.Retracting
                );
            }

            private void SafteyCheckUpper()
            {
                // Saftey: If the upper is connected, it should be extended
                if (_connectionController.State == ConnectionController.STATE.UPPER_CONNECTED &&
                    _pistonController.GetUpperHorizontalPiston().Status == PistonStatus.Retracted)
                {
                    _pistonController.ExtendLowerHorizontal();
                }
            }

            private void SafteyCheckLower()
            {
                // Saftey: If the lower is connected, it should be extended
                if (_connectionController.State == ConnectionController.STATE.LOWER_CONNECTED &&
                    _pistonController.GetLowerHorizontalPiston().Status == PistonStatus.Retracted)
                {
                    _pistonController.ExtendLowerHorizontal();
                }
            }

            private void RunFullyConnected()
            {
                switch (_pistonController.VerticalState)
                {
                    case PistonStatus.Retracted:
                        _connectionController.DisableLower();
                        _pistonController.RetractLowerHorizontal();
                        break;
                    case PistonStatus.Extended:
                        _connectionController.DisableUpper();
                        _pistonController.RetractUpperHorizontal();
                        break;
                }
            }

            private void RunUpperConnected()
            {

                switch (_pistonController.VerticalState)
                {
                    case PistonStatus.Retracted:
                        _pistonController.ExtendVertical();
                        break;
                    case PistonStatus.Extended:
                        if (_pistonController.GetLowerHorizontalPiston().Status == PistonStatus.Retracted)
                        {
                            _pistonController.ExtendLowerHorizontal();
                        }
                        else if (_pistonController.GetLowerHorizontalPiston().Status == PistonStatus.Extended)
                        {
                            _connectionController.EnableLower();
                        }
                        break;
                }
            }

            private void RunLowerConnected()
            {
                switch (_pistonController.VerticalState)
                {
                    case PistonStatus.Retracted:
                        if (_pistonController.GetUpperHorizontalPiston().Status == PistonStatus.Retracted)
                        {
                            _pistonController.ExtendUpperHorizontal();
                        }
                        else if (_pistonController.GetUpperHorizontalPiston().Status == PistonStatus.Extended)
                        {
                            _connectionController.EnableUpper();
                        }
                        
                        break;
                    case PistonStatus.Extended:
                        _pistonController.RetractVertical();
                        break;

                }
            }

            public void run()
            {
                SafteyCheckUpper();
                SafteyCheckLower();

                if (_connectionController.State == ConnectionController.STATE.UNKNOWN) 
                {
                    _gridProgram.Echo("Connection state unknown, attempting reconnect");
                    _connectionController.Reconnect();
                }

                if (_connectionController.State == ConnectionController.STATE.CONNECTED)
                {
                    _gridProgram.Echo("Fully connected");
                    RunFullyConnected();
                    return;
                }

                if (IsPistonMoving())
                {
                    _gridProgram.Echo("Moving");
                    return;
                }

                switch (_connectionController.State)
                {
                    case ConnectionController.STATE.UPPER_CONNECTED:
                        _gridProgram.Echo("Upper connected");
                        RunUpperConnected();
                        break;
                    case ConnectionController.STATE.LOWER_CONNECTED:
                        _gridProgram.Echo("Lower connected");
                        RunLowerConnected();
                        break;
                }
            }
        }
    }
}
