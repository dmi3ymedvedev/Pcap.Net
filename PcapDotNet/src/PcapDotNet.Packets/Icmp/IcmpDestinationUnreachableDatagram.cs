using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792 and RFC 1191.
    /// <pre>
    /// +-----+------+------+--------------+
    /// | Bit | 0-7  | 8-15 | 16-31        |
    /// +-----+------+------+--------------+
    /// | 0   | Type | Code | Checksum     |
    /// +-----+------+------+--------------+
    /// | 32  | unused      | Next-Hop MTU |
    /// +-----+-------------+--------------+
    /// | 64  | Internet Header            |
    /// |     | + 64 bits of               |
    /// |     | Original Data Datagram     |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public class IcmpDestinationUnreachableDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        private static class Offset
        {
            public const int NextHopMtu = 6;
        }

        public IcmpDestinationUnreachableDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public ushort NextHopMtu
        {
            get { return ReadUShort(Offset.NextHopMtu, Endianity.Big); }
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpDestinationUnreachableLayer
                       {
                           Code = (IcmpCodeDestinationUnrechable)Code,
                           Checksum = Checksum,
                           NextHopMtu = NextHopMtu,
                       };
        }

        protected override bool CalculateIsValid()
        {
            return base.CalculateIsValid() &&
                   (((IcmpCodeDestinationUnrechable)Code == IcmpCodeDestinationUnrechable.FragmentationNeededAndDontFragmentSet) ||
                    NextHopMtu == 0);
        }

        protected override byte MinCodeValue
        {
            get { return _minCode; }
        }

        protected override byte MaxCodeValue
        {
            get { return _maxCode; }
        }

        private static readonly byte _minCode = (byte)typeof(IcmpCodeDestinationUnrechable).GetEnumValues<IcmpCodeDestinationUnrechable>().Min();
        private static readonly byte _maxCode = (byte)typeof(IcmpCodeDestinationUnrechable).GetEnumValues<IcmpCodeDestinationUnrechable>().Max();
    }
}