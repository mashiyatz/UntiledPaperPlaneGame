using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using MinimapRadar;

public class PaperPlaneController : MonoBehaviour
{
    public enum FlightState { CRUISE, BOOST, GROUNDED, FREEFALL, COLLIDE }
    public FlightState currentState;
    public TextMeshProUGUI altitudeTextbox;
    public TextMeshProUGUI speedTextbox;
    public GameObject gameManager;
    public GameObject greatTreeGO;
    public Image fadeoutPanel;
    private AudioSource audioSource;

    public float groundSpeed = 5f;
    private float currentGroundSpeed;
    private float turnSpeed = 60f;
    private float inclineSpeed = 45f;
    private float maxHeight = 100f;
    private float timePenalty = 3f;
    private float windSpeed = 2f;

    private Rigidbody rb;
    private float timeAtBoost;
    private float timeAtGrounded;
    private int landmarksAcquired = 0;

    public SFXPlayer sfx;
    public TextMeshProUGUI notificationTextbox;
    public ParticleSystem planeParticle;

    private float gameOverTime = 600f;
    private Vector3 startPos;
    private Vector3 startRot;

    [SerializeField] private GameObject[] virtualCameras; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentState = FlightState.CRUISE;
        currentGroundSpeed = groundSpeed;
        virtualCameras[2].SetActive(true);
        notificationTextbox.gameObject.SetActive(false);
        audioSource = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
        startPos = transform.position;
        startRot = transform.localEulerAngles;

        StartCoroutine(FadeInBackground());
        StartCoroutine(FadeInSound());
    }

    IEnumerator FadeInBackground(float time = 5f)
    {
        float startTime = Time.time;
        while (Time.time - startTime <= time)
        {
            float a  = Mathf.Lerp(0, 1, (Time.time - startTime) / time);
            Color newColor = Color.white;
            newColor.a = 1 - a;
            fadeoutPanel.color = newColor;
            yield return null;
        }
        if (virtualCameras[2].activeSelf) virtualCameras[2].SetActive(false);
    }

    IEnumerator FadeOutToMainMenu()
    {
        float startTime = Time.time;
        while (Time.time - startTime <= 5f)
        {
            float a = Mathf.Lerp(0, 1, (Time.time - startTime) / 5f);
            Color newColor = Color.black;
            newColor.a = a;
            audioSource.volume = Mathf.Clamp(1 - a, 0.2f, 1);
            fadeoutPanel.color = newColor;
            yield return null;
        }
        SceneManager.LoadScene("StartMenu");
    }

    IEnumerator FadeInSound()
    {
        float startTime = Time.time;
        while (Time.time - startTime <= 15f)
        {
            float a = Mathf.Lerp(0.2f, 1, (Time.time - startTime) / 10f);
            audioSource.volume = a;
            yield return null;
        }
        // Debug.Log("Sound at max volume.");
    }

    IEnumerator IncreasePitch()
    {
        float currentPitch = audioSource.pitch;
        float nextPitch = audioSource.pitch + 0.025f;
        // Debug.Log(currentPitch);
        // Debug.Log($"next pitch: {nextPitch}");
        float startTime = Time.time;
        while (Time.time - startTime <= 10f)
        {
            audioSource.pitch = Mathf.Lerp(currentPitch, nextPitch, (Time.time - startTime) / 10f);
            yield return null;
        }
    }

