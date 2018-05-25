using OA.Tes.FilePacks.Records;

namespace OA.Tes.FilePacks.Components
{
    public class ACTIComponent : BASEComponent
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