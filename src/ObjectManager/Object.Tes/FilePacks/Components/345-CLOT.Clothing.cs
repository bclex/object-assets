using OA.Tes.FilePacks.Records;

namespace OA.Tes.FilePacks.Components
{
    public class CLOTComponent : BASEComponent
    {
        void Start()
        {
            var CLOT = (CLOTRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = CLOT.FULL.Value;
            objData.weight = CLOT.DATA.Weight.ToString();
            objData.value = CLOT.DATA.Value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}