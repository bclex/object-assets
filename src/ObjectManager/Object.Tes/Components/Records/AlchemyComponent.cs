using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class AlchemyComponent : GenericObjectComponent
    {
        void Start()
        {
            var ALCH = (ALCHRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = ALCH.FNAM.value;
            objData.weight = ALCH.ALDT.Weight.ToString();
            objData.value = ALCH.ALDT.Value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}
