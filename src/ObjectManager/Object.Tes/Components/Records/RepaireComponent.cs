using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class RepaireComponent : GenericObjectComponent
    {
        void Start()
        {
            var REPA = (REPARecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = REPA.FNAM.value;
            objData.weight = REPA.RIDT.Weight.ToString();
            objData.value = REPA.RIDT.Value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}