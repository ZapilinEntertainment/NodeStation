using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;

#if UNITY_EDITOR
using TriInspector;
using UnityEditor;
#endif

namespace ZE.NodeStation
{
    public class PathsConstructor : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<int, NodePoint> _nodes;
        [SerializeField, OnValueChanged(nameof(RecalculateNodeTypes))] private ConstructingPathData[] _paths;
        private readonly Dictionary<int, int> _nodeEntrancesCount = new();
        private readonly Dictionary<int, int> _nodeExitsCount = new();
        private NodeBuilder _nodeBuilder;
        private RailPathBuilder _railPathBuilder;

        public PathsMap ConstructMap()
        {
            _nodeBuilder ??= new(_paths);

            var map = new PathsMap();
            if (_nodes != null && _nodes.Count != 0)
            {
                foreach (var nodeKvp in _nodes)
                {
                    var key = nodeKvp.Key;
                    var node = _nodeBuilder.Build(key, nodeKvp.Value);
                    map.AddNode(key, node);
                }
            }

            if (_paths != null && _paths.Length != 0)
            {
                _railPathBuilder ??= new(_nodes);
                foreach (var path in _paths)
                {
                    var key = new PathKey(path.StartNodeKey, path.EndNodeKey);
                    var rail = _railPathBuilder.Build(key, path);
                    map.AddPath(key, rail);
                }
            }

            return map;
        }

        [Button("Recalculate Node Types")]
        private void RecalculateNodeTypes()
        {
            _nodeEntrancesCount.Clear();
            _nodeExitsCount.Clear();

            if (_nodes == null || _nodes.Count == 0)
                return;

            if (_paths == null || _paths.Length == 0)
            {
                foreach (var node in _nodes.Values)
                {
                    node.Type = NodeType.Undefined;
                }
                return;
            }

            foreach (var path in _paths)
            {
                if (!_nodeEntrancesCount.TryGetValue(path.EndNodeKey, out var entrancesCount))
                    entrancesCount = 0;

                _nodeEntrancesCount[path.EndNodeKey] = entrancesCount + 1;

                if (!_nodeExitsCount.TryGetValue(path.StartNodeKey, out var exitsCount))
                    exitsCount = 0;

                _nodeExitsCount[path.StartNodeKey] = exitsCount + 1;
            }

            var errorsCount = 0;
            // show node types
            foreach (var nodeKvp in _nodes)
            {
                var nodeKey = nodeKvp.Key;
                var nodeType = DefineNodeType(nodeKey);
                nodeKvp.Value.Type = nodeType;
                if (nodeType == NodeType.Undefined)
                {
                    errorsCount++;
                    Debug.LogWarning($"node {nodeKey} cannot be defined");
                }
            }

            if (errorsCount == 0)
                Debug.Log("paths map is valid");
        }

        private NodeType DefineNodeType(int nodeKey)
        {
            int entrancesCount;
            int exitsCount;
            if (!_nodeEntrancesCount.TryGetValue(nodeKey, out entrancesCount))
            {
                if (_nodeExitsCount.TryGetValue(nodeKey, out exitsCount) && exitsCount == 1)
                    return NodeType.DeadEnd;
            }
            
            if (!_nodeExitsCount.TryGetValue(nodeKey, out exitsCount))
            {
                if (entrancesCount == 1)
                    return NodeType.DeadEnd;
            }

            if (entrancesCount == 1)
            {
                if (exitsCount == 1)
                    return NodeType.Straight;

                if (exitsCount == 2)
                    return NodeType.Dividing;
            }
            else
            {
                if (entrancesCount == 2 && exitsCount == 1)
                    return NodeType.DividingReversed;
            }

            return NodeType.Undefined;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_paths != null && _paths.Length != 0)
            {
                foreach (var path in _paths)
                {
                    if (!_nodes.TryGetValue(path.StartNodeKey, out var startNodePos)
                        || !_nodes.TryGetValue(path.EndNodeKey, out var endNodePos))
                        continue;

                    var start = startNodePos.transform.position;
                    var end = endNodePos.transform.position;
                    Gizmos.DrawLine(start, end);

                    var dir = Vector3.Normalize(end - start);
                    var center = Vector3.Lerp(start, end, 0.5f);
                    Gizmos.DrawLine(center, Quaternion.AngleAxis(135f, Vector3.up) * dir + center);
                    Gizmos.DrawLine(center, Quaternion.AngleAxis(135f, Vector3.down) * dir + center);
                }
            }

            if (_nodes != null && _nodes.Count != 0)
            {
                foreach (var nodeKvp in _nodes)
                {
                    var node = nodeKvp.Value;
                    if (node == null)
                        continue;
                    var pos = nodeKvp.Value.transform.position;

                    Color color;
                    switch (node.Function)
                    {
                        case NodeFunction.Spawn: color = Color.blue; break;
                        case NodeFunction.Exit: color = Color.red; break;
                        default: 
                            {
                                switch (node.Type) 
                                {
                                    case NodeType.Dividing: color = Color.yellow; break;
                                    case NodeType.DividingReversed: color = Color.orange; break;
                                    default: color = Color.white; break;
                                }
                                break;
                            }; 
                    }

                    Gizmos.color = color;
                    Gizmos.DrawSphere(pos, DebugConstants.RAIL_NODES_RADIUS);
                    Handles.Label(pos + new Vector3(0f,DebugConstants.RAIL_NODES_RADIUS * 2f,0f), nodeKvp.Key.ToString());
                }
            }

           
        }
#endif
    }
}
