#if DEBUG && !UNITY_WP_8_1 && !UNITY_WSA
using System;
using System.Collections.Generic;
using System.Text;
using FlyingWormConsole3.LiteNetLib.Utils;

namespace FlyingWormConsole3.LiteNetLib
{
    public enum ConnectionState
    {
        InProgress,
        Connected,
        Disconnected
    }

    public sealed class NetPeer
    {
        private const int RttResetDelay = 1000;
        private const int MtuCheckDelay = 1000;
        private const int MaxMtuCheckAttempts = 4;
        private readonly object _flushLock = new object();
        private readonly Dictionary<ushort, IncomingFragments> _holdedFragments;

        //Merging
        private readonly NetPacket _mergeData;
        private readonly object _mtuMutex = new object();
        private readonly NetPacketPool _packetPool;

        //Channels
        private readonly ReliableChannel _reliableOrderedChannel;
        private readonly ReliableChannel _reliableUnorderedChannel;

        //Common            
        private readonly SequencedChannel _sequencedChannel;
        private readonly SimpleChannel _simpleChannel;
        private int _avgRtt;

        //Connection
        private int _connectAttempts;
        private int _connectTimer;
        //Flow control
        private bool _finishMtu;
        private int _flowTimer;
        private ushort _fragmentId;
        private int _goodRttCount;
        private int _mergeCount;
        private int _mergePos;

        //MTU
        private int _mtuCheckAttempts;
        private int _mtuCheckTimer;
        private int _mtuIdx;

        //Ping and RTT

        private int _pingSendTimer;
        private ushort _pingSequence;

        private DateTime _pingTimeStart;
        private ushort _remotePingSequence;
        private int _rtt;
        private int _rttCount;
        private int _rttResetTimer;
        private int _sendedPacketsCount;

        private readonly int _windowSize = NetConstants.DefaultWindowSize;

        /// <summary>
        ///     Application defined object containing data about the connection
        /// </summary>
        public object Tag;

        internal NetPeer(NetManager peerListener, NetEndPoint remoteEndPoint, long connectId)
        {
            _packetPool = peerListener.PacketPool;
            NetManager = peerListener;
            EndPoint = remoteEndPoint;

            _avgRtt = 0;
            _rtt = 0;
            _pingSendTimer = 0;

            _reliableOrderedChannel = new ReliableChannel(this, true, _windowSize);
            _reliableUnorderedChannel = new ReliableChannel(this, false, _windowSize);
            _sequencedChannel = new SequencedChannel(this);
            _simpleChannel = new SimpleChannel(this);

            _holdedFragments = new Dictionary<ushort, IncomingFragments>();

            _mergeData = _packetPool.Get(PacketProperty.Merged, NetConstants.MaxPacketSize);

            //if ID != 0 then we already connected
            _connectAttempts = 0;
            if (connectId == 0)
            {
                ConnectId = DateTime.UtcNow.Ticks;
                SendConnectRequest();
            }
            else
            {
                ConnectId = connectId;
                ConnectionState = ConnectionState.Connected;
                SendConnectAccept();
            }

            NetUtils.DebugWrite(ConsoleColor.Cyan, "[CC] ConnectId: {0}", ConnectId);
        }

        public ConnectionState ConnectionState { get; private set; }

        public long ConnectId { get; private set; }

        public NetEndPoint EndPoint { get; }

        public int Ping { get; private set; }

        public int CurrentFlowMode { get; private set; }

        public int Mtu { get; private set; } = NetConstants.PossibleMtu[0];

        public int TimeSinceLastPacket { get; private set; }

        public NetManager NetManager { get; }

        public int PacketsCountInReliableQueue => _reliableUnorderedChannel.PacketsInQueue;

        public int PacketsCountInReliableOrderedQueue => _reliableOrderedChannel.PacketsInQueue;

        internal double ResendDelay { get; private set; } = 27.0;

        private void SendConnectRequest()
        {
            //Get connect key bytes
            byte[] keyData = Encoding.UTF8.GetBytes(NetManager.ConnectKey);

            //Make initial packet
            NetPacket connectPacket = _packetPool.Get(PacketProperty.ConnectRequest, 12 + keyData.Length);

            //Add data
            FastBitConverter.GetBytes(connectPacket.RawData, 1, NetConstants.ProtocolId);
            FastBitConverter.GetBytes(connectPacket.RawData, 5, ConnectId);
            Buffer.BlockCopy(keyData, 0, connectPacket.RawData, 13, keyData.Length);

            //Send raw
            NetManager.SendRawAndRecycle(connectPacket, EndPoint);
        }

