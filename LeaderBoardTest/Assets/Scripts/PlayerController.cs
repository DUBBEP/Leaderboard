using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlayerController : MonoBehaviour
{

    private Rigidbody rig;

    private float startTime;
    private float timeTaken;

    private int collectablePicked;

    private bool playerHasControl, overMaxTriggered, isPlaying;

    public float speed, maxFallSpeed, tempX, tempZ;
    
    public float bounceForce;

    public GameObject playButton;
    public TextMeshProUGUI curTimeText;
    public GameObject ColletablePrefab;
    public GameObject ObstaclePrefab;

    public List<GameObject> collectablesList;
    public List<GameObject> obstaclesList;
 
    public Rigidbody cameraRig;

    void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rig.useGravity = false;
        cameraRig.useGravity = false;
    }

    void Update()
    {
        if (!isPlaying)
            return;



        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;

/*        if ( rig.velocity.magnitude + rig.velocity.y  > maxSpeed)
        {
            if (!overMaxTriggered)
            {
                Debug.Log("New Temps Set");
                tempX = rig.velocity.x;
                tempZ = rig.velocity.z;
                overMaxTriggered = true;
            }
            rig.velocity = new Vector3(tempX, rig.velocity.y, tempZ);
        }
        else if (rig.velocity.magnitude + rig.velocity.y < maxSpeed)
        {
            overMaxTriggered = false;
        }*/

        if (-rig.velocity.y > maxFallSpeed)
            rig.velocity = new Vector3 (rig.velocity.x, -maxFallSpeed, rig.velocity.z);

        if (playerHasControl)
            rig.AddForce(new Vector3(x, 0, z));
        
        cameraRig.velocity = new Vector3(0, rig.velocity.y, 0);

    }


    public void Begin()
    {
        curTimeText.text = "0";
        this.transform.position = Vector3.zero;
        rig.velocity = Vector3.zero;
        cameraRig.gameObject.transform.position = new Vector3(0, 18, 0);
        collectablePicked = 0;

        SpawnCollectables();
        SpawnObstacles();

        startTime = Time.time;
        isPlaying = true;
        playerHasControl = true;
        playButton.SetActive(false);
        rig.useGravity = true;
    }

    // Spawn Collectabels along the Play Area
    void SpawnCollectables()
    {
        // Delete any existing Collectables in the scene
        if (collectablesList.Count > 0)
        {
            for (int i = 0; i < collectablesList.Count; i++)
            {
                Destroy(collectablesList[0]);
                collectablesList.Remove(collectablesList[0]);

            }
        }

        // Spawn Colletables along the range of the fall
        for (int i = -25; i > -1000; i -= 50)
        {
            // Spawn between 0 and 5 collectables per loop
            int possibleCollectables = Random.Range(1, 5);

            for (int j = 0; j < possibleCollectables; j++)
            {
                // spawn weach collectable in a random location within the walls of the play area
                // including a vertical offset
                int x = Random.Range(-6, 6);
                int y = Random.Range(i - 25, i + 25);
                int z = Random.Range(-6, 6);

                // create the collectable and add it to the collectable list
                collectablesList.Add(Instantiate(ColletablePrefab, new Vector3(x, y, z), Quaternion.identity));
            }
        }
    }

    // Spawn obstacles along the play area
    void SpawnObstacles()
    {
        // if there are obstacles then destroy them
        if (obstaclesList.Count > 0)
        {
            for (int i = 0; i < obstaclesList.Count; i++)
            {
                Destroy(obstaclesList[0]);
                obstaclesList.Remove(obstaclesList[0]);
            }
        }

        // spawn an obstacle every 150 units of distance
        for (int i = -150; i > -1000; i -= 150)
        {
            // spawn between 0 and 2 obstacles every loop
            int possibleObstacles = Random.Range(0, 3);

            for (int j = 0; j < possibleObstacles; j++)
            {
                // give each obstacle a random offset and rotation
                int x = Random.Range(-6, 6);
                int y = Random.Range(i - 50, i + 50);
                int z = Random.Range(-6, 6); 
                int angleX = Random.Range(-50, 50);
                int angleY = Random.Range(-45, 45);
                int angleZ = Random.Range(-45, 45);

                // spawn the obstacle with a random offset and rotation and add it to our obstacle list
                obstaclesList.Add(Instantiate(ObstaclePrefab, new Vector3(x, y, z), Quaternion.Euler(angleZ, angleX, angleY)));
            }
        }
    }

    void End()
    {
        timeTaken = Time.time - startTime;
        isPlaying = false;
        playButton.SetActive(true);
        Leaderboard.instance.SetLeaderboardEntry(collectablePicked);
        cameraRig.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            End();
        }
/*        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector3 dir = collision.contacts[0].normal;
            rig.velocity = new Vector3(0, rig.velocity.y, 0);
            StartCoroutine(PauseControls());
            rig.AddForce(dir * bounceForce, ForceMode.Impulse);
        }*/
    }

/*    IEnumerator PauseControls()
    {
        playerHasControl = false;
        yield return new WaitForSeconds(0.1f);
        playerHasControl = true;
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable"))
        {
            collectablePicked++;
            curTimeText.text = collectablePicked.ToString();
            other.gameObject.SetActive(false);

        }
        if (other.gameObject.CompareTag("EndTrigger"))
        {
            End();
        }
    }
}
