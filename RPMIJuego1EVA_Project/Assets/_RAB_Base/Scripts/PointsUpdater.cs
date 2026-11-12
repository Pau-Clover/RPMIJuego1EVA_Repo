using UnityEngine;
using TMPro; //Librería que permite trabajar con objetos de Text Mesh Pro

public class PointsUpdater : MonoBehaviour
{
    public TMP_Text pointsText; //Ref al texto que indica los puntos, para poder modificar el texto
    public TMP_Text hpText; // texto que indica la vida
    public PlayerInteractor playerIntScript; //Ref al script que contiene los puntos y los winpoints. Para poder acceder a su valor

    // Update is called once per frame
    void Update()
    {
        //Actualizar constantemente el valor de puntos
        //Points: points/winpoints
        pointsText.text = "Points: " + playerIntScript.points.ToString() + "/" + playerIntScript.winPoints.ToString();
        // Actualiza constantemente el valor de la vida que queda
        hpText.text = "Health: " + playerIntScript.hp.ToString();
    }
}