        private void SendConnectAccept()
        {
            //Reset connection timer
            TimeSinceLastPacket = 0;

            //Make initial packet
            NetPacket connectPacket = _packetPool.Get(PacketProperty.ConnectAccept, 8);

            //Add data
            FastBitConverter.GetBytes(connectPacket.RawData, 1, ConnectId);

            //Send raw
            NetManager.SendRawAndRecycle(connectPacket, EndPoint);
        }

        internal bool ProcessConnectAccept(NetPacket packet)
        {
            if (ConnectionState != ConnectionState.InProgress)
                return false;

            //check connection id
            if (BitConverter.ToInt64(packet.RawData, 1) != ConnectId)
            {
                return false;
            }

            NetUtils.DebugWrite(ConsoleColor.Cyan, "[NC] Received connection accept");
            TimeSinceLastPacket = 0;
            ConnectionState = ConnectionState.Connected;
            return true;
        }

        private static PacketProperty SendOptionsToProperty(SendOptions options)
        {
            switch (options)
            {
                case SendOptions.ReliableUnordered:
                    return PacketProperty.Reliable;
                case SendOptions.Sequenced:
                    return PacketProperty.Sequenced;
                case SendOptions.ReliableOrdered:
                    return PacketProperty.ReliableOrdered;
                default:
                    return PacketProperty.Unreliable;
            }
        }

        public int GetMaxSinglePacketSize(SendOptions options)
        {
            return Mtu - NetPacket.GetHeaderSize(SendOptionsToProperty(options));
        }

        public void Send(byte[] data, SendOptions options)
        {
            Send(data, 0, data.Length, options);
        }

        public void Send(NetDataWriter dataWriter, SendOptions options)
        {
            Send(dataWriter.Data, 0, dataWriter.Length, options);
        }

        public void Send(byte[] data, int start, int length, SendOptions options)
        {
            //Prepare
            PacketProperty property = SendOptionsToProperty(options);
            int headerSize = NetPacket.GetHeaderSize(property);

            //Check fragmentation
            if (length + headerSize > Mtu)
            {
                if (options == SendOptions.Sequenced || options == SendOptions.Unreliable)
                {
                    throw new Exception("Unreliable packet size > allowed (" + (Mtu - headerSize) + ")");
                }

                int packetFullSize = Mtu - headerSize;
                int packetDataSize = packetFullSize - NetConstants.FragmentHeaderSize;

                int fullPacketsCount = length / packetDataSize;
                int lastPacketSize = length % packetDataSize;
                int totalPackets = fullPacketsCount + (lastPacketSize == 0 ? 0 : 1);

                NetUtils.DebugWrite("FragmentSend:\n" +
                                    " MTU: {0}\n" +
                                    " headerSize: {1}\n" +
                                    " packetFullSize: {2}\n" +
                                    " packetDataSize: {3}\n" +
                                    " fullPacketsCount: {4}\n" +
                                    " lastPacketSize: {5}\n" +
                                    " totalPackets: {6}",
                    Mtu, headerSize, packetFullSize, packetDataSize, fullPacketsCount, lastPacketSize, totalPackets);

                if (totalPackets > ushort.MaxValue)
                {
                    throw new Exception("Too many fragments: " + totalPackets + " > " + ushort.MaxValue);
                }

                int dataOffset = headerSize + NetConstants.FragmentHeaderSize;
                for (ushort i = 0; i < fullPacketsCount; i++)
                {
                    NetPacket p = _packetPool.Get(property, packetFullSize);
                    p.FragmentId = _fragmentId;
                    p.FragmentPart = i;
                    p.FragmentsTotal = (ushort)totalPackets;
                    p.IsFragmented = true;
                    Buffer.BlockCopy(data, i * packetDataSize, p.RawData, dataOffset, packetDataSize);
                    SendPacket(p);
                }

                if (lastPacketSize > 0)
                {
                    NetPacket p = _packetPool.Get(property, lastPacketSize + NetConstants.FragmentHeaderSize);
                    p.FragmentId = _fragmentId;
                    p.FragmentPart = (ushort)fullPacketsCount; //last
                    p.FragmentsTotal = (ushort)totalPackets;
                    p.IsFragmented = true;
                    Buffer.BlockCopy(data, fullPacketsCount * packetDataSize, p.RawData, dataOffset, lastPacketSize);
                    SendPacket(p);
                }

                _fragmentId++;
                return;
            }

            //Else just send
            NetPacket packet = _packetPool.GetWithData(property, data, start, length);
            SendPacket(packet);
        }

