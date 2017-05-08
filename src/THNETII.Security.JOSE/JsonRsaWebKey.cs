using System;
using System.Runtime.Serialization;
using THNETII.Common;

namespace THNETII.Security.JOSE
{
    [DataContract]
    public class JsonRsaWebKey : JsonWebKey
    {
        private readonly DuplexConversionTuple<string, byte[]> n = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> e = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> d = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> p = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> q = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> dp = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> dq = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> qi = CreateBase64UrlDuplexConversionTuple();

        /// <inheritdoc />
        public override string KeyTypeString
        {
            get => base.KeyTypeString;
            set => base.KeyTypeString = (value == "RSA" ? value : throw new ArgumentException($"{nameof(value)} must be \"RSA\".", nameof(value)));
        }

        /// <inheritdoc />
        public override JsonWebKeyType KeyType
        {
            get => base.KeyType;
            set => base.KeyType = (value == JsonWebKeyType.Rsa ? value : throw new ArgumentException($"value must be {JsonWebKeyType.Rsa}.", nameof(value)));
        }

        [IgnoreDataMember]
        public byte[] N
        {
            get => n.ConvertedValue;
            set => n.ConvertedValue = value;
        }

        [IgnoreDataMember]
        public byte[] E
        {
            get => e.ConvertedValue;
            set => e.ConvertedValue = value;
        }

        [IgnoreDataMember]
        public byte[] D
        {
            get => d.ConvertedValue;
            set => d.ConvertedValue = value;
        }

        [IgnoreDataMember]
        public byte[] P
        {
            get => p.ConvertedValue;
            set => p.ConvertedValue = value;
        }

        [IgnoreDataMember]
        public byte[] Q
        {
            get => q.ConvertedValue;
            set => q.ConvertedValue = value;
        }

        [IgnoreDataMember]
        public byte[] DP
        {
            get => dp.ConvertedValue;
            set => dp.ConvertedValue = value;
        }

        [IgnoreDataMember]
        public byte[] DQ
        {
            get => dq.ConvertedValue;
            set => dq.ConvertedValue = value;
        }

        [IgnoreDataMember]
        public byte[] QI
        {
            get => qi.ConvertedValue;
            set => qi.ConvertedValue = value;
        }

        [DataMember(Name = "n")]
        public string NBase64String
        {
            get => n.RawValue;
            set => n.RawValue = value;
        }

        [DataMember(Name = "e")]
        public string EBase64String
        {
            get => e.RawValue;
            set => e.RawValue = value;
        }

        [DataMember(Name = "d", IsRequired = false, EmitDefaultValue = false)]
        public string DBase64String
        {
            get => d.RawValue;
            set => d.RawValue = value;
        }

        [DataMember(Name = "p", IsRequired = false, EmitDefaultValue = false)]
        public string PBase64String
        {
            get => p.RawValue;
            set => p.RawValue = value;
        }

        [DataMember(Name = "q", IsRequired = false, EmitDefaultValue = false)]
        public string QBase64String
        {
            get => q.RawValue;
            set => q.RawValue = value;
        }

        [DataMember(Name = "dp", IsRequired = false, EmitDefaultValue = false)]
        public string DPBase64String
        {
            get => dp.RawValue;
            set => dp.RawValue = value;
        }

        [DataMember(Name = "dq", IsRequired = false, EmitDefaultValue = false)]
        public string DQBase64String
        {
            get => dq.RawValue;
            set => dq.RawValue = value;
        }

        [DataMember(Name = "qi", IsRequired = false, EmitDefaultValue = false)]
        public string QIBase64String
        {
            get => qi.RawValue;
            set => qi.RawValue = value;
        }

        private static DuplexConversionTuple<string, byte[]> CreateBase64UrlDuplexConversionTuple()
        {
            return new DuplexConversionTuple<string, byte[]>(
                Base64UrlEncoder.FromBase64UrlString,
                Base64UrlEncoder.ToBase64UrlString
                );
        }

        public JsonRsaWebKey() : base()
        {
            KeyType = JsonWebKeyType.Rsa;
        }
    }
}