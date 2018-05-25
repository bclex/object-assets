using OA.Tes.FilePacks.Records;

namespace OA.Tes.FilePacks.Components
{
    public class ARMOComponent : BASEComponent
    {
        void Start()
        {
            var ARMO = (ARMORecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = ARMO.FULL.Value;
            objData.weight = ARMO.DATA.Weight.ToString();
            objData.value = ARMO.DATA.Value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}