        private void CreateAndSend(PacketProperty property, ushort sequence)
        {
            NetPacket packet = _packetPool.Get(property, 0);
            packet.Sequence = sequence;
            SendPacket(packet);
        }

        //from user thread, our thread, or recv?
        private void SendPacket(NetPacket packet)
        {
            NetUtils.DebugWrite("[RS]Packet: " + packet.Property);
            switch (packet.Property)
            {
                case PacketProperty.Reliable:
                    _reliableUnorderedChannel.AddToQueue(packet);
                    break;
                case PacketProperty.Sequenced:
                    _sequencedChannel.AddToQueue(packet);
                    break;
                case PacketProperty.ReliableOrdered:
                    _reliableOrderedChannel.AddToQueue(packet);
                    break;
                case PacketProperty.Unreliable:
                    _simpleChannel.AddToQueue(packet);
                    break;
                case PacketProperty.MtuCheck:
                    //Must check result for MTU fix
                    if (!NetManager.SendRawAndRecycle(packet, EndPoint))
                    {
                        _finishMtu = true;
                    }
                    break;
                case PacketProperty.AckReliable:
                case PacketProperty.AckReliableOrdered:
                case PacketProperty.Ping:
                case PacketProperty.Pong:
                case PacketProperty.Disconnect:
                case PacketProperty.MtuOk:
                    SendRawData(packet);
                    _packetPool.Recycle(packet);
                    break;
                default:
                    throw new Exception("Unknown packet property: " + packet.Property);
            }
        }

        private void UpdateRoundTripTime(int roundTripTime)
        {
            //Calc average round trip time
            _rtt += roundTripTime;
            _rttCount++;
            _avgRtt = _rtt / _rttCount;

            //flowmode 0 = fastest
            //flowmode max = lowest

            if (_avgRtt < NetManager.GetStartRtt(CurrentFlowMode - 1))
            {
                if (CurrentFlowMode <= 0)
                {
                    //Already maxed
                    return;
                }

                _goodRttCount++;
                if (_goodRttCount > NetConstants.FlowIncreaseThreshold)
                {
                    _goodRttCount = 0;
                    CurrentFlowMode--;

                    NetUtils.DebugWrite("[PA]Increased flow speed, RTT: {0}, PPS: {1}", _avgRtt, NetManager.GetPacketsPerSecond(CurrentFlowMode));
                }
            }
            else if (_avgRtt > NetManager.GetStartRtt(CurrentFlowMode))
            {
                _goodRttCount = 0;
                if (CurrentFlowMode < NetManager.GetMaxFlowMode())
                {
                    CurrentFlowMode++;
                    NetUtils.DebugWrite("[PA]Decreased flow speed, RTT: {0}, PPS: {1}", _avgRtt, NetManager.GetPacketsPerSecond(CurrentFlowMode));
                }
            }

            //recalc resend delay
            double avgRtt = _avgRtt;
            if (avgRtt <= 0.0)
                avgRtt = 0.1;
            ResendDelay = 25 + avgRtt * 2.1; // 25 ms + double rtt
        }

