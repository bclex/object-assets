using OA.Tes.FilePacks.Records;

namespace OA.Tes.FilePacks.Components
{
    public class ALCHComponent : BASEComponent
    {
        void Start()
        {
            var ALCH = (ALCHRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = ALCH.FULL.Value;
            objData.weight = ALCH.DATA.Weight.ToString();
            objData.value = ALCH.DATA.Value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}
