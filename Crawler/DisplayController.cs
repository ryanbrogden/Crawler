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
        public class DisplayController
        {
            private MyGridProgram _gridProgram;
            private List<IMyTextPanel> textPanels = new List<IMyTextPanel>();

            public DisplayController(MyGridProgram gridProgram, IMyBlockGroup group)
            {
                group.GetBlocksOfType(textPanels);
            }

            public void write(string text)
            {
                if (textPanels.Count > 0)
                {
                    textPanels.ForEach(panel =>
                    {
                        panel.ContentType = ContentType.TEXT_AND_IMAGE;
                        panel.Alignment = TextAlignment.LEFT;
                        panel.WriteText(text, false);                    
                    });
                }
            }
        }
    }
}