        internal void AddIncomingPacket(NetPacket p)
        {
            if (p.IsFragmented)
            {
                NetUtils.DebugWrite("Fragment. Id: {0}, Part: {1}, Total: {2}", p.FragmentId, p.FragmentPart, p.FragmentsTotal);
                //Get needed array from dictionary
                ushort packetFragId = p.FragmentId;
                IncomingFragments incomingFragments;
                if (!_holdedFragments.TryGetValue(packetFragId, out incomingFragments))
                {
                    incomingFragments = new IncomingFragments
                    {
                        Fragments = new NetPacket[p.FragmentsTotal]
                    };
                    _holdedFragments.Add(packetFragId, incomingFragments);
                }

                //Cache
                NetPacket[] fragments = incomingFragments.Fragments;

                //Error check
                if (p.FragmentPart >= fragments.Length || fragments[p.FragmentPart] != null)
                {
                    _packetPool.Recycle(p);
                    NetUtils.DebugWriteError("Invalid fragment packet");
                    return;
                }
                //Fill array
                fragments[p.FragmentPart] = p;

                //Increase received fragments count
                incomingFragments.ReceivedCount++;

                //Increase total size
                int dataOffset = p.GetHeaderSize() + NetConstants.FragmentHeaderSize;
                incomingFragments.TotalSize += p.Size - dataOffset;

                //Check for finish
                if (incomingFragments.ReceivedCount != fragments.Length)
                {
                    return;
                }

                NetUtils.DebugWrite("Received all fragments!");
                NetPacket resultingPacket = _packetPool.Get(p.Property, incomingFragments.TotalSize);

                int resultingPacketOffset = resultingPacket.GetHeaderSize();
                int firstFragmentSize = fragments[0].Size - dataOffset;
                for (int i = 0; i < incomingFragments.ReceivedCount; i++)
                {
                    //Create resulting big packet
                    int fragmentSize = fragments[i].Size - dataOffset;
                    Buffer.BlockCopy(
                        fragments[i].RawData,
                        dataOffset,
                        resultingPacket.RawData,
                        resultingPacketOffset + firstFragmentSize * i,
                        fragmentSize);

                    //Free memory
                    _packetPool.Recycle(fragments[i]);
                    fragments[i] = null;
                }

                //Send to process
                NetManager.ReceiveFromPeer(resultingPacket, EndPoint);

                //Clear memory
                _packetPool.Recycle(resultingPacket);
                _holdedFragments.Remove(packetFragId);
            }
            else //Just simple packet
            {
                NetManager.ReceiveFromPeer(p, EndPoint);
                _packetPool.Recycle(p);
            }
        }

        private void ProcessMtuPacket(NetPacket packet)
        {
            if (packet.Size == 1 ||
                packet.RawData[1] >= NetConstants.PossibleMtu.Length)
                return;

            //MTU auto increase
            if (packet.Property == PacketProperty.MtuCheck)
            {
                if (packet.Size != NetConstants.PossibleMtu[packet.RawData[1]])
                {
                    return;
                }
                _mtuCheckAttempts = 0;
                NetUtils.DebugWrite("MTU check. Resend: " + packet.RawData[1]);
                NetPacket mtuOkPacket = _packetPool.Get(PacketProperty.MtuOk, 1);
                mtuOkPacket.RawData[1] = packet.RawData[1];
                SendPacket(mtuOkPacket);
            }
            else if (packet.RawData[1] > _mtuIdx) //MtuOk
            {
                lock (_mtuMutex)
                {
                    _mtuIdx = packet.RawData[1];
                    Mtu = NetConstants.PossibleMtu[_mtuIdx];
                }
                //if maxed - finish.
                if (_mtuIdx == NetConstants.PossibleMtu.Length - 1)
                {
                    _finishMtu = true;
                }
                NetUtils.DebugWrite("MTU ok. Increase to: " + Mtu);
            }
        }