/*    IEnumerator DisplayMessage(string message)
    {
        notificationTextbox.text = message;
        notificationTextbox.gameObject.GetComponent<ChangeOpacityOverTime>().ResetTime();
        notificationTextbox.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.9f);
        notificationTextbox.gameObject.SetActive(false);
    }*/

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            landmarksAcquired = 4; // for debugging
            greatTreeGO.GetComponent<Collider>().enabled = true;
        }

        /*        if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    PlayerPrefs.SetInt("BestTime", 600);
                }*/

        if (Input.GetKeyDown(KeyCode.Y)) currentGroundSpeed += 5; // for debugging

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(FadeOutToMainMenu());
        }

        if (TrackScore.timeElapsed > gameOverTime)
        {
            Debug.Log(TrackScore.timeElapsed);
            StartCoroutine(FadeOutToMainMenu());
        }
        altitudeTextbox.text = $"{transform.position.y:N0}m";
        speedTextbox.text = $"{currentGroundSpeed:N0}mps";
    }

    void FixedUpdate()
    {
        Vector3 m_Input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (currentState == FlightState.CRUISE)
        {
            float roll = transform.eulerAngles.z - turnSpeed * Time.deltaTime * m_Input.x;
            float pitch = transform.eulerAngles.x - inclineSpeed * Time.deltaTime * m_Input.z;

            float theta = Vector3.SignedAngle(Vector3.up, transform.up, transform.forward);
            float rotationSpeed = Mathf.Lerp(30f, -30f, (theta + 180) / 360);

            float yaw = transform.eulerAngles.y + rotationSpeed * Time.deltaTime;

            Vector3 planeAngle;
            planeAngle.x = ClampAngle(pitch, -60, 60);
            planeAngle.y = yaw;
            planeAngle.z = ClampAngle(roll, -70, 70);

            // transform.eulerAngles = planeAngle;
            rb.MoveRotation(Quaternion.Euler(planeAngle));
            rb.MovePosition(rb.position + Time.deltaTime * currentGroundSpeed * transform.forward);
            if (currentGroundSpeed > 5f) currentGroundSpeed -= Time.deltaTime / 20;

            if (rb.position.y > maxHeight && landmarksAcquired < 4)
            {
                currentState = FlightState.FREEFALL;
            } 
        }

        if (currentState == FlightState.GROUNDED)
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            if (Time.time - timeAtGrounded > timePenalty)
            {
                rb.AddForce(new Vector3(0, 0.05f, 0), ForceMode.Impulse);
                timeAtBoost = Time.time;
                currentState = FlightState.BOOST;
                virtualCameras[0].SetActive(true);
            }
        }

        if (currentState == FlightState.BOOST)
        {
            if (Time.time - timeAtBoost < 0.5f)
            {
                float roll = Mathf.LerpAngle(transform.eulerAngles.z, 0, (Time.time - timeAtBoost) / 0.5f);
                float pitch = Mathf.LerpAngle(transform.eulerAngles.x, 0, (Time.time - timeAtBoost) / 0.5f);
                Vector3 planeAngle = transform.eulerAngles;
                planeAngle.z = roll;
                planeAngle.x = pitch;
                transform.eulerAngles = planeAngle;
            } else
            {
                rb.velocity = Vector3.zero;
                rb.useGravity = false;
                currentGroundSpeed = groundSpeed;
                currentState = FlightState.CRUISE;
            }
        }

        if (currentState == FlightState.FREEFALL)
        {
            rb.useGravity = true;
            if (rb.position.y < 20)
            {
                rb.velocity = Vector3.zero; 
                rb.useGravity = false;
                currentGroundSpeed = groundSpeed;
                currentState = FlightState.CRUISE;
            }
        }

        if (currentState == FlightState.COLLIDE)
        {
            rb.MovePosition(rb.position + Time.deltaTime * currentGroundSpeed * transform.forward);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Terrain"))
        {
            currentState = FlightState.GROUNDED;
            virtualCameras[0].SetActive(false);
            //rb.useGravity = true;
            StartCoroutine(GravityTimer());
            timeAtGrounded = Time.time;
        }

        if (collision.collider.CompareTag("Obstacle"))
        {
            transform.rotation = Quaternion.LookRotation(-transform.forward);
            rb.useGravity = true;
            currentState = FlightState.COLLIDE;
        }
    }

    IEnumerator GravityTimer()
    {
        rb.useGravity = true;
        yield return new WaitForSeconds(0.2f);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Landmark"))
        {
            sfx.PlayStampSound();
            landmarksAcquired += 1;
            var emission = planeParticle.emission;
            emission.rateOverTimeMultiplier *= 2f;
            StartCoroutine(IncreasePitch());
            other.GetComponent<StampAssignment>().CollectStamp();
            if (landmarksAcquired == 4)
            {
                greatTreeGO.GetComponent<MinimapItem>().enabled = true;
                greatTreeGO.GetComponent<BoxCollider>().enabled = true;
                audioSource.gameObject.GetComponent<AudioReverbFilter>().enabled = true;
            }
        }

        if (other.CompareTag("Cloud"))
        {
            // StartCoroutine(DisplayMessage("Clouds shift over time"));
            transform.rotation = Quaternion.LookRotation(-transform.forward);
            currentState = FlightState.COLLIDE;
        }

        if (other.CompareTag("Windmill"))
        {
            sfx.PlayWindSound();
        }

        if (other.CompareTag("Goal") && landmarksAcquired == 4)
        {
            notificationTextbox.text = "Fly higher!";
            notificationTextbox.gameObject.SetActive(true);
        }
    }

    IEnumerator TransitionBackToStart()
    { 
        float startTime = Time.time;
        while (Time.time - startTime <= 2.5f)
        {
            float a = Mathf.Lerp(0, 1, (Time.time - startTime) / 2.5f);
            Color newColor = Color.black;
            newColor.a = a;
            fadeoutPanel.color = newColor;
            yield return null;
        }
        transform.position = startPos;
        transform.localEulerAngles = startRot;
        currentGroundSpeed = 5.0f;
        StartCoroutine(FadeInBackground(2.5f));
        currentState = FlightState.CRUISE;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cloud"))
        {
            currentState = FlightState.CRUISE;
        }

        if (other.CompareTag("Goal"))
        {
            notificationTextbox.gameObject.SetActive(false);
        }

        if (other.CompareTag("Boundary"))
        {
            // currentState = FlightState.BOUND;
            StartCoroutine(TransitionBackToStart());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Windmill"))
        {
            // Vector3 newDirection = Vector3.RotateTowards(transform.forward, other.transform.forward, turnSpeed * 2, 0.0f);
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, -other.transform.up, turnSpeed * 2, 0.0f); // for my windmill model
            transform.rotation = Quaternion.LookRotation(newDirection);
            currentGroundSpeed += windSpeed * Time.deltaTime;
        }

        if (other.CompareTag("Goal") && landmarksAcquired == 4)
        {

            if (rb.position.y > 75)
            {
                float a = Mathf.Lerp(0, 1, (rb.position.y - 75) / 75);
                Color newColor = Color.white;
                newColor.a = a;
                fadeoutPanel.color = newColor;
                audioSource.volume = 1 - a;
            }

            if (rb.position.y > 150)
            {
                audioSource.pitch = 1;
                gameManager.GetComponent<TrackScore>().UpdateHighScore();
                SceneManager.LoadScene("EndScene");
            }
        }
    }

    static float ClampAngle(float angle, float min, float max)
    {
        // https://forum.unity.com/threads/limiting-rotation-with-mathf-clamp.171294/
        if (min < 0 && max > 0 && (angle > max || angle < min))
        {
            angle -= 360;
            if (angle > max || angle < min)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
                else return max;
            }
        }
        else if (min > 0 && (angle > max || angle < min))
        {
            angle += 360;
            if (angle > max || angle < min)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
                else return max;
            }
        }

        if (angle < min) return min;
        else if (angle > max) return max;
        else return angle;
    }


}
