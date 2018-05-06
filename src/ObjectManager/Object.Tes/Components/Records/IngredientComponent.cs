using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class IngredientComponent : GenericObjectComponent
    {
        void Start()
        {
            var INGR = (INGRRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(INGR.ITEX.value, "icons"); 
            objData.name = INGR.FNAM.Value;
            objData.weight = INGR.IRDT.Weight.ToString();
            objData.value = INGR.IRDT.Value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}
