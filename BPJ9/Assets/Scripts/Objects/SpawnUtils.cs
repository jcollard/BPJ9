using UnityEngine;

public class Spawner
{

    private GameObject toSpawn;
    private Vector2 position = new Vector2();

    private bool IsLocalPosition = false;
    private Transform parent = null;
    private string name = null;



    public static Spawner SpawnObject(GameObject obj)
    {
        return new Spawner(obj);
    }

    private Spawner(GameObject obj)
    {
        this.toSpawn = obj;
    }

    public Spawner Position(Vector2 position)
    {
        this.position = position;
        return this;
    }

    public Spawner LocalPosition(Vector2 position)
    {
        this.IsLocalPosition = true;
        this.position = position;
        return this;
    }

    public Spawner Parent(Transform parent)
    {
        this.parent = parent;
        return this;
    }

    public Spawner Name(string name)
    {
        this.name = name;
        return this;
    }

    public GameObject Spawn()
    {

        GameObject newObj = UnityEngine.Object.Instantiate(this.toSpawn);
        if (this.parent != null) newObj.transform.SetParent(parent);
        if (name != null) newObj.name = name;
        if (IsLocalPosition) newObj.transform.localPosition = this.position;
        else newObj.transform.position = this.position;
        
        LayerController lc = newObj.GetComponent<LayerController>();
        if (lc != null) lc.SetLayer();
        newObj.gameObject.SetActive(true);
        return newObj;
    }
}