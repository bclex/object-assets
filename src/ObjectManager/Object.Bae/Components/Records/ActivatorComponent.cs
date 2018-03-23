using OA.Bae.Esm;

namespace OA.Bae.Components.Records
{
    public class ActivatorComponent : GenericObjectComponent
    {
        void Start()
        {
            usable = true;
            pickable = false;
            var ACTI = (ACTIRecord)record; 
            objData.name = ACTI.FNAM.value;
            objData.interactionPrefix = "Use ";
        }
    }
}