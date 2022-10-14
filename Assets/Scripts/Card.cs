using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool inSelectZone = false;
    public bool isSelected = false;
    Material material;
    Material oldMaterial;
    Material[] myMaterials;
    Shader shader;
    Shader oldShader;

    // Start is called before the first frame update
    void Start()
    {
        /*
        shader = Shader.Find("Custom/DistortionRim");
        oldShader = GetComponent<Renderer>().material.shader;

        material = new Material(shader);
        material.color = Color.cyan;

        oldMaterial = GetComponent<Renderer>().material;

        myMaterials[0] = oldMaterial;
        myMaterials[1] = oldMaterial;*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter");

        inSelectZone = true;
        //GetComponent<Renderer>().materials = myMaterials;

        //oldMaterial.shader = shader;

        //GetComponent<Renderer>().material = material;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exit");

        inSelectZone = false;
        //GetComponent<Renderer>().material = oldMaterial;
    }

    public void Select()
    {
        if (!isSelected)
        {
            transform.position += new Vector3(0f, 0.04f, 0f);
            isSelected = true;
        }
    }

    public void Deselect()
    {
        if (isSelected)
        {
            transform.position += new Vector3(0f, -0.04f, 0f);
            isSelected = false;
        }
    }

    public void PlayCard()
    {
        if (isSelected)
        {

        }
    }
}
