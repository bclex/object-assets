using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class ClothComponent : GenericObjectComponent
    {
        void Start()
        {
            var CLOT = (CLOTRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = CLOT.FNAM.value;
            objData.weight = CLOT.CTDT.Weight.ToString();
            objData.value = CLOT.CTDT.Value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}