
using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class MiscObjectComponent : GenericObjectComponent
    {
        void Start()
        {
            var MISC = (MISCRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(MISC.ITEX.value, "icons"); 
            objData.name = MISC.FNAM.value;
            objData.weight = MISC.MCDT.Weight.ToString();
            objData.value = MISC.MCDT.Value.ToString();
            objData.interactionPrefix = "Take ";
        }

        public override void Interact() { }
    }
}
