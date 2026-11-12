using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement; //Librería de carga/descarga de escenas

public class PlayerInteractor : MonoBehaviour
{
    [Header("Respawn Configuration")]
    public Transform respawnPoint; //Ref al punto de respawn para usar su posición
    public float respawnFallLimit = -10; //Valor negativo en eje Y a partir del cual se respawnea
    public int sceneToRespawn; //Número de escena en la build a respawnear

    [Header("Point System")]
    public int points; //Cantidad de puntos actuales del player
    public int winPoints = 5; //Cantidad de puntos necesarios para ganar
    public int sceneToLoad = 2; //Número de escena a cargar cuando se consigue la victoria

    [Header("Health Point system")]
    public int hp;
    private int maxHp = 3;

    [Header("Reference Variables")]
    public Rigidbody playerRb; //Ref al rigidbody para resetear su motor de aceleración al respawnear

    [Header("SFX Configuration")]
    public AudioClip[] sfxList; //Array de sonidos: diferentes sonidos agrupados para diferentes acciones
    public AudioSource sfxSource; //Ref al altavoz del player: referenciarlo permite reproducir sonidos cuando queramos


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points = 0; //Cuando empieza la partida, los puntos se ponen solos en cero
    }

    // Update is called once per frame
    void Update()
    {
        //Respawn por caída
        if (transform.position.y <= respawnFallLimit) 
        { 
            Respawn();
        }

        //Condición de victoria: Cuando points alcanza winpoints, cambiamos de escena
        if (points >= winPoints)
        {
            ChangeScene();
        }

        healthControl();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            //Respawn por choque de obstáculo
            Respawn();
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("PickUp"))
        {
            points += 1; //Suma un punto al total de puntos actuales
            sfxSource.PlayOneShot(sfxList[2]); //Reproducir el sonido de pick up
            other.gameObject.SetActive(false); //Apagar el objeto con el que chocas
        }
        if (other.gameObject.CompareTag("Switch"))
        {
            sfxSource.PlayOneShot(sfxList[3]); //Reproducir el sonido asociado al switch
            other.gameObject.SetActive(false); //Apagar el objeto con el que chocas
        }
        // recuperar vida
        if (other.gameObject.CompareTag("Heal")) 
        {
            other.gameObject.SetActive(false); //apaga la cura
            hp = hp + 1; // suma una vida
        }
        
    }

    void Respawn()
    {
        // 1. Quita un punto de vida
        hp = hp - 1;
        //2- Cambia la posición del player por la del punto de respawn
        transform.position = respawnPoint.position;
        //3- Resetea la velocidad del Rigidbody para evitar acumulaciones de aceleración
        playerRb.linearVelocity = new Vector3(0, 0, 0);
        //4- Resetea la velocidad de rotación
        playerRb.angularVelocity = new Vector3(0, 0, 0);
        //5- Reproducir el sonido de respawn desde el audiosource del player
        sfxSource.PlayOneShot(sfxList[1]);
    }

    void healthControl()
    {
        // No puede superar 3 de vida
        if (hp > 3) 
        {
            hp = 3;
        }
        // Game over
        if (hp == 0) 
        {
            // Sujeto a cambios, deberia llevar a una escena de pantalla de game over pero de momento se queda en el main menu
            SceneManager.LoadScene(0);
        }
        
    }

    void ChangeScene()
    {
        //Acción que al ejecutarse carga la escena asignada a la variable sceneToLoad
        SceneManager.LoadScene(sceneToLoad);
    }



}
