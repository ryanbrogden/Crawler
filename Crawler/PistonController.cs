using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Noise.Patterns;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class PistonController
        {
            private List<IMyPistonBase> horizontalPistons = new List<IMyPistonBase>();
            private List<IMyPistonBase> verticalPistons = new List<IMyPistonBase>();

            private readonly int MIN_HORIZONTAL_PISTON_EXTENSION = 0;
            private readonly int MAX_HORIZONTAL_PISTON_EXTENSION = 1;
            private readonly float MIN_VERTICAL_PISTON_EXTENSION = 1.1f;
            private readonly float MAX_VERTICAL_PISTON_EXTENSION = 8.6f;
            private float VERTICAL_PISTON_VELOCITY = 0.2f;
            private float HORIZONTAL_PISTON_VELOCITY = 1f;

            public enum STATE { COLLAPSED, EXTENDED, UPPER_EXTENDED, LOWER_EXTENDED, MOVING }

            private MyGridProgram _gridProgram;

            public PistonStatus VerticalState
            {
                get
                {
                    return verticalPistons[0].Status;                    
                }
            }

            public PistonStatus UpperHorizontalState
            {
                get
                {
                    return GetUpperHorizontalPiston().Status;
                }
            }

            public PistonStatus LowerHorizontalState
            {
                get
                {
                    return GetLowerHorizontalPiston().Status;
                }
            }

            public PistonController(MyGridProgram gridProgram, IMyBlockGroup group)
            {
                _gridProgram = gridProgram;
                List<IMyPistonBase> pistons = new List<IMyPistonBase>();
                group.GetBlocksOfType(pistons);

                pistons.ForEach(piston =>
                {
                    if (piston.CustomName.ToLower().Contains("horizontal"))
                    {
                        piston.MaxLimit = MAX_HORIZONTAL_PISTON_EXTENSION;
                        piston.MinLimit = MIN_HORIZONTAL_PISTON_EXTENSION;
                        horizontalPistons.Add(piston);
                    }
                    else if (piston.CustomName.ToLower().Contains("vertical"))
                    {
                        piston.MaxLimit = MAX_VERTICAL_PISTON_EXTENSION;
                        piston.MinLimit = MIN_VERTICAL_PISTON_EXTENSION;
                        verticalPistons.Add(piston);
                    }
                });

                _gridProgram.Echo($"Controlling {pistons.Count} pistons");
                _gridProgram.Echo($"{horizontalPistons.Count} horizontal pistons");
                _gridProgram.Echo($"{verticalPistons.Count} vertical pistons");
            }

            public PistonController(MyGridProgram gridProgram, IMyBlockGroup group, float vSpeed)
            {
                VERTICAL_PISTON_VELOCITY = vSpeed;

                _gridProgram = gridProgram;
                List<IMyPistonBase> pistons = new List<IMyPistonBase>();
                group.GetBlocksOfType(pistons);

                pistons.ForEach(piston =>
                {
                    if (piston.CustomName.ToLower().Contains("horizontal"))
                    {
                        piston.MaxLimit = MAX_HORIZONTAL_PISTON_EXTENSION;
                        piston.MinLimit = MIN_HORIZONTAL_PISTON_EXTENSION;
                        horizontalPistons.Add(piston);
                    }
                    else if (piston.CustomName.ToLower().Contains("vertical"))
                    {
                        piston.MaxLimit = MAX_VERTICAL_PISTON_EXTENSION;
                        piston.MinLimit = MIN_VERTICAL_PISTON_EXTENSION;
                        verticalPistons.Add(piston);
                    }
                });

                _gridProgram.Echo($"Controlling {pistons.Count} pistons");
                _gridProgram.Echo($"{horizontalPistons.Count} horizontal pistons");
                _gridProgram.Echo($"{verticalPistons.Count} vertical pistons");
            }

            private void ExtendPiston(IMyPistonBase piston)
            {
                if (piston.CustomName.ToLower().Contains("horizontal"))
                {
                    piston.Velocity = HORIZONTAL_PISTON_VELOCITY;
                } else
                {
                    piston.Velocity = VERTICAL_PISTON_VELOCITY;
                }
            }

            private void RetractPiston(IMyPistonBase piston)
            {
               piston.Velocity = -HORIZONTAL_PISTON_VELOCITY;
            }

            public void ExtendVertical()
            {
                _gridProgram.Echo($"Extending {verticalPistons.Count} pistons");
                verticalPistons.ForEach((piston) => 
                {
                    ExtendPiston(piston);
                });
            }

            public void RetractVertical()
            {
                verticalPistons.ForEach((piston) => 
                {
                    RetractPiston(piston);
                });
            }

            public IMyPistonBase GetUpperHorizontalPiston()
            {
                return horizontalPistons.Find(piston => piston.CustomName.ToLower().Contains("upper"));
            }

            public void ExtendUpperHorizontal() 
            {
                ExtendPiston(GetUpperHorizontalPiston());
            }

            public void RetractUpperHorizontal() 
            {
                RetractPiston(GetUpperHorizontalPiston());
            }

            public IMyPistonBase GetLowerHorizontalPiston()
            {
                return horizontalPistons.Find(piston => piston.CustomName.ToLower().Contains("lower"));
            }

            public void ExtendLowerHorizontal()
            {
                ExtendPiston(GetLowerHorizontalPiston());
            }

            public void RetractLowerHorizontal()
            {
                RetractPiston(GetLowerHorizontalPiston());
            }
        }
    }
}
