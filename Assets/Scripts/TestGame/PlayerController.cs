using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;

    [Header("Configuración de Movimiento e Interfaz")]
    public float speed = 10;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    [Header("Configuración de Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);

        // Si no asignaste el Audio Source manualmente en el Inspector, intenta obtenerlo del mismo GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;

            // Reproduce el sonido de recolección
            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        if (count >= 12)
        {
            winTextObject.SetActive(true);

            // Reproduce el sonido de victoria
            if (audioSource != null && winSound != null)
            {
                audioSource.PlayOneShot(winSound);
            }

            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Reproduce el sonido de derrota en la posición actual antes de destruir al Player
            if (loseSound != null)
            {
                AudioSource.PlayClipAtPoint(loseSound, transform.position);
            }

            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";

            Destroy(gameObject);
        }
    }
}