        //Process incoming packet
        internal void ProcessPacket(NetPacket packet)
        {
            TimeSinceLastPacket = 0;

            NetUtils.DebugWrite("[RR]PacketProperty: {0}", packet.Property);
            switch (packet.Property)
            {
                case PacketProperty.ConnectRequest:
                    //response with connect
                    long newId = BitConverter.ToInt64(packet.RawData, 1);
                    if (newId > ConnectId)
                    {
                        ConnectId = newId;
                    }

                    NetUtils.DebugWrite("ConnectRequest LastId: {0}, NewId: {1}, EP: {2}", ConnectId, newId, EndPoint);
                    SendConnectAccept();
                    _packetPool.Recycle(packet);
                    break;

                case PacketProperty.Merged:
                    int pos = NetConstants.HeaderSize;
                    while (pos < packet.Size)
                    {
                        ushort size = BitConverter.ToUInt16(packet.RawData, pos);
                        pos += 2;
                        NetPacket mergedPacket = _packetPool.GetAndRead(packet.RawData, pos, size);
                        if (mergedPacket == null)
                        {
                            _packetPool.Recycle(packet);
                            break;
                        }
                        pos += size;
                        ProcessPacket(mergedPacket);
                    }
                    break;
                //If we get ping, send pong
                case PacketProperty.Ping:
                    if (NetUtils.RelativeSequenceNumber(packet.Sequence, _remotePingSequence) < 0)
                    {
                        _packetPool.Recycle(packet);
                        break;
                    }
                    NetUtils.DebugWrite("[PP]Ping receive, send pong");
                    _remotePingSequence = packet.Sequence;
                    _packetPool.Recycle(packet);

                    //send
                    CreateAndSend(PacketProperty.Pong, _remotePingSequence);
                    break;

                //If we get pong, calculate ping time and rtt
                case PacketProperty.Pong:
                    if (NetUtils.RelativeSequenceNumber(packet.Sequence, _pingSequence) < 0)
                    {
                        _packetPool.Recycle(packet);
                        break;
                    }
                    _pingSequence = packet.Sequence;
                    int rtt = (int)(DateTime.UtcNow - _pingTimeStart).TotalMilliseconds;
                    UpdateRoundTripTime(rtt);
                    NetUtils.DebugWrite("[PP]Ping: {0}", rtt);
                    _packetPool.Recycle(packet);
                    break;

                //Process ack
                case PacketProperty.AckReliable:
                    _reliableUnorderedChannel.ProcessAck(packet);
                    _packetPool.Recycle(packet);
                    break;

                case PacketProperty.AckReliableOrdered:
                    _reliableOrderedChannel.ProcessAck(packet);
                    _packetPool.Recycle(packet);
                    break;

                //Process in order packets
                case PacketProperty.Sequenced:
                    _sequencedChannel.ProcessPacket(packet);
                    break;

                case PacketProperty.Reliable:
                    _reliableUnorderedChannel.ProcessPacket(packet);
                    break;

                case PacketProperty.ReliableOrdered:
                    _reliableOrderedChannel.ProcessPacket(packet);
                    break;

                //Simple packet without acks
                case PacketProperty.Unreliable:
                    AddIncomingPacket(packet);
                    return;

                case PacketProperty.MtuCheck:
                case PacketProperty.MtuOk:
                    ProcessMtuPacket(packet);
                    break;

                default:
                    NetUtils.DebugWriteError("Error! Unexpected packet type: " + packet.Property);
                    break;
            }
        }

        private static bool CanMerge(PacketProperty property)
        {
            switch (property)
            {
                case PacketProperty.ConnectAccept:
                case PacketProperty.ConnectRequest:
                case PacketProperty.MtuOk:
                case PacketProperty.Pong:
                case PacketProperty.Disconnect:
                    return false;
                default:
                    return true;
            }
        }

        internal void SendRawData(NetPacket packet)
        {
            //2 - merge byte + minimal packet size + datalen(ushort)
            if (NetManager.MergeEnabled &&
                CanMerge(packet.Property) &&
                _mergePos + packet.Size + NetConstants.HeaderSize * 2 + 2 < Mtu)
            {
                FastBitConverter.GetBytes(_mergeData.RawData, _mergePos + NetConstants.HeaderSize, (ushort)packet.Size);
                Buffer.BlockCopy(packet.RawData, 0, _mergeData.RawData, _mergePos + NetConstants.HeaderSize + 2, packet.Size);
                _mergePos += packet.Size + 2;
                _mergeCount++;

                //DebugWriteForce("Merged: " + _mergePos + "/" + (_mtu - 2) + ", count: " + _mergeCount);
                return;
            }

            NetUtils.DebugWrite(ConsoleColor.DarkYellow, "[P]SendingPacket: " + packet.Property);
            NetManager.SendRaw(packet.RawData, 0, packet.Size, EndPoint);
        }

        private void SendQueuedPackets(int currentMaxSend)
        {
            int currentSended = 0;
            while (currentSended < currentMaxSend)
            {
                //Get one of packets
                if (_reliableOrderedChannel.SendNextPacket() ||
                    _reliableUnorderedChannel.SendNextPacket() ||
                    _sequencedChannel.SendNextPacket() ||
                    _simpleChannel.SendNextPacket())
                {
                    currentSended++;
                }
                else
                {
                    //no outgoing packets
                    break;
                }
            }

            //Increase counter
            _sendedPacketsCount += currentSended;

            //If merging enabled
            if (_mergePos > 0)
            {
                if (_mergeCount > 1)
                {
                    NetUtils.DebugWrite("Send merged: " + _mergePos + ", count: " + _mergeCount);
                    NetManager.SendRaw(_mergeData.RawData, 0, NetConstants.HeaderSize + _mergePos, EndPoint);
                }
                else
                {
                    //Send without length information and merging
                    NetManager.SendRaw(_mergeData.RawData, NetConstants.HeaderSize + 2, _mergePos - 2, EndPoint);
                }
                _mergePos = 0;
                _mergeCount = 0;
            }
        }

