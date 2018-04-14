using UnityEngine;

namespace OA.Tes.Components.Records
{
    public class CreatureComponent : GenericObjectComponent
    {
        void Start()
        {
            transform.rotation = Quaternion.Euler(-70, 0, 0); 
        }
    }
}
