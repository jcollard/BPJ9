
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class OnDeathExplode : MonoBehaviour
{
    public GameObject ToDisable;
    public GameObject Explosion;
    public float StartAt = -1;
    public float Duration = 1f;

    public void Start()
    {
        this.GetComponent<EnemyController>().OnDestroy = this.DoDestroy;
    }

    public void Update()
    {
        if (StartAt < 0) return;
        if (Time.time > (StartAt + Duration))
        {
            this.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

    private void DoDestroy(EnemyController ec)
    {
        StartAt = Time.time;
        ToDisable.SetActive(false);
        Explosion.SetActive(true);
    }

}