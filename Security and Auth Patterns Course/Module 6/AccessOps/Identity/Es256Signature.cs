using System;
using System.Formats.Asn1;
using System.Numerics;

namespace AccessOps.Identity
{
    /// <summary>
    /// Works around how <c>WebAuthn.Validate</c> (Wisej-4-WebAuthn 4.1.7) reads an ES256 signature.
    /// It takes the DER <c>SEQUENCE { r INTEGER, s INTEGER }</c> by fixed offsets, drops one leading
    /// 0x00 from each integer and concatenates the rest. When r or s is a number shorter than 32
    /// bytes (DER encodes it in 31 bytes, or in 32 with a sign byte) the concatenation is not the 64
    /// bytes P-256 needs and a genuine signature fails - measured: 13 in 3,000 signatures that DER
    /// already encodes in 32 or 33 bytes, more when the 31-byte encodings are counted.
    /// <para>
    /// The fix re-encodes the same r and s, each as exactly 33 bytes (0x00 plus the value padded to
    /// 32 bytes), so the extension's "drop one leading zero" leaves exactly 32 bytes. The numbers do
    /// not change, so nothing the signature proves changes; a malformed input is passed through and
    /// fails verification as before.
    /// </para>
    /// </summary>
    public static class Es256Signature
    {
        /// <summary>The DER signature re-encoded for <c>WebAuthn.Validate</c>, or the input when it cannot be parsed.</summary>
        public static byte[] ForValidate(byte[] der)
        {
            if (der == null)
                return null;
            try
            {
                var reader = new AsnReader(der, AsnEncodingRules.DER);
                var sequence = reader.ReadSequence();
                var r = sequence.ReadIntegerBytes().ToArray();
                var s = sequence.ReadIntegerBytes().ToArray();
                sequence.ThrowIfNotEmpty();
                reader.ThrowIfNotEmpty();

                var rFixed = Fixed33(r);
                var sFixed = Fixed33(s);
                if (rFixed == null || sFixed == null)
                    return der;

                // 0x30 len 0x02 0x21 r(33) 0x02 0x21 s(33): the single-byte length form the extension expects
                var result = new byte[2 + 2 + 33 + 2 + 33];
                result[0] = 0x30;
                result[1] = (byte)(result.Length - 2);
                result[2] = 0x02;
                result[3] = 33;
                rFixed.CopyTo(result, 4);
                result[37] = 0x02;
                result[38] = 33;
                sFixed.CopyTo(result, 39);
                return result;
            }
            catch (AsnContentException)
            {
                return der;
            }
        }

        // the unsigned value, left-padded to 32 bytes, behind one 0x00
        private static byte[] Fixed33(byte[] integer)
        {
            var value = new BigInteger(integer, isUnsigned: false, isBigEndian: true);
            if (value.Sign <= 0)
                return null;
            var bytes = value.ToByteArray(isUnsigned: true, isBigEndian: true);
            if (bytes.Length > 32)
                return null;
            var result = new byte[33];
            bytes.CopyTo(result, 33 - bytes.Length);
            return result;
        }
    }
}
