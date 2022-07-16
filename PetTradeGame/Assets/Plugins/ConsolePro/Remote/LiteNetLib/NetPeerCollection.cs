#if DEBUG && !UNITY_WP_8_1 && !UNITY_WSA
using System;
using System.Collections.Generic;

namespace FlyingWormConsole3.LiteNetLib
{
    internal sealed class NetPeerCollection
    {
        private readonly NetPeer[] _peersArray;
        private readonly Dictionary<NetEndPoint, NetPeer> _peersDict;

        public NetPeerCollection(int maxPeers)
        {
            _peersArray = new NetPeer[maxPeers];
            _peersDict = new Dictionary<NetEndPoint, NetPeer>();
        }

        public int Count { get; private set; }

        public NetPeer this[int index] => _peersArray[index];

        public bool TryGetValue(NetEndPoint endPoint, out NetPeer peer)
        {
            return _peersDict.TryGetValue(endPoint, out peer);
        }

        public void Clear()
        {
            Array.Clear(_peersArray, 0, Count);
            _peersDict.Clear();
            Count = 0;
        }

        public void Add(NetEndPoint endPoint, NetPeer peer)
        {
            _peersArray[Count] = peer;
            _peersDict.Add(endPoint, peer);
            Count++;
        }

        public bool ContainsAddress(NetEndPoint endPoint)
        {
            return _peersDict.ContainsKey(endPoint);
        }

        public NetPeer[] ToArray()
        {
            NetPeer[] result = new NetPeer[Count];
            Array.Copy(_peersArray, 0, result, 0, Count);
            return result;
        }

        public void RemoveAt(int idx)
        {
            _peersDict.Remove(_peersArray[idx].EndPoint);
            _peersArray[idx] = _peersArray[Count - 1];
            _peersArray[Count - 1] = null;
            Count--;
        }

        public void Remove(NetEndPoint endPoint)
        {
            for (int i = 0; i < Count; i++)
            {
                if (_peersArray[i].EndPoint.Equals(endPoint))
                {
                    RemoveAt(i);
                    break;
                }
            }
        }
    }
}
#endif