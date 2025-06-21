using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderBoardData : INetworkSerializable, IEquatable<LeaderBoardData>
{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;
    public int Scores;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Scores);
    }

    public bool Equals(LeaderBoardData other)
    {
        return ClientId == other.ClientId &&
            PlayerName.Equals(other.PlayerName) &&
            Scores == other.Scores;
    }
}
