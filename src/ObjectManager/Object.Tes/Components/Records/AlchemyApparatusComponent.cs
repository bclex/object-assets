using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class AlchemyApparatusComponent : GenericObjectComponent
    {
        void Start()
        {
            var APPA = (APPARecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = APPA.FNAM.Value;
            objData.weight = APPA.AADT.Weight.ToString();
            objData.value = APPA.AADT.Value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}