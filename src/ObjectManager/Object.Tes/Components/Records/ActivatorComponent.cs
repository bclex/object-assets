using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class ActivatorComponent : GenericObjectComponent
    {
        void Start()
        {
            usable = true;
            pickable = false;
            var ACTI = (ACTIRecord)record; 
            objData.name = ACTI.FULL.Value;
            objData.interactionPrefix = "Use ";
        }
    }
}