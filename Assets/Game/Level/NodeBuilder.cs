using UnityEngine;
using System.Collections.Generic;

namespace ZE.NodeStation
{
    // converts editor-data nodes (NodePoint) into in-game nodes(IPathNode)
    public class NodeBuilder
    {
        private readonly IReadOnlyCollection<ConstructingPathData> _pathsData;

        public NodeBuilder(IReadOnlyCollection<ConstructingPathData> paths)
        {
            _pathsData = paths;
        }

        public IPathNode Build(int key, NodePoint nodePoint)
        {
            // no type-checks included (data is already checked)
            switch (nodePoint.Type)
            {
                case NodeType.DeadEnd:
                    {
                        //only one connected node

                        int entranceNodeKey = 0;
                        foreach (var path in _pathsData)
                        {
                            if (path.StartNodeKey == key)
                            {
                                entranceNodeKey = path.EndNodeKey; 
                                break;
                            }

                            if (path.EndNodeKey == key)
                            {
                                entranceNodeKey = path.StartNodeKey;
                                break;
                            }
                        }
                        return new DeadEndNode(key, entranceNodeKey);
                    }

                    case NodeType.Straight:
                    {
                        // two neighbours - left and right
                        var firstConnectionFound = false;
                        ConstructingPathData connectionA = default;
                        ConstructingPathData connectionB = default;

                        foreach (var path in _pathsData)
                        {
                            if (path.StartNodeKey == key || path.EndNodeKey == key)
                            {
                                if (firstConnectionFound)
                                {
                                    connectionB = path;
                                    break;
                                }
                                else
                                {
                                    connectionA = path;
                                    firstConnectionFound = true;
                                }
                            }
                        }

                        // operating found connections
                        int entranceNodeKey = 0;
                        int exitNodeKey = 0;
                        if (connectionA.StartNodeKey == key)
                        {
                            exitNodeKey = connectionA.StartNodeKey;
                            if (connectionB.EndNodeKey == key)
                                entranceNodeKey = connectionB.EndNodeKey;
                            else
                                entranceNodeKey = connectionB.StartNodeKey;
                        }
                        else
                        {
                            entranceNodeKey = connectionA.EndNodeKey;
                            if (connectionB.StartNodeKey == key)
                                exitNodeKey = connectionB.StartNodeKey;
                            else
                                exitNodeKey = connectionB.EndNodeKey;
                        }

                        return new StraightPathNode(key, entranceNodeKey, exitNodeKey);
                    }

                    case NodeType.Dividing:
                    {
                        var entranceNodeKey = 0;
                        var exitNodeKeyA = 0;
                        var exitNodeKeyB = 0;
                        var foundMask = 0;

                        foreach (var path in _pathsData)
                        {
                            if ((foundMask & 1) == 0 && path.EndNodeKey == key)
                            {
                                entranceNodeKey = path.StartNodeKey;
                                foundMask += 1;
                            }

                            if (path.StartNodeKey == key)
                            {
                                if ((foundMask & 2) == 0)
                                {
                                    exitNodeKeyA = path.EndNodeKey;
                                    foundMask += 2;
                                }
                                else
                                {
                                    exitNodeKeyB = path.EndNodeKey;
                                    break;
                                }
                            }                            
                        }
                        
                        return new DividingPathNode(key, entranceNodeKey, exitNodeKeyA, exitNodeKeyB);
                    }

                    case NodeType.DividingReversed:
                    {
                        var entranceNodeKey = 0;
                        var exitNodeKeyA = 0;
                        var exitNodeKeyB = 0;
                        var foundMask = 0;

                        foreach (var path in _pathsData)
                        {
                            if ((foundMask & 1) == 0 && path.StartNodeKey == key)
                            {
                                entranceNodeKey = path.EndNodeKey;
                                foundMask += 1;
                            }

                            if (path.EndNodeKey == key)
                            {
                                if ((foundMask & 2) == 0)
                                {
                                    exitNodeKeyA = path.StartNodeKey;
                                    foundMask += 2;
                                }
                                else
                                {
                                    exitNodeKeyB = path.StartNodeKey;
                                    break;
                                }
                            }
                        }

                        return new DividingPathNode(key, entranceNodeKey, exitNodeKeyA, exitNodeKeyB);
                    }

                    default:
                    {
                        throw new System.Exception("Undefined node cannot be added to map");
                    }
            }
        }

    }
}
