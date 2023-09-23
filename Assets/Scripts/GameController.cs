using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject currentCube;
    public GameObject lastCube;
    public Text text;
    public int level;
    public bool done;

    public static GameController instance;

    void Awake()
    {
        if (instance != null) Destroy(instance.gameObject);
        instance = this;

        NewBlock();
    }

    void Update()
    {
        if (done) return;
        var time = Mathf.Abs(Time.realtimeSinceStartup % 2f - 1f);
        var evenLevel = level % 2 == 0;
        var pos1 = lastCube.transform.position + Vector3.up * 10f;
        var pos2 = pos1 + (evenLevel ? Vector3.left : Vector3.forward) * 120;
        if (evenLevel) currentCube.transform.position = Vector3.Lerp(pos2, pos1, time);
        else currentCube.transform.position = Vector3.Lerp(pos1, pos2, time);

        if (Input.GetMouseButtonDown(0)) NewBlock();
    }

    public void NewBlock()
    {
        if (lastCube != null)
        {
            currentCube.transform.position = new Vector3(Mathf.Round(currentCube.transform.position.x),
                currentCube.transform.position.y,
                Mathf.Round(currentCube.transform.position.z));
            currentCube.transform.position = new Vector3(lastCube.transform.localScale.x - Mathf.Abs(currentCube.transform.position.x - lastCube.transform.position.x),
                lastCube.transform.localScale.y,
                lastCube.transform.localScale.z - Mathf.Abs(currentCube.transform.position.z - lastCube.transform.position.z));

            currentCube.transform.position = Vector3.Lerp(currentCube.transform.position, lastCube.transform.position, 0.5f) + Vector3.up * 5f;

            if (currentCube.transform.localScale.x <= 0f ||
                currentCube.transform.localScale.z <= 0f)
            {
                done = true;
                text.gameObject.SetActive(true);
                text.text = "Final Score: " + level;
                StartCoroutine(RestartGameAfterDelay());
            }
        }

        lastCube = currentCube;
        currentCube = Instantiate(lastCube);
        currentCube.name = level + "";
        currentCube.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.HSVToRGB((level / 100f) % 1f, 1f, 1f));
        level++;
        Camera.main.transform.position = currentCube.transform.position + new Vector3(100, 100, 100);
        Camera.main.transform.LookAt(currentCube.transform.position);
    }

    private IEnumerator RestartGameAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
