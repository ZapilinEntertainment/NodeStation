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

        public IPathNode Build(int key, ConstructingNodePoint nodePoint)
        {
            var worldPos = nodePoint.transform.position;
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
                        return new DeadEndNode(worldPos, key, entranceNodeKey, nodePoint.Function);
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

                        if (!firstConnectionFound)
                            Debug.LogWarning("only one connection found for straight node!");

                        // operating found connections
                        int entranceNodeKey = 0;
                        int exitNodeKey = 0;
                        if (connectionA.StartNodeKey == key)
                        {                           
                            //  connectionB - node - connectionA
                            exitNodeKey = connectionA.EndNodeKey;
                            if (connectionB.EndNodeKey == key)
                                entranceNodeKey = connectionB.StartNodeKey;
                            else
                                entranceNodeKey = connectionB.EndNodeKey;
                        }
                        else
                        {
                            // connectionA - node - connectionB
                            entranceNodeKey = connectionA.StartNodeKey;
                            if (connectionB.EndNodeKey == key)
                                exitNodeKey = connectionB.StartNodeKey;
                            else
                                exitNodeKey = connectionB.EndNodeKey;
                        }

                        //Debug.Log($"A: {connectionA.StartNodeKey} -> {connectionA.EndNodeKey}, B: {connectionB.StartNodeKey} -> {connectionB.EndNodeKey}");
                        //Debug.Log($"{entranceNodeKey} -> {exitNodeKey}");

                        return new StraightPathNode(worldPos, key, entranceNodeKey, exitNodeKey, nodePoint.Function);
                    }

                    case NodeType.Dividing:
                    {
                        //Debug.Log("Build dividing node:");

                        //                 / Exit A
                        // Entrance - Node - Exit B

                        var entranceNodeKey = 0;
                        var exitNodeKeyA = 0;
                        var exitNodeKeyB = 0;
                        var searchMask = 0;

                        foreach (var path in _pathsData)
                        {
                            if (path.EndNodeKey == key)
                            {
                                entranceNodeKey = path.StartNodeKey;
                                searchMask += 1;
                            }    
                            else
                            {
                                if (path.StartNodeKey == key)
                                {
                                    if ((searchMask & 2) == 0)
                                    {
                                        exitNodeKeyA = path.EndNodeKey;
                                        searchMask += 2;
                                    }
                                    else
                                    {
                                        exitNodeKeyB = path.EndNodeKey;
                                        searchMask += 4;
                                    }
                                }
                            }

                            if (searchMask == 7)
                                break;
                        }
                        
                        return new DividingPathNode(worldPos, key, entranceNodeKey, exitNodeKeyA, exitNodeKeyB, isReversed : false, nodePoint.Function);
                    }

                    case NodeType.DividingReversed:
                    {
                        //Debug.Log("Build dividing reversed node:");

                        // Entrance A \
                        // Entrance B - Node - Exit

                        var entranceNodeKeyA = 0;
                        var entranceNodeKeyB = 0;
                        var exitNodeKey = 0;
                        var searchMask = 0;

                        foreach (var path in _pathsData)
                        {
                            if (path.EndNodeKey == key)
                            {
                                if ((searchMask & 1) == 0) 
                                { 
                                    entranceNodeKeyA = path.StartNodeKey;
                                    searchMask += 1;
                                    //Debug.Log("entrance A: " + entranceNodeKeyA);
                                }
                                else
                                {
                                    entranceNodeKeyB = path.StartNodeKey;
                                    searchMask += 2;
                                    //Debug.Log("entrance B: " + entranceNodeKeyB);
                                }
                            }
                            else 
                            { 
                                if (path.StartNodeKey == key)
                                {
                                    exitNodeKey = path.EndNodeKey;
                                    searchMask += 4;
                                    //Debug.Log("exit: " + exitNodeKey);
                                }
                            }

                            if (searchMask == 7)
                                break;
                        }

                        return new DividingPathNode(worldPos, key, exitNodeKey, entranceNodeKeyA, entranceNodeKeyB, isReversed : true, nodePoint.Function);
                    }

                    default:
                    {
                        throw new System.Exception("Undefined node cannot be added to map");
                    }
            }
        }

    }
}
