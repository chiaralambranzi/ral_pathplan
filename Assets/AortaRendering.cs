using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AortaRendering : MonoBehaviour
{
    public Material renderMaterial;
    public Material aortaMaterial;
    public Renderer aorta;

    public void OnPreRender()
    {
        aorta.material = aortaMaterial;
    }
    public void OnPostRender()
    {
        aorta.material = renderMaterial;
    }
}
