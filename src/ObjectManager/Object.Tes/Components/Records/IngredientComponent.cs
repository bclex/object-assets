using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class IngredientComponent : GenericObjectComponent
    {
        void Start()
        {
            var INGR = (INGRRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(INGR.ITEX.value, "icons"); 
            objData.name = INGR.FULL.Value;
            objData.weight = INGR.DATA.Weight.ToString();
            objData.value = INGR.DATA.Value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}
