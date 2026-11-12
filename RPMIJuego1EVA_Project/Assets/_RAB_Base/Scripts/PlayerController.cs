using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float speed; //velocidad del player
    public float jumpForce; //fuerza de salto
    public bool useLinearVelocity = true; //true = movimiento físico por motor de aceleración
    public bool groundCheck; //detecta si la pelota toca el suelo

    [Header("Reference Variables")]
    public Rigidbody playerRb; //Almacén de referencia del rb, una vez relleno permite modificar sus variables internas
    public Vector2 moveInput; //Almacén de los valores de pulsación de movimiento (0, 1, -1)
    private bool canDash = true;
    public AudioSource sfxSource; //Ref al altavoz del player, para reproducir sonidos
    public PlayerInteractor playerInteractor; //Ref al script, permite acceder a su lista de sonidos públicos
    private Vector3 dashDirection;
    public Transform cameraTransform;
    private float originalInputX;
    private float originalInputY;

    [Header("Dash Settings")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Ejecuta todo lo que depende del procesador y ejecuta el input
        //CinematicMove();
    }

    private void FixedUpdate()
    {
        //FixedUpdate ejecuta todo lo que tenga que ver con el motor de físicas
        PhysicalMove();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundCheck = true; //Al chocar con el suelo se devuelve la posibilidad de saltar
        }
    }


    void PhysicalMove()
    {
            //Tipo de movimiento por fuerza de empuje en cada eje del rigidbody
            //A cada eje se le aplica un AddForce en la dirección concreta
            playerRb.AddForce(Vector3.right * speed * moveInput.x);
            playerRb.AddForce(Vector3.forward * speed * moveInput.y);
        
    }

    void Jump()
    {
        groundCheck = false; //Al saltar, se quita la posibilidad de volver a saltar hasta tocar suelo
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //Reproducir sonido de salto: accede al player interactor y busca el sonido exacto en la lista
        sfxSource.PlayOneShot(playerInteractor.sfxList[0]);
    }

    IEnumerator Dash()         //IEnumerator es una variable que permite hacer una secuencia de acciones con pausas entre medias
                               //(que no lo hace todo de una vamos)
    {
        canDash = false;
        useLinearVelocity = true;
        originalInputX = moveInput.normalized.x; //Almacenamos los valores del input para que la pelota no pueda
        originalInputY = moveInput.normalized.y; //cambiar de dirección durante el dash

        sfxSource.PlayOneShot(playerInteractor.sfxList[4]);

        //Para mantener el dash mientras dure, con su timer para que determina lo que dura
        float timer = 0f;

        while (timer < dashDuration)
        {
            playerRb.linearVelocity = new Vector3(dashSpeed * originalInputX, 0, dashSpeed * originalInputY); //Mantiene el dash en una dirección,
                                                                                                                                      //con la velocidad que le hayamos puesto

            timer += Time.fixedDeltaTime; //Para que el timer vaya sumando segundos
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForFixedUpdate();
        playerRb.linearVelocity = new Vector3(speed * originalInputX, playerRb.linearVelocity.y, speed * originalInputY); //Frena la pelota para dar un efecto
                                                                                                                          //de dash mas convincente
        useLinearVelocity = false;
        //Esperamos cooldown
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;

    }

    #region Input Methods
    //Declaramos los métodos de input (eventos de unity)
    //Siguen esta estructura: public void NombreAccion(referencia al contexto = botones de la acción)
    public void OnMove(InputAction.CallbackContext context)
    {
        //Se rellena la variable de referencia con el valor de cualquier contexto
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //Si aprieto el botón y si estoy en el suelo...
        if (context.performed && groundCheck == true)
        {
            Jump();
        }
        
    }

    public void OnDash(InputAction.CallbackContext context)
    {

        if (context.performed && canDash == true && (moveInput.x != 0 || moveInput.y !=0))
        {
            StartCoroutine(Dash());
        }
    }

    #endregion
}
