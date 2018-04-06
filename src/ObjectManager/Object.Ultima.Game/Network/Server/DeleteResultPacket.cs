using OA.Core;
using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Resources;

namespace OA.Ultima.Network.Server
{
    public class DeleteResultPacket : RecvPacket
    {
        public string Result
        {
            get
            {
                // get the resource provider
                var provider = Service.Get<IResourceProvider>();
                switch ((DeleteResultType)_result)
                {
                    case DeleteResultType.PasswordInvalid: return provider.GetString(3000018); // 3000018: That character password is invalid.
                    case DeleteResultType.CharNotExist: return provider.GetString(3000019); // 3000019: That character does not exist.
                    case DeleteResultType.CharBeingPlayed: return provider.GetString(3000020); // 3000020: That character is being played right now.
                    case DeleteResultType.CharTooYoung: return provider.GetString(3000021); // 3000021: That character is not old enough to delete. The character must be 7 days old before it can be deleted.
                    case DeleteResultType.CharQueued: return provider.GetString(3000022); // 3000022: That character is currently queued for backup and cannot be deleted.
                    case DeleteResultType.BadRequest: return provider.GetString(3000023); // 3000023: Couldn't carry out your request.
                    default: return "Could not delete character.";
                }
            }
        }
        // enum from RunUO. Other values may be possible
        enum DeleteResultType
        {
            PasswordInvalid, // never sent by RunUO
            CharNotExist,
            CharBeingPlayed,
            CharTooYoung,
            CharQueued, // never sent by RunUO
            BadRequest
        }

        readonly byte _result;

        public DeleteResultPacket(PacketReader reader)
            : base(0x85, "Character Delete Result")
        {
            _result = reader.ReadByte();
        }
    }
}
