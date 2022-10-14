using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTurning : MonoBehaviour
{
    [SerializeField]
    float Rotation_Speed;
    [SerializeField]
    float Rotation_Friction;
    [SerializeField]
    float Rotation_Smoothness; //modify this

    private float Residual_Force;

    private float Resulting_Values_from_Input;
    private Quaternion Quaternion_Rotate_From;
    private Quaternion Quaternion_Rotate_to;


    // Update is called once per frame
    void Update()
    {
        if(Residual_Force >= 0.001 || Residual_Force <= -0.001)
        {
            SlowTurn(Residual_Force);
        }
        else
        {
            Residual_Force = 0;
        }
    }

    public void SlowTurn(float force)
    {
        Resulting_Values_from_Input += force * Rotation_Speed * Rotation_Friction;
        Quaternion_Rotate_From = transform.rotation;
        Quaternion_Rotate_to = Quaternion.Euler(0, Resulting_Values_from_Input, 0);

        transform.rotation = Quaternion.Lerp(Quaternion_Rotate_From, Quaternion_Rotate_to, Time.deltaTime * Rotation_Smoothness);

        Residual_Force = force * Rotation_Friction;
    }
}