        /// <summary>
        ///     Flush all queued packets
        /// </summary>
        public void Flush()
        {
            lock (_flushLock)
            {
                SendQueuedPackets(int.MaxValue);
            }
        }

        internal void Update(int deltaTime)
        {
            if (ConnectionState == ConnectionState.Disconnected)
            {
                return;
            }

            TimeSinceLastPacket += deltaTime;
            if (ConnectionState == ConnectionState.InProgress)
            {
                _connectTimer += deltaTime;
                if (_connectTimer > NetManager.ReconnectDelay)
                {
                    _connectTimer = 0;
                    _connectAttempts++;
                    if (_connectAttempts > NetManager.MaxConnectAttempts)
                    {
                        ConnectionState = ConnectionState.Disconnected;
                        return;
                    }

                    //else send connect again
                    SendConnectRequest();
                }
                return;
            }

            //Get current flow mode
            int maxSendPacketsCount = NetManager.GetPacketsPerSecond(CurrentFlowMode);
            int currentMaxSend;

            if (maxSendPacketsCount > 0)
            {
                int availableSendPacketsCount = maxSendPacketsCount - _sendedPacketsCount;
                currentMaxSend = Math.Min(availableSendPacketsCount, maxSendPacketsCount * deltaTime / NetConstants.FlowUpdateTime);
            }
            else
            {
                currentMaxSend = int.MaxValue;
            }

            //DebugWrite("[UPDATE]Delta: {0}ms, MaxSend: {1}", deltaTime, currentMaxSend);

            //Pending acks
            _reliableOrderedChannel.SendAcks();
            _reliableUnorderedChannel.SendAcks();

            //ResetFlowTimer
            _flowTimer += deltaTime;
            if (_flowTimer >= NetConstants.FlowUpdateTime)
            {
                NetUtils.DebugWrite("[UPDATE]Reset flow timer, _sendedPackets - {0}", _sendedPacketsCount);
                _sendedPacketsCount = 0;
                _flowTimer = 0;
            }

            //Send ping
            _pingSendTimer += deltaTime;
            if (_pingSendTimer >= NetManager.PingInterval)
            {
                NetUtils.DebugWrite("[PP] Send ping...");

                //reset timer
                _pingSendTimer = 0;

                //send ping
                CreateAndSend(PacketProperty.Ping, _pingSequence);

                //reset timer
                _pingTimeStart = DateTime.UtcNow;
            }

            //RTT - round trip time
            _rttResetTimer += deltaTime;
            if (_rttResetTimer >= RttResetDelay)
            {
                _rttResetTimer = 0;
                //Rtt update
                _rtt = _avgRtt;
                Ping = _avgRtt;
                NetManager.ConnectionLatencyUpdated(this, Ping);
                _rttCount = 1;
            }

            //MTU - Maximum transmission unit
            if (!_finishMtu)
            {
                _mtuCheckTimer += deltaTime;
                if (_mtuCheckTimer >= MtuCheckDelay)
                {
                    _mtuCheckTimer = 0;
                    _mtuCheckAttempts++;
                    if (_mtuCheckAttempts >= MaxMtuCheckAttempts)
                    {
                        _finishMtu = true;
                    }
                    else
                    {
                        lock (_mtuMutex)
                        {
                            //Send increased packet
                            if (_mtuIdx < NetConstants.PossibleMtu.Length - 1)
                            {
                                int newMtu = NetConstants.PossibleMtu[_mtuIdx + 1] - NetConstants.HeaderSize;
                                NetPacket p = _packetPool.Get(PacketProperty.MtuCheck, newMtu);
                                p.RawData[1] = (byte)(_mtuIdx + 1);
                                SendPacket(p);
                            }
                        }
                    }
                }
            }
            //MTU - end

            //Pending send
            lock (_flushLock)
            {
                SendQueuedPackets(currentMaxSend);
            }
        }

        //For channels
        internal void Recycle(NetPacket packet)
        {
            _packetPool.Recycle(packet);
        }

        internal NetPacket GetPacketFromPool(PacketProperty property, int bytesCount)
        {
            return _packetPool.Get(property, bytesCount);
        }

        //Fragment
        private class IncomingFragments
        {
            public NetPacket[] Fragments;
            public int ReceivedCount;
            public int TotalSize;
        }
    }
}
#endif