using System.IO;

namespace OA.Ultima.Core.UI.Fonts
{
    abstract class AFont : IFont
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
