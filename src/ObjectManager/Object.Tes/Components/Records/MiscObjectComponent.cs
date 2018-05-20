
using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class MiscObjectComponent : GenericObjectComponent
    {
        void Start()
        {
            var MISC = (MISCRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(MISC.ITEX.value, "icons"); 
            objData.name = MISC.FULL.Value;
            objData.weight = MISC.DATA.Weight.ToString();
            objData.value = MISC.DATA.Value.ToString();
            objData.interactionPrefix = "Take ";
        }

        public override void Interact() { }
    }
}
