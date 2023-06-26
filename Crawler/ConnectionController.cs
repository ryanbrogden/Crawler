using Sandbox.Common.ObjectBuilders;
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
        public class ConnectionController
        {
            private MyGridProgram _gridProgram;

            // The minimum number of connections required before we can disconnect
            private readonly int MIN_CONNECTIONS = 2;

            private IMyShipMergeBlock upperMergeBlock;
            private IMyShipMergeBlock lowerMergeBlock;

            private IMyShipConnector upperConnector;
            private IMyShipConnector lowerConnector;

            public enum STATE { CONNECTED, UPPER_CONNECTED, LOWER_CONNECTED, UNKNOWN };

            public STATE State { 
                get
                {
                    if(upperMergeBlock.IsConnected && upperConnector.IsConnected)
                    {
                        if (lowerMergeBlock.IsConnected && lowerConnector.IsConnected) return STATE.CONNECTED;

                        return STATE.UPPER_CONNECTED;
                    } else if (lowerMergeBlock.IsConnected && lowerConnector.IsConnected)
                    {
                        return STATE.LOWER_CONNECTED;
                    }

                    return STATE.UNKNOWN;
                }
            }

            private void SetMergeBlocks(IMyBlockGroup group)
            {
                List<IMyShipMergeBlock> mergeBlocks = new List<IMyShipMergeBlock>();
                group.GetBlocksOfType(mergeBlocks);

                mergeBlocks.ForEach(block =>
                {
                    if (block.CustomName.ToLower().Contains("upper"))
                    {
                        upperMergeBlock = block;
                    }
                    else if (block.CustomName.ToLower().Contains("lower"))
                    {
                        lowerMergeBlock = block;
                    }
                });
            }

            private void SetConnectors(IMyBlockGroup group)
            {
                List<IMyShipConnector> connectors = new List<IMyShipConnector>();
                group.GetBlocksOfType(connectors);

                connectors.ForEach(block =>
                {
                    if (block.CustomName.ToLower().Contains("upper"))
                    {
                        upperConnector = block;
                    }
                    else if (block.CustomName.ToLower().Contains("lower"))
                    {
                        lowerConnector = block;
                    }
                });
            }

            public ConnectionController(IMyBlockGroup group, MyGridProgram gridProgram)
            {
                _gridProgram = gridProgram;
                SetMergeBlocks(group);
                SetConnectors(group);
            }

            private int ConnectionCount()
            {
                List<IMyShipMergeBlock> blocks = new List<IMyShipMergeBlock>
                {
                    upperMergeBlock,
                    lowerMergeBlock
                };

                return blocks.FindAll(block => block.IsConnected).Count;
            }

            private bool CanDisconnect() 
            {
                return ConnectionCount() >= MIN_CONNECTIONS;
            }

            private void Disconnect(IMyShipMergeBlock mergeBlock, IMyShipConnector connector)
            {
                if (CanDisconnect())
                {
                    mergeBlock.Enabled = false;
                    connector.Disconnect();
                    connector.Enabled = false;
                }
            }

            private void Enable(IMyShipMergeBlock mergeBlock, IMyShipConnector connector)
            {
                mergeBlock.Enabled = true;
                connector.Enabled = true;
                connector.Connect();
            }

            public void EnableUpper()
            {
                Enable(upperMergeBlock, upperConnector);
            }

            public void DisableUpper()
            {
                if (CanDisconnect()) 
                {
                    Disconnect(upperMergeBlock, upperConnector);
                }
                
            }

            public void EnableLower()
            {
                Enable(lowerMergeBlock, lowerConnector);
            }

            public void DisableLower()
            {
                if (CanDisconnect())
                {
                    Disconnect(lowerMergeBlock, lowerConnector);
                }
            }

            public void Reconnect()
            {
                EnableUpper();
                EnableLower();
            }

            public void run()
            {
                if (upperMergeBlock.IsConnected && !upperConnector.IsConnected)
                { 
                    upperConnector.Connect();
                }

                if (!upperMergeBlock.IsConnected && upperConnector.IsConnected)
                {
                    DisableUpper();
                    EnableUpper();
                }

                if (lowerMergeBlock.IsConnected && !lowerConnector.IsConnected) 
                {
                    lowerConnector.Connect();
                }

                if (!lowerMergeBlock.IsConnected && lowerConnector.IsConnected)
                {
                    DisableLower();
                    EnableLower();
                }
            }
        }
    }
}
