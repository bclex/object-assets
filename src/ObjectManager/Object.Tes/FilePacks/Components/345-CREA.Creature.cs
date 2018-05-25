using UnityEngine;

namespace OA.Tes.FilePacks.Components
{
    public class CREAComponent : BASEComponent
    {
        void Start()
        {
            transform.rotation = Quaternion.Euler(-70, 0, 0); 
        }
    }
}
