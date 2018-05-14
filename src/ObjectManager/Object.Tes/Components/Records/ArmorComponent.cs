using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class ArmorComponent : GenericObjectComponent
    {
        void Start()
        {
            var ARMO = (ARMORecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = ARMO.FNAM.Value;
            objData.weight = ARMO.AODT.Weight.ToString();
            objData.value = ARMO.AODT.Value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}