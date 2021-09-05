using UnityEngine;

public class FactionPolicies : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    private float _popTaxRate = 0.2f;
    public float popTaxRate { set; get; }

    // Start is called before the first frame update
    void Start()
    {
        popTaxRate = _popTaxRate;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
