using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Range(0.5f, 5f)]
    [SerializeField]
    private float _timeScale = 1;

    public static float timeScale;

    public static float elapsedTime = 0f;
    public static int dayLengthInSeconds = 30;
    public static int day = 0;
    public static float dayProgressPercent = 0;
    private float dayProgress = 0f;

    public static int updateIntervalDistrict = 5;
    public static int updateIntervalSettlement = 5;




    // Start is called before the first frame update
    void Start()
    {
        timeScale = _timeScale;
        Time.timeScale = timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
        elapsedTime += Time.deltaTime;
        dayProgress += Time.deltaTime;

        if (dayProgress >= dayLengthInSeconds)
        {
            dayProgress = 0;
            day++;
        }

        dayProgressPercent = (dayProgress / dayLengthInSeconds) * 100;
    }
}

