using System.IO;

namespace OA.Ultima.Core.UI
{
    public interface IFont
    {
        int Baseline { get; }
        int Height { get; }
        ICharacter GetCharacter(char ch);
        bool HasBuiltInOutline { get; }
    }

    public abstract class AFont : IFont
    {
        public bool HasBuiltInOutline { set; get; }

        public int Height { get; set; }

        public int Baseline
        {
            get { return GetCharacter('M').Height + GetCharacter('M').YOffset; }
        }

        public abstract ICharacter GetCharacter(char character);

        public abstract void Initialize(BinaryReader reader);
    }
}
