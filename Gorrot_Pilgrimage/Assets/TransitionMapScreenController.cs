using GorrotGame;
using System.Collections;
using UnityEngine;

public class TransitionMapScreenController : MonoBehaviour
{
    [SerializeField] GameObject[] allMapLocationNodes;

    [SerializeField] GameObject playerMarker;

    MapNames cache_fromMap;
    MapNames cache_toMap;
    GameObject cache_FromMapNode;
    GameObject cache_ToMapNode;



    public void StartMapTransition(MapNames fromMap, MapNames toMap)
    {
        Debug.Log("Go from: " + fromMap.ToString() + ", to: " + toMap.ToString());

        cache_fromMap = fromMap;
        cache_toMap = toMap;

        GetMapNode();
    }

 

    void GetMapNode()
    {
      

        foreach(var node in allMapLocationNodes)
        {

            MapLocationNodeController mapNodeController = node.GetComponent<MapLocationNodeController>();

            if (mapNodeController.ThisMapName == cache_fromMap)
            {
                cache_FromMapNode = node;
                
            }
            if (mapNodeController.ThisMapName == cache_toMap)
            {
                cache_ToMapNode = node;
            }
        }


       playerMarker.transform.position = cache_FromMapNode.transform.position;

        
        StartCoroutine(PlayerTransition());
    }

    IEnumerator PlayerTransition()
    {

        Debug.Log("In Coroutine");

        if (cache_FromMapNode == null || cache_ToMapNode == null)
            yield break;


        Vector3 fromPosition = cache_FromMapNode.transform.position;
        Vector3 toPosition = cache_ToMapNode.transform.position;

        float duration = 1.5f; // tweakable later
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            playerMarker.transform.position = Vector3.Lerp(fromPosition, toPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to exact final position (avoids tiny float errors)
        playerMarker.transform.position = toPosition;
    }
}
