using Unity.Collections;
using Unity.Netcode;

namespace Characters.Player
{
    public struct NetworkString : INetworkSerializable
    {
        private FixedString32Bytes _info;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter => 
            serializer.SerializeValue(ref _info);

        public override string ToString() => 
            _info.ToString();

        public static implicit operator string(NetworkString s) => s.ToString();

        public static implicit operator NetworkString(string s) =>
            new NetworkString() { _info = new FixedString32Bytes(s) };
    }
}