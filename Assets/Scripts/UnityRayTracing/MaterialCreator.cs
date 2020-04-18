using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class MaterialCreator : MonoBehaviour
{

    public float _Metallic;
    public float _Roughness;
    public float _IoR;
    public Color _Emission;
    public Color _Color;
    public Texture2D _MainTex = null;
    private void Awake()
    {
        Material mat = GetComponent<MeshRenderer>().material;
        mat = new Material(Shader.Find("Custom/HitShader"));
        mat.SetColor("_Color", _Color);
        mat.SetColor("_Emission", _Emission);
        mat.SetFloat("_Metallic", _Metallic);
        mat.SetFloat("_Roughness", _Roughness);
        mat.SetFloat("_IoR", _IoR);
        if (_MainTex)
        {
            mat.SetTexture("_MainTex", _MainTex);
        }
    }

